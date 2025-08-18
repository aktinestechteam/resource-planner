using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Entities.LuckySheet
{
    public class WeeklyStaffingRowConfig
    {
        public string MetricName { get; set; }          
        public string SourceRowHeader { get; set; }     
        public string FormulaTemplate { get; set; } = null; 
        public bool IncludeInCalcChain { get; set; } = true; 
    }
}
