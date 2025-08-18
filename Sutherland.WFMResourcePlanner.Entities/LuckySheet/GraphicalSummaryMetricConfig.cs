using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Entities.LuckySheet
{
    public class GraphicalSummaryMetricConfig
    {
        public string MetricName { get; set; }             // e.g. "FTE Delta"
        public string SourceRowHeader { get; set; }         // Row header in LOB sheet
        public bool IncludeInCalcChain { get; set; } = true;
    }
}
