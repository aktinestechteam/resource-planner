using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Entities.DTO
{
    public class SaveSheetDto
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public int? LobId { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string JsonData { get; set; } = "";
    }

}
