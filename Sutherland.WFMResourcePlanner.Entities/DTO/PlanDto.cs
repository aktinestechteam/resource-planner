using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Entities.DTO
{
    public class PlanDto
    {
        public int PlanId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Optional fields (add if needed)
        public string? Vertical { get; set; }
        public string? Account { get; set; }
        public string? Geo { get; set; }
        public string? Site { get; set; }
        public string? BusinessUnit { get; set; }
        public string? WeekStart { get; set; }
        public DateTime? PlanFromDate { get; set; }
        public DateTime? PlanToDate { get; set; }
    }

}
