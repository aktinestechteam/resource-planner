$(function () {
    let currentSheets = [];
    const planId = $('#planId').val();
    $.ajax({
        url: '/Plan/GetPlanById/' + planId,
        method: 'GET',
        success: function (data) {
            if (data && data.sheets && data.sheets.length > 0) {
                currentSheets = data.sheets;
                // Set workbook and launch Luckysheet
                luckysheet.create({
                    container: 'luckysheet',
                    data: data.sheets.map(s => {
                        let parsed = JSON.parse(s.jsonData);
                        return {
                            ...parsed,
                            name: s.name
                        };
                    }),
                    title: data.name,
                    showinfobar: false,
                    hook: {
                        workbookCreateAfter: function () {
                            setTimeout(() => {
                                const sheets = luckysheet.getAllSheets();
                                sheets.forEach((sheet, index) => {
                                    luckysheet.setSheetActive(index);
                                    const formulaCells = sheet.celldata?.filter(cell => cell.v?.f);
                                    formulaCells?.forEach(cell => {
                                        const { r, c, v } = cell;
                                        luckysheet.setCellValue(r, c, {
                                            f: v.f
                                        }, index);
                                    });
                                });
                                luckysheet.setSheetActive(0);
                                luckysheet.refreshFormula();
                            }, 100);
                        }
                    }
                });

                $('#saveContainer').show();
            } else {
                alert("No sheets found for this plan.");
            }
        },
        error: function (err) {
            console.error("Error loading plan", err);
            alert("Failed to load plan data.");
        }
    });
    $('#btnSaveSheets').on('click', function () {
        $('#loader').show();
        const allSheets = luckysheet.getAllSheets(); // current sheet order
        const updatedSheets = [];
        for (let i = 0; i < allSheets.length; i++) {
            const sheetData = luckysheet.getSheet(i);
            const jsonData = JSON.stringify(sheetData);
            // Match original metadata by sheet name
            const meta = currentSheets.find(m => m.name === sheetData.name);
            if (!meta) continue;
            updatedSheets.push({
                id: meta.planSheetId,
                planId: planId,
                lobId: meta.lobId,
                name: sheetData.name,
                type: meta.type,
                jsonData: jsonData
            });
        }
        $.ajax({
            url: '/Plan/saveSheets',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(updatedSheets),
            success: function () {
                $('#loader').hide();
                alert("Plan saved successfully!");
            },
            error: function () {
                $('#loader').hide();
                alert("Error saving plan.");
            }
        });
    });
});