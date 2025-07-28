using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Utilities
{
    public static class WeeklyStaffingSummaryBuilder
    {
        public static string BuildSummaryJson(List<string> weekHeaders, List<string> lobSheetNames)
        {
            var celldata = new JArray();
            var conditionformat = GetConditionFormatString();
            int currentRow = 0;

            // Metric title
            celldata.Add(CreateCell(currentRow, 0, "Metric", bold: true));

            // Week headers
            for (int i = 0; i < weekHeaders.Count; i++)
            {
                celldata.Add(CreateCell(currentRow, i + 1, weekHeaders[i], bold: true));
            }

            currentRow++; // Move to total rows

            // FTE Required (Total)
            celldata.Add(CreateCell(currentRow, 0, "FTE Required"));
            for (int i = 0; i < weekHeaders.Count; i++)
            {
                string colLetter = GetExcelColumnName(i + 2);
                string formula = $"=SUM({string.Join(",", lobSheetNames.Select(name => $"{name}!{colLetter}2"))})";
                celldata.Add(CreateFormulaCell(currentRow, i + 1, formula));
            }

            currentRow++;

            // FTE Available (Total)
            celldata.Add(CreateCell(currentRow, 0, "FTE Available"));
            for (int i = 0; i < weekHeaders.Count; i++)
            {
                string colLetter = GetExcelColumnName(i + 2);
                string formula = $"=SUM({string.Join(",", lobSheetNames.Select(name => $"{name}!{colLetter}3"))})";
                celldata.Add(CreateFormulaCell(currentRow, i + 1, formula));
            }

            currentRow++;

            // FTE Gap (Total)
            celldata.Add(CreateCell(currentRow, 0, "FTE Gap"));
            for (int i = 0; i < weekHeaders.Count; i++)
            {
                string colLetter = GetExcelColumnName(i + 2);
                celldata.Add(CreateFormulaCell(currentRow, i + 1, $"={colLetter}{currentRow-1} - {colLetter}{currentRow}"));
            }

            currentRow++;

            // Spacer row (optional)
            currentRow++;

            // Per-LOB Breakdown
            foreach (var lob in lobSheetNames)
            {
                // 1. LOB Header
                celldata.Add(CreateCell(currentRow, 0, lob, bold: true));
                currentRow++;

                int requiredRow = currentRow;
                int availableRow = currentRow + 1;
                int gapRow = currentRow + 2;

                // 2. FTE Required
                celldata.Add(CreateCell(requiredRow, 0, "FTE Required"));
                for (int i = 0; i < weekHeaders.Count; i++)
                {
                    string colLetter = GetExcelColumnName(i + 2); // +2 because A = 1, B = 2
                    celldata.Add(CreateFormulaCell(requiredRow, i + 1, $"={lob}!{colLetter}2"));
                }

                // 3. FTE Available
                celldata.Add(CreateCell(availableRow, 0, "FTE Available"));
                for (int i = 0; i < weekHeaders.Count; i++)
                {
                    string colLetter = GetExcelColumnName(i + 2);
                    celldata.Add(CreateFormulaCell(availableRow, i + 1, $"={lob}!{colLetter}3"));
                }

                // 4. FTE Gap = Required - Available
                celldata.Add(CreateCell(gapRow, 0, "FTE Gap"));
                for (int i = 0; i < weekHeaders.Count; i++)
                {
                    string colLetter = GetExcelColumnName(i + 2);
                    string gapFormula = $"={colLetter}{requiredRow + 1} - {colLetter}{availableRow + 1}";
                    celldata.Add(CreateFormulaCell(gapRow, i + 1, gapFormula));
                }

                currentRow = gapRow + 1;
            }

            var sheet = new JObject
            {
                ["name"] = "Weekly Staffing Summary",
                ["celldata"] = celldata,
                ["config"] = new JObject
                {
                    ["protected"] = true
                },
                ["index"] = 0,
                ["order"] = 0,
                ["status"] = 1,
                ["luckysheet_conditionformat_save"] = conditionformat
            };

            return sheet.ToString(Formatting.None);
        }

        private static JObject CreateCell(int row, int col, string value, bool bold = false)
        {
            var cell = new JObject
            {
                ["r"] = row,
                ["c"] = col,
                ["v"] = new JObject
                {
                    ["v"] = value
                }
            };
            if (bold) cell["v"]["bl"] = 1;
            return cell;
        }

        private static JObject CreateFormulaCell(int row, int col, string formula)
        {
            return new JObject
            {
                ["r"] = row,
                ["c"] = col,
                ["v"] = new JObject
                {
                    ["f"] = formula
                }
            };
        }

        public static string GetExcelColumnName(int columnNumber)
        {
            var columnName = string.Empty;
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }

        private static JArray GetConditionFormatString()
        {
            string jsonString = @"
        [
            {
                ""type"": ""default"",
                ""cellrange"": [
                    {
                        ""row"": [
                            3,
                            3
                        ],
                        ""column"": [
                            1,
                            5
                        ],
                        ""row_focus"": 3,
                        ""column_focus"": 1,
                        ""moveXY"": {
                            ""x"": 3,
                            ""y"": 1
                        },
                        ""left"": 74,
                        ""width"": 73,
                        ""top"": 60,
                        ""height"": 19,
                        ""left_move"": 74,
                        ""width_move"": 369,
                        ""top_move"": 60,
                        ""height_move"": 19
                    }
                ],
                ""format"": {
                    ""textColor"": ""#9c0006"",
                    ""cellColor"": ""#ffc7ce""
                },
                ""conditionName"": ""lessThan"",
                ""conditionRange"": [],
                ""conditionValue"": [
                    ""0""
                ]
            },
            {
                ""type"": ""default"",
                ""cellrange"": [
                    {
                        ""left"": 0,
                        ""width"": 73,
                        ""top"": 60,
                        ""height"": 19,
                        ""left_move"": 0,
                        ""width_move"": 4661,
                        ""top_move"": 60,
                        ""height_move"": 19,
                        ""row"": [
                            3,
                            3
                        ],
                        ""column"": [
                            0,
                            62
                        ],
                        ""row_focus"": 3,
                        ""column_focus"": 0,
                        ""row_select"": true
                    }
                ],
                ""format"": {
                    ""textColor"": ""#9c0006"",
                    ""cellColor"": ""#ffc7ce""
                },
                ""conditionName"": ""lessThan"",
                ""conditionRange"": [],
                ""conditionValue"": [
                    ""0""
                ]
            }
        ]";
            return  JArray.Parse(jsonString);
        }

    }


}
