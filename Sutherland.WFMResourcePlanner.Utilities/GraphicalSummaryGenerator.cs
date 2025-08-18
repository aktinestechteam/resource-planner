using Newtonsoft.Json.Linq;
using Sutherland.WFMResourcePlanner.Entities.LuckySheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Utilities
{
    public static class GraphicalSummaryGenerator
    {
        public static JObject GenerateGraphicalSummarySheet(
            List<JObject> lobSheets,
            List<GraphicalSummaryGroupConfig> groupConfigs,
            DateTime planFrom,
            DateTime planTo,
            DayOfWeek weekStart = DayOfWeek.Monday)
        {
            var sheet = new JObject
            {
                ["name"] = "Graphical Summary",
                ["celldata"] = new JArray(),
                ["calcChain"] = new JArray(),
                ["index"] = 1000,
                ["status"] = 1
            };

            var celldata = (JArray)sheet["celldata"];
            var calcChain = (JArray)sheet["calcChain"];

            var weeks = GetWeeklyColumns(planFrom, planTo, weekStart);
            int currentRow = 0;

            foreach (var group in groupConfigs)
            {
                // 1. Add header row for this group
                AddCell(sheet, currentRow, 0, CreateTextCell("LOB"));
                AddCell(sheet, currentRow, 1, CreateTextCell("Header"));

                for (int w = 0; w < weeks.Count; w++)
                {
                    AddCell(sheet, currentRow, w + 2, CreateTextCell(weeks[w].ToString("dd-MMM-yy")));
                }

                currentRow++;

                // 2. For each LOB in lobSheets
                foreach (var lobSheet in lobSheets)
                {
                    string lobName = lobSheet["name"]?.ToString() ?? "LOB";
                    var lobCelldata = (JArray)lobSheet["celldata"];

                    foreach (var header in group.Headers)
                    {
                        AddCell(sheet, currentRow, 0, CreateTextCell(lobName));
                        AddCell(sheet, currentRow, 1, CreateTextCell(header));

                        var rowRef = FindRowIndexByHeader(lobCelldata, header);
                        if (rowRef >= 0)
                        {
                            for (int w = 0; w < weeks.Count; w++)
                            {
                                string colLetter = ColumnIndexToLetter(w + 2);
                                string formula = $"='{lobName}'!{colLetter}{rowRef + 1}";
                                AddCell(sheet, currentRow, w + 2,
                                    CreateFormulaCell(formula, sheet, currentRow, w + 2, group.IncludeInCalcChain));
                            }
                        }

                        currentRow++;
                    }
                }

                // 3. Blank row separator
                currentRow++;
            }

            RebuildDataArray(sheet);
            return sheet;
        }

        private static JObject CreateTextCell(string value)
        {
            return new JObject
            {
                ["v"] = value,
                ["m"] = value,
                ["ct"] = new JObject { ["fa"] = "General", ["t"] = "g" }
            };
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
            ((JArray)sheet["celldata"]).Add(new JObject
            {
                ["r"] = row,
                ["c"] = col,
                ["v"] = value
            });
        }

        private static int FindRowIndexByHeader(JArray celldata, string header)
        {
            foreach (var cell in celldata.Children<JObject>().Where(c => (int)c["c"] == 0))
            {
                string val = cell["v"]?["v"]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(val) && val.Equals(header, StringComparison.OrdinalIgnoreCase))
                {
                    return (int)cell["r"];
                }
            }
            return -1;
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

        private static string ColumnIndexToLetter(int colIndex)
        {
            int temp = colIndex;
            string letter = string.Empty;
            while (temp >= 0)
            {
                letter = (char)('A' + temp % 26) + letter;
                temp = temp / 26 - 1;
            }
            return letter;
        }

        private static void RebuildDataArray(JObject sheet)
        {
            var celldata = (JArray)sheet["celldata"];
            if (!celldata.Any())
            {
                sheet["data"] = new JArray();
                return;
            }

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
