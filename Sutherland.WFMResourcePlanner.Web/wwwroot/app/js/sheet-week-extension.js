// sheet-week-extender.js
// luckysheet-week-extension.js
var SheetWeekExtender = (function () {

    function addNextWeeksToAllSheets(weeksToAdd = 5, matchTemplate = true) {
        const allSheets = luckysheet.getluckysheetfile();
        let updated = 0;

        allSheets.forEach(sheet => {
            if (!sheet || !sheet.data || !Array.isArray(sheet.data)) return;
            if (matchTemplate && !isWeeklyTemplate(sheet.data)) return;

            // normalize shape first
            normalizeRectangular(sheet);

            // find last Week-ST col
            const weekSTRow = sheet.data[1];
            const lastColIndex = findLastWeekStartCol(weekSTRow);
            if (lastColIndex < 0) return;

            const lastWeekSTDate = parseExcelDate(weekSTRow[lastColIndex]?.v ?? weekSTRow[lastColIndex]);
            if (!lastWeekSTDate) return;

            // compute new target column count & ensure rectangular
            const newLastColIndex = lastColIndex + weeksToAdd;
            const targetCols = Math.max(newLastColIndex + 1, getMaxCols(sheet.data));
            ensureCols(sheet.data, targetCols);

            // add weeks and drag formulas (respect $ locks)
            for (let i = 1; i <= weeksToAdd; i++) {
                const newCol = lastColIndex + i;
                const st = new Date(lastWeekSTDate); st.setDate(st.getDate() + 7 * i);
                const ed = new Date(st); ed.setDate(st.getDate() + 6); // week end

                sheet.data[0][newCol] = cellV(st.toLocaleString('en-GB', { month: 'short', year: '2-digit' }));
                sheet.data[1][newCol] = cellV(formatDate(st));
                sheet.data[2][newCol] = cellV(formatDate(ed));

                for (let r = 4; r < sheet.data.length; r++) {
                    const src = sheet.data[r][lastColIndex];
                    if (src && src.f) {
                        // If dense data has top-level f (rare), prefer src.f
                        const shifted = shiftFormulaColumns(src.f, lastColIndex, newCol);
                        sheet.data[r][newCol] = cellF(shifted);
                    } else if (src && src.v && src.v.f) {
                        // If dense cell stores formula inside v.f
                        const shifted = shiftFormulaColumns(src.v.f, lastColIndex, newCol);
                        sheet.data[r][newCol] = cellF(shifted);
                    } else {
                        // keep explicit null so every row has same length (rectangular)
                        if (sheet.data[r][newCol] == null) sheet.data[r][newCol] = null;
                    }
                }
            }

            // rebuild sparse celldata from dense data
            sheet.celldata = luckysheet.transToCellData(sheet.data);

            // rebuild calcChain from celldata using exact Luckysheet func shape
            const cc = [];
            sheet.celldata.forEach(cell => {
                // formula may be at cell.f or inside cell.v.f depending on representation
                const f = (cell && cell.f) || (cell && cell.v && cell.v.f);
                if (f) {
                    cc.push({
                        r: cell.r,
                        c: cell.c,
                        index: sheet.index,
                        func: [true, 0, f]   // exact shape Luckysheet uses
                    });
                }
            });
            // sort by row then column to mirror Luckysheet ordering
            cc.sort((a, b) => (a.r - b.r) || (a.c - b.c));
            sheet.calcChain = cc;

            // update visible column/row arrays and content width
            sheet.visibledatacolumn = luckysheet.buildGridData("col", sheet.data[0].length);
            sheet.visibledatarow = luckysheet.buildGridData("row", sheet.data.length);
            sheet.ch_width = sheet.visibledatacolumn[sheet.visibledatacolumn.length - 1] || sheet.ch_width;

            updated++;
        });

        // Refresh UI & trigger formula execution (if function exists)
        luckysheet.refresh();

        

        // execFunctionGroup is used by Luckysheet to evaluate the calcChain
        if (typeof luckysheet.execFunctionGroup === "function") {
            try { luckysheet.execFunctionGroup(); } catch (e) { console.warn("execFunctionGroup failed:", e); }
        } else if (typeof luckysheet.runCalc === "function") {
            try { luckysheet.runCalc(); } catch (e) { console.warn("runCalc failed:", e); }
        }

        setTimeout(() => {
            const currentSheets = luckysheet.getluckysheetfile();
            const currentConfig = luckysheet.config || {};

            luckysheet.create({
                container: 'luckysheet',       // your container div id
                showinfobar: false,            // match your settings
                data: currentSheets,
                config: currentConfig,
                title: luckysheet.title || '', // preserve existing title if set
                lang: 'en',                    // adjust if using another language
                plugins: luckysheet.plugins || []
            });
            luckysheet.refreshFormula();
        }, 50);
        console.log(`✅ Weeks added to ${updated} sheet(s).`);
    }

    /* ---------------- Helpers ---------------- */

    function isWeeklyTemplate(data) {
        return (
            data[0] && data[0][1] && String(data[0][1].v || data[0][1]).toLowerCase().includes("month") &&
            data[1] && data[1][1] && String(data[1][1].v || data[1][1]).toLowerCase().includes("week-st") &&
            data[2] && data[2][1] && String(data[2][1].v || data[2][1]).toLowerCase().includes("week-ed")
        );
    }

    function findLastWeekStartCol(row) {
        if (!row || !Array.isArray(row)) return -1;
        // prefer right-to-left parseable date
        for (let c = row.length - 1; c >= 0; c--) {
            const val = row[c] && (row[c].v ?? row[c]);
            if (val != null && parseExcelDate(val)) return c;
        }
        return -1;
    }

    function ensureCols(data, targetCols) {
        for (let r = 0; r < data.length; r++) {
            if (!Array.isArray(data[r])) data[r] = [];
            if (data[r].length < targetCols) {
                const need = targetCols - data[r].length;
                for (let i = 0; i < need; i++) data[r].push(null);
            } else if (data[r].length > targetCols) {
                data[r].length = targetCols;
            }
        }
    }

    function normalizeRectangular(sheet) {
        const data = sheet.data;
        let maxCols = 0;
        for (let r = 0; r < data.length; r++) if (Array.isArray(data[r]) && data[r].length > maxCols) maxCols = data[r].length;
        if (maxCols === 0) maxCols = 1;
        // ensure each row array exists and has same length
        for (let r = 0; r < data.length; r++) {
            if (!Array.isArray(data[r])) data[r] = [];
            ensureCols(data, maxCols);
        }
    }

    function getMaxCols(data) {
        let m = 0;
        for (let r = 0; r < data.length; r++) if (Array.isArray(data[r]) && data[r].length > m) m = data[r].length;
        return m || 1;
    }

    const cellV = v => ({ v });
    const cellF = f => ({ f });

    function shiftFormulaColumns(formula, oldCol, newCol) {
        const shift = newCol - oldCol;
        return String(formula).replace(/(\$?)([A-Z]+)(\$?)(\d+)/g, (m, cLock, cLetters, rLock, rNum) => {
            let colNum = colLettersToNumber(cLetters);
            if (!cLock) colNum += shift;
            const newLetters = numberToColLetters(colNum);
            return (cLock ? "$" : "") + newLetters + (rLock ? "$" : "") + rNum;
        });
    }

    function colLettersToNumber(letters) {
        let num = 0;
        for (let i = 0; i < letters.length; i++) num = num * 26 + (letters.charCodeAt(i) - 64);
        return num;
    }
    function numberToColLetters(num) {
        let s = '';
        while (num > 0) {
            const mod = (num - 1) % 26;
            s = String.fromCharCode(65 + mod) + s;
            num = (num - 1 - mod) / 26;
        }
        return s;
    }

    function parseExcelDate(val) {
        if (val == null) return null;
        // if val is an object like {v: "28-Jul-25"} handle that
        const maybe = (typeof val === "object" && val !== null && val.v) ? val.v : val;
        if (typeof maybe === "string") {
            const p = Date.parse(maybe);
            if (!isNaN(p)) return new Date(p);
        }
        if (!isNaN(maybe)) {
            // excel serial -> JS date
            return new Date(Math.round((maybe - 25569) * 86400 * 1000));
        }
        return null;
    }

    function formatDate(d) {
        return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
    }

    return { addNextWeeksToAllSheets };

})();

