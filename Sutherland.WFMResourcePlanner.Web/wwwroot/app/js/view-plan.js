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

    document.addEventListener("contextmenu", function (e) {
        const tab = e.target.closest(".luckysheet-sheets-item");
        if (!tab) return;

        const sheetName = tab.querySelector(".luckysheet-sheets-item-name").textContent;
        const sheet = currentSheets.find(m => m.name === sheetName);
            if (sheet && sheet.type && sheet.type !== "Custom") {
                $('#luckysheetsheetconfigdelete').css('display', 'none');
                $('#luckysheetsheetconfigrename').css('display', 'none');

            } else {
                $('#luckysheetsheetconfigdelete').css('display', 'block');
                $('#luckysheetsheetconfigrename').css('display', 'none');
            }
    });

    $('#btnSaveSheets').on('click', function () {
        $('#loader-overlay').css('display','flex'); // Show the Bootstrap spinner
        $(this).prop('disabled', true); // Disable save button during saving
        const allSheets = luckysheet.getAllSheets(); // current sheet order
        const updatedSheets = [];
        for (let i = 0; i < allSheets.length; i++) {
            const sheetData = allSheets[i];
            const jsonData = JSON.stringify(sheetData);
            // Match original metadata by sheet name
            const meta = currentSheets.find(m => m.name === sheetData.name);
            if (!meta) {
                updatedSheets.push({
                    id: 0,
                    planId: planId,
                    lobId:0,
                    name: sheetData.name,
                    type: "Custom",
                    jsonData: jsonData
                });

            } else { 
                updatedSheets.push({
                    id: meta.planSheetId,
                    planId: planId,
                    lobId: meta.lobId,
                    name: sheetData.name,
                    type: meta.type,
                    jsonData: jsonData
                });
            }
           
        }
        $.ajax({
            url: '/Plan/saveSheets',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(updatedSheets),
            success: function () {
                $('#loader-overlay').css('display', 'none');
                $('#btnSaveSheets').prop('disabled', false); // Re-enable save button
                showMessageBox("Plan saved successfully!"); // Use custom message box
            },
            error: function () {
                $('#loader-overlay').css('display', 'none');
                $('#btnSaveSheets').prop('disabled', false); // Re-enable save button
                showMessageBox("Error saving plan."); // Use custom message box
            }
        });
    });

   
    function showMessageBox(message) {
        const messageBox = document.createElement('div');
        messageBox.className = 'custom-message-box';
        messageBox.innerHTML = `
                <div class="custom-message-box-content">
                    <p class="fs-5 fw-semibold mb-4">${message}</p>
                    <button class="btn btn-primary" onclick="this.parentNode.parentNode.remove()">Close</button>
                </div>
            `;
        document.body.appendChild(messageBox);
    }
    $('#btnViewVersions').on('click', function () {
        window.location.href = "/Plan/PlanSummary";
    });
    $('#btnDashboard').on('click', function () {
        window.location.href = "/w3crm/Index";
    });
});