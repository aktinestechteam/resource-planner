using Newtonsoft.Json.Linq;
using Sutherland.WFMResourcePlanner.Entities.LuckySheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Utilities
{
    public class WeeklyStaffingGeneratorStd
    {
        public static JObject GenerateWeeklyStaffingSheet(
            List<JObject> lobSheets,
            List<WeeklyStaffingRowConfig> config,
            DateTime planFrom,
            DateTime planTo,
            DayOfWeek weekStart = DayOfWeek.Monday)
        {
            var staffingSheet = new JObject
            {
                ["name"] = "Weekly Staffing",
                ["celldata"] = new JArray(),
                ["calcChain"] = new JArray(),
                ["index"] = 999,
                ["status"] = 1
            };

            var celldata = (JArray)staffingSheet["celldata"];
            var calcChain = (JArray)staffingSheet["calcChain"];
            int currentRow = 0;
            var weeks = GetWeeklyColumns(planFrom, planTo, weekStart);

            // Add Month/WeekStart/WeekEnd headers (top 3 rows)
            for (int i = 0; i < weeks.Count; i++)
            {
                string col = ColumnIndexToLetter(i + 2); // Start from column C
                AddCell(staffingSheet, currentRow, i + 2, new JObject { ["v"] = weeks[i].ToString("MMM-yy"), ["m"] = weeks[i].ToString("MMM-yy") });
                AddCell(staffingSheet, currentRow + 1, i + 2, new JObject { ["v"] = weeks[i].ToString("dd-MMM-yy"), ["m"] = weeks[i].ToString("dd-MMM-yy"), ["bl"] = 1 });
                AddCell(staffingSheet, currentRow + 2, i + 2, new JObject { ["v"] = weeks[i].AddDays(6).ToString("dd-MMM-yy"), ["m"] = weeks[i].AddDays(6).ToString("dd-MMM-yy") });
            }
            currentRow = 3;

            foreach (var lobSheet in lobSheets)
            {
                string lobName = lobSheet["name"]?.ToString() ?? "LOB";
                AddCell(staffingSheet, currentRow++, 0, new JObject { ["v"] = lobName, ["m"] = lobName, ["bl"] = 1 });

                var lobCelldata = (JArray)lobSheet["celldata"];

                var rowRefDict = FindMetricRowIndices(lobCelldata, config.Select(c => c.SourceRowHeader));
                var metricRowMap = new Dictionary<string, int>();

                // Step 1: Add base metric rows
                foreach (var rowConfig in config.Where(c => c.FormulaTemplate == null))
                {
                    metricRowMap[rowConfig.MetricName] = currentRow;
                    AddCell(staffingSheet, currentRow, 0, new JObject { ["v"] = rowConfig.MetricName, ["m"] = rowConfig.MetricName });

                    foreach (var (week, weekIdx) in weeks.Select((w, i) => (w, i)))
                    {
                        string colLetter = ColumnIndexToLetter(weekIdx + 2);
                        if (rowRefDict.TryGetValue(rowConfig.SourceRowHeader, out int rowIndex))
                        {
                            string formula = $"='{lobName}'!{colLetter}{rowIndex + 1}";
                            AddCell(staffingSheet, currentRow, weekIdx + 2, CreateFormulaCell(formula, staffingSheet, currentRow, weekIdx + 2, rowConfig.IncludeInCalcChain));
                        }
                    }

                    currentRow++;
                }

                // Step 2: Add derived metrics (custom formula templates)
                foreach (var rowConfig in config.Where(c => c.FormulaTemplate != null))
                {
                    int formulaRow = currentRow;
                    AddCell(staffingSheet, formulaRow, 0, new JObject { ["v"] = rowConfig.MetricName, ["m"] = rowConfig.MetricName });

                    foreach (var (week, weekIdx) in weeks.Select((w, i) => (w, i)))
                    {
                        string colLetter = ColumnIndexToLetter(weekIdx + 2);
                        var refs = metricRowMap.Values.Select(r => $"{colLetter}{r + 1}").ToArray();
                        string resolvedFormula = string.Format(rowConfig.FormulaTemplate, refs);

                        AddCell(staffingSheet, formulaRow, weekIdx + 2, CreateFormulaCell(resolvedFormula, staffingSheet, formulaRow, weekIdx + 2, rowConfig.IncludeInCalcChain));
                    }

                    currentRow++;
                }
            }

            RebuildDataArray(staffingSheet);
            return staffingSheet;
        }

        private static JObject CreateFormulaCell(string formula, JObject sheet, int row, int col, bool addToCalcChain)
        {
            var cell = new JObject
            {
                ["f"] = formula,
                ["v"] = null,
                ["m"] = null,
                ["ct"] = new JObject { ["fa"] = "General", ["t"] = "n" }
            };

            if (addToCalcChain)
            {
                ((JArray)sheet["calcChain"]).Add(new JObject
                {
                    ["index"] = sheet["index"],
                    ["r"] = row,
                    ["c"] = col,
                    ["func"] = new JArray(null, null, formula)
                });
            }

            return cell;
        }

        private static void AddCell(JObject sheet, int row, int col, JObject value)
        {
            ((JArray)sheet["celldata"]).Add(new JObject { ["r"] = row, ["c"] = col, ["v"] = value });
        }

        private static List<DateTime> GetWeeklyColumns(DateTime from, DateTime to, DayOfWeek weekStart)
        {
            List<DateTime> weeks = new();
            DateTime current = from;
            while (current <= to)
            {
                DateTime start = current.AddDays(-(7 + (current.DayOfWeek - weekStart)) % 7);
                if (!weeks.Contains(start)) weeks.Add(start);
                current = current.AddDays(7);
            }
            return weeks.Distinct().OrderBy(d => d).ToList();
        }

        private static Dictionary<string, int> FindMetricRowIndices(JArray celldata, IEnumerable<string> headers)
        {
            var result = new Dictionary<string, int>();
            foreach (var cell in celldata.Children<JObject>().Where(c => (int)c["c"] == 0))
            {
                string val = cell["v"]?["v"]?.ToString()?.Trim();
                int row = (int)cell["r"];
                if (!string.IsNullOrEmpty(val) && headers.Contains(val) && !result.ContainsKey(val))
                {
                    result[val] = row;
                }
            }
            return result;
        }

        private static string ColumnIndexToLetter(int colIndex)
        {
            int temp = colIndex; string letter = string.Empty;
            while (temp >= 0) { letter = (char)('A' + temp % 26) + letter; temp = temp / 26 - 1; }
            return letter;
        }

        private static void RebuildDataArray(JObject sheet)
        {
            var celldata = (JArray)sheet["celldata"];
            int maxRow = celldata.Max(c => (int)c["r"]);
            int maxCol = celldata.Max(c => (int)c["c"]);

            var data = new JArray();
            for (int i = 0; i <= maxRow; i++)
            {
                var row = new JArray();
                for (int j = 0; j <= maxCol; j++) row.Add(null);
                data.Add(row);
            }

            foreach (var cell in celldata)
            {
                int r = (int)cell["r"];
                int c = (int)cell["c"];
                data[r][c] = cell["v"];
            }

            sheet["data"] = data;
        }
    }
}
