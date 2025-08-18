using Newtonsoft.Json.Linq;
using Sutherland.WFMResourcePlanner.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Utilities
{
	public static class LuckySheetParser
	{
		public static List<PlanSheetCellDto> ParseSheetData(JObject sheetJson, int planSheetId)
		{
			var result = new List<PlanSheetCellDto>();
			string sheetName = sheetJson["name"]?.ToString();
			var celldata = sheetJson["celldata"] as JArray;
			if (celldata == null) return result;

			foreach (var cell in celldata.OfType<JObject>())
			{
				int row = cell["r"]?.Value<int>() ?? -1;
				int col = cell["c"]?.Value<int>() ?? -1;
				var vToken = cell["v"] as JObject;
				if (row < 0 || col < 0 || vToken == null) continue;

				string valueText = vToken["m"]?.ToString();
				string formula = vToken["f"]?.ToString();
				double? valueNumeric = null;
				if (double.TryParse(vToken["v"]?.ToString(), out double parsed))
					valueNumeric = parsed;

				var columnLetter = ColumnIndexToLetter(col);
				string metric = null;
				if (col != 0)
				{
					// Try get metric from column A (col = 0) of the same row
					var headerCell = celldata.FirstOrDefault(c => (int)c["r"] == row && (int)c["c"] == 0) as JObject;
					metric = headerCell?["v"]?["v"]?.ToString();
				}

				DateTime? weekStartDate = null;
				var weekHeaderCell = celldata.FirstOrDefault(c => (int)c["r"] == 1 && (int)c["c"] == col) as JObject;
				if (weekHeaderCell != null && DateTime.TryParse(weekHeaderCell["v"]?["v"]?.ToString(), out DateTime parsedWeek))
				{
					weekStartDate = parsedWeek;
				}

				result.Add(new PlanSheetCellDto
				{
					PlanSheetId = planSheetId,
					RowIndex = row,
					ColumnIndex = col,
					ColumnLetter = columnLetter,
					ValueText = valueText,
					ValueNumeric = valueNumeric,
					Formula = formula,
					IsFormula = !string.IsNullOrEmpty(formula),
					Metric = metric,
					SheetName = sheetName,
					WeekStartDate = weekStartDate,
					LastUpdated = DateTime.UtcNow
				});
			}

			return result;
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
	}
}