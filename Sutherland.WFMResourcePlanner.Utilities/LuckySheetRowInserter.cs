using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Utilities
{
    public static class LuckySheetRowInserter
    {
        public static void InsertRowsBelowHeaderWithFormula(JObject sheet, string headerText, List<string> labelsToInsert)
        {
            var celldata = (JArray)sheet["celldata"];
            var calcChain = (JArray)sheet["calcChain"] ?? new JArray();

            if (labelsToInsert == null || labelsToInsert.Count == 0) return;

            // 1. Find header row index in column A
            int headerRow = celldata
                .Children<JObject>()
                .Where(c => (int)c["c"] == 0 && c["v"]?["v"]?.ToString().Trim() == headerText.Trim())
                .Select(c => (int)c["r"])
                .FirstOrDefault();

            if (!celldata.Any(c => (int)c["r"] == headerRow && (int)c["c"] == 0 && c["v"]?["v"]?.ToString().Trim() == headerText.Trim()))
                throw new Exception($"Header '{headerText}' not found in column A.");

            int insertAt = headerRow + 1;
            int rowsInserted = labelsToInsert.Count;

            // 2. Shift rows below insert point
            foreach (var cell in celldata.Where(c => (int)c["r"] >= insertAt).ToList())
            {
                cell["r"] = (int)cell["r"] + rowsInserted;
            }

            // 3. Insert label rows
            for (int i = 0; i < labelsToInsert.Count; i++)
            {
                celldata.Add(new JObject
                {
                    ["r"] = insertAt + i,
                    ["c"] = 0,
                    ["v"] = new JObject
                    {
                        ["v"] = labelsToInsert[i],
                        ["m"] = labelsToInsert[i],
                        ["ct"] = new JObject { ["fa"] = "General", ["t"] = "s" },
                        ["bl"] = 1
                    }
                });
            }

            // 4. Update formulas in celldata
            foreach (var cell in celldata)
            {
                var vToken = cell["v"];
                if (vToken?["f"]?.Type == JTokenType.String)
                {
                    string oldFormula = vToken["f"].ToString();
                    string newFormula = AdjustFormulaRowReferences(oldFormula, insertAt, rowsInserted);
                    if (newFormula != oldFormula)
                    {
                        vToken["f"] = newFormula; // ✅ Only formula changes
                    }
                }
            }

            // 5. Update formulas in calcChain
            foreach (var entry in calcChain)
            {
                var funcArray = entry["func"] as JArray;
                if (funcArray != null && funcArray.Count > 2 && funcArray[2]?.Type == JTokenType.String)
                {
                    string oldFormula = funcArray[2].ToString();
                    string newFormula = AdjustFormulaRowReferences(oldFormula, insertAt, rowsInserted);
                    if (newFormula != oldFormula)
                    {
                        funcArray[2] = newFormula;
                    }
                }
            }

            // 6. Rebuild data array
            RebuildDataArray(sheet);
        }

        private static string AdjustFormulaRowReferences_(string formula, int insertAtRow, int rowsInserted)
        {
            return Regex.Replace(formula, @"(\$?[A-Z]+)(\$?)(\d+)", match =>
            {
                string col = match.Groups[1].Value;
                string abs = match.Groups[2].Value;
                int row = int.Parse(match.Groups[3].Value);

                // ✅ Shift only if relative and row >= insert point
                if (string.IsNullOrEmpty(abs) && row >= insertAtRow)
                {
                    row += rowsInserted;
                }

                return $"{col}{abs}{row}";
            });
        }
        private static string AdjustFormulaRowReferences(string formula, int insertAtRow, int rowsInserted)
        {
            return Regex.Replace(formula, @"(\$?[A-Z]+)(\$?)(\d+)", match =>
            {
                string col = match.Groups[1].Value;
                string abs = match.Groups[2].Value;
                int row = int.Parse(match.Groups[3].Value);

                // ✅ Shift both absolute & relative if row is >= insert point
                if (row >= insertAtRow)
                {
                    row += rowsInserted;
                }

                return $"{col}{abs}{row}";
            });
        }


        private static void RebuildDataArray(JObject sheet)
        {
            var celldata = (JArray)sheet["celldata"];
            if (celldata == null || !celldata.HasValues) { sheet["data"] = new JArray(); return; }

            int maxRow = celldata.Max(c => (int)c["r"]), maxCol = celldata.Max(c => (int)c["c"]);
            var data = new JArray();
            for (int i = 0; i <= maxRow; i++)
            {
                var row = new JArray(); for (int j = 0; j <= maxCol; j++) row.Add(null);
                data.Add(row);
            }
            foreach (var cell in celldata.Children<JObject>())
            {
                int r = (int)cell["r"];
                int c = (int)cell["c"];
                data[r][c] = cell["v"];
            }
            sheet["data"] = data;
        }
    }

}
