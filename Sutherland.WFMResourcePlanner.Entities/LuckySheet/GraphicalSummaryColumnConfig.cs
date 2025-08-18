using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Entities.LuckySheet
{

    public class GraphicalSummaryGroupConfig
    {
        public string GroupName { get; set; } // e.g. "FTE Delta", "FTE Required & Available"
        public List<string> Headers { get; set; } = new();
        public bool IncludeInCalcChain { get; set; } = true;
    }
}
