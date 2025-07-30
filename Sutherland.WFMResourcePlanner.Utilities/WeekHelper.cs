using System;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace Sutherland.WFMResourcePlanner.Utilities
{
    public static class WeekHelper
    {

        //public static string InjectWeekHeaders(string templateJson, List<string> weekHeaders)
        //{
        //    for (int i = 0; i < weekHeaders.Count; i++)
        //    {
        //        templateJson = templateJson.Replace($"{{WEEK_{i + 1}}}", weekHeaders[i]);
        //    }

        //    return templateJson;
        //}

        public static string InjectWeekHeaders(string jsonTemplate, List<string> weekHeaders)
        {
            JObject sheet = JObject.Parse(jsonTemplate);
            JArray celldata = (JArray)sheet["celldata"];

            // Inject week header row
            for (int i = 0; i < weekHeaders.Count; i++)
            {
                celldata.Add(new JObject
                {
                    ["r"] = 0,
                    ["c"] = i + 1,
                    ["v"] = new JObject
                    {
                        ["v"] = weekHeaders[i],
                        ["bl"] = 1
                    }
                });
            }

            //// Inject formulas in "FTE Gap" row (row index 3)
            //for (int i = 0; i < weekHeaders.Count; i++)
            //{
            //    string colLetter = GetExcelColumnName(i + 2); // B, C, D, etc.

            //    celldata.Add(new JObject
            //    {
            //        ["r"] = 3,
            //        ["c"] = i + 1,
            //        ["v"] = new JObject
            //        {
            //            ["f"] = $"={colLetter}2-{colLetter}3"
            //        }
            //    });
            //}

            return sheet.ToString(); // Final modified JSON as string
        }



        public static List<string> GenerateWeekHeaders(DateTime start, DateTime end, DayOfWeek weekStart)
        {
       

            var result = new List<string>();
            var current = start;

            // Align to first week start
            while (current.DayOfWeek != weekStart)
                current = current.AddDays(-1);

            while (current <= end)
            {
                result.Add(current.ToString("dd-MMM-yy"));
                current = current.AddDays(7);
            }

            return result;
        }

        public static string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
    }

}