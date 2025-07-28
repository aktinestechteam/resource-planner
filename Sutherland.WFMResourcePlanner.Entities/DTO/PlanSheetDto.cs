using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Entities.DTO
{
    public class PlanSheetDto
    {
        public int PlanSheetId { get; set; }
        public int PlanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;  // "FTE", "Transaction", "Summary", etc.
        public string JsonData { get; set; } = "{}";      // Luckysheet JSON
    }

}
