using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Entities.LuckySheet
{
    public class CustomWeekConfig
    {
        public string Header { get; set; }
        public bool IsTrainingWk { get; set; }
        public bool IsNestingWk { get; set; }
        public bool IsLearningWk { get; set; }
        public List<string> Weeks { get; set; }

    }
}
