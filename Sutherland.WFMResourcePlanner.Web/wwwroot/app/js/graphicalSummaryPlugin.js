const GraphicalSummaryPlugin = {
    generateGraphicalSummary: function (luckysheet, lobSheets, groupConfigs, weekRange) {
        const newSheet = this.generateGraphicalSummarySheetFrontEnd(lobSheets, groupConfigs, weekRange);

        // Check if "Graphical Summary" sheet exists
        const existingIndex = lobSheets.findIndex(s => s.name === "Graphical Summary");
        if (existingIndex >= 0) {
            lobSheets[existingIndex] = newSheet;
        } else {
            lobSheets.push(newSheet);
        }

        // Refresh UI and select summary sheet
        luckysheet.create({
            container: 'luckysheet',
            data: lobSheets,
            // your initial config here (toolbar, hooks, etc)
        });

        luckysheet.refreshFormula();
    },

    generateGraphicalSummarySheetFrontEnd: function (lobSheets, groupConfigs, weekRange) {
        // Parse weekRange, e.g. "B2:AA2"
        const [startCell, endCell] = weekRange.split(':');
        const startCol = this.columnLetterToIndex(startCell.match(/[A-Z]+/)[0]);
        const endCol = this.columnLetterToIndex(endCell.match(/[A-Z]+/)[0]);
        const headerRow = parseInt(startCell.match(/\d+/)[0]) - 1; // zero-based

        const sheet = {
            name: "Graphical Summary",
            celldata: [],
            calcChain: [],
            index: 1000,
            status: 1
        };

        let currentRow = 0;

        // Extract week headers from first LOB sheet for date strings
        const firstLobCelldata = lobSheets[0].celldata || [];
        const weeks = [];
        for (let col = startCol; col <= endCol; col++) {
            const cell = this.findCell(firstLobCelldata, headerRow, col);
            const val = cell?.v?.v || "";
            weeks.push(val);
        }

        groupConfigs.forEach(group => {
            // Header row for group
            this.addCell(sheet, currentRow, 0, this.createTextCell("LOB"));
            this.addCell(sheet, currentRow, 1, this.createTextCell("Header"));
            weeks.forEach((w, i) => this.addCell(sheet, currentRow, i + 2, this.createTextCell(w)));
            currentRow++;

            // Rows per LOB sheet and header
            lobSheets.forEach(lobSheet => {
                const lobName = lobSheet.name || "LOB";
                const lobCelldata = lobSheet.celldata || [];

                group.Headers.forEach(header => {
                    this.addCell(sheet, currentRow, 0, this.createTextCell(lobName));
                    this.addCell(sheet, currentRow, 1, this.createTextCell(header));

                    const rowIndex = this.findRowIndexByHeader(lobCelldata, header);
                    if (rowIndex >= 0) {
                        for (let i = 0; i < weeks.length; i++) {
                            const colLetter = this.columnIndexToLetter(startCol + i);
                            const formula = `='${lobName}'!${colLetter}${rowIndex + 1}`;
                            this.addCell(sheet, currentRow, i + 2, this.createFormulaCell(formula, sheet, currentRow, i + 2, true));
                        }
                    }
                    currentRow++;
                });
            });

            currentRow++; // blank row separator
        });

        this.rebuildDataArray(sheet);
        return sheet;
    },

    // --- Helper functions ---

    findCell: function (celldata, r, c) {
        return celldata.find(cell => cell.r === r && cell.c === c);
    },

    findRowIndexByHeader: function (celldata, header) {
        for (const cell of celldata) {
            if (cell.c === 0) {
                const val = cell.v?.v?.toString()?.trim();
                if (val && val.toLowerCase() === header.toLowerCase()) return cell.r;
            }
        }
        return -1;
    },

    createTextCell: function (value) {
        return { v: value, m: value, ct: { fa: "General", t: "g" } };
    },

    createFormulaCell: function (formula, sheet, row, col, addToCalcChain) {
        const cell = { f: formula, v: null, m: null, ct: { fa: "General", t: "n" } };
        if (addToCalcChain) {
            sheet.calcChain.push({ index: sheet.index, r: row, c: col, func: [null, null, formula] });
        }
        return cell;
    },

    addCell: function (sheet, row, col, value) {
        sheet.celldata.push({ r: row, c: col, v: value });
    },

    columnIndexToLetter: function (index) {
        let letter = "";
        let temp = index;
        while (temp >= 0) {
            letter = String.fromCharCode((temp % 26) + 65) + letter;
            temp = Math.floor(temp / 26) - 1;
        }
        return letter;
    },

    columnLetterToIndex: function (letter) {
        let result = 0;
        for (let i = 0; i < letter.length; i++) {
            result *= 26;
            result += letter.charCodeAt(i) - 64; // 'A' => 65
        }
        return result - 1; // zero-based
    },

    rebuildDataArray: function (sheet) {
        if (!sheet.celldata.length) {
            sheet.data = [];
            return;
        }
        const maxRow = Math.max(...sheet.celldata.map(c => c.r));
        const maxCol = Math.max(...sheet.celldata.map(c => c.c));
        const data = [];
        for (let r = 0; r <= maxRow; r++) {
            const row = new Array(maxCol + 1).fill(null);
            data.push(row);
        }
        sheet.celldata.forEach(cell => {
            data[cell.r][cell.c] = cell.v;
        });
        sheet.data = data;
    }
};
