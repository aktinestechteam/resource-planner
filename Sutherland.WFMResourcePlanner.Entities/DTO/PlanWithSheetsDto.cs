using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Entities.DTO
{
    public class PlanWithSheetsDto
    {
        public int PlanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PlanSheetDto> Sheets { get; set; } = new();
    }

}
