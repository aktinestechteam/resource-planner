using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Entities.DTO
{
	public class PlanSheetCellDto
	{
		public int PlanSheetId { get; set; }
		public int RowIndex { get; set; }
		public int ColumnIndex { get; set; }
		public string ColumnLetter { get; set; }
		public string Header { get; set; } // Optional, can be mapped externally
		public string Metric { get; set; }
		public string ValueText { get; set; }
		public double? ValueNumeric { get; set; }
		public string Formula { get; set; }
		public bool IsFormula { get; set; }
		public string SheetName { get; set; }
		public DateTime? WeekStartDate { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}
