using System;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Sutherland.WFMResourcePlanner.Entities.LuckySheet;

namespace Sutherland.WFMResourcePlanner.Utilities
{
    public static class WeekHelper
    {
        public static List<CustomWeekConfig> BuildCustomWeek(int trainingWk, int nestingWk, int learningWk)
        {
            // Generate week labels for each type
            var trainingWks = Enumerable.Range(1, trainingWk).Select(i => $"Training_WK{i}").ToList();
            var nestingWks = Enumerable.Range(1, nestingWk).Select(i => $"Nesting_WK{i}").ToList();
            var learningWks = Enumerable.Range(1, learningWk).Select(i => $"Learning_Curve-WK{i}").ToList();

            // Define custom week configs
            var customWeekConfigs = new List<CustomWeekConfig>
            {
                new CustomWeekConfig
                {
                    Header = "Fullfillment Rate%",
                    IsTrainingWk = true,
                    IsNestingWk = true,
                    IsLearningWk = true,
                    Weeks = new List<string>()
                },
                new CustomWeekConfig
                {
                    Header = "Production hours - FTE",
                    IsTrainingWk = false,
                    IsNestingWk = true,
                    IsLearningWk = true,
                    Weeks = new List<string>()
                }
            };
            // Assign relevant weeks based on flags
            foreach (var config in customWeekConfigs)
            {
                if (config.IsTrainingWk)
                    config.Weeks.AddRange(trainingWks);
                if (config.IsNestingWk)
                    config.Weeks.AddRange(nestingWks);
                if (config.IsLearningWk)
                    config.Weeks.AddRange(learningWks);
            }

            return customWeekConfigs;
        }
        public static List<WeeklyStaffingRowConfig> GetWeeklyStaffingConfigForLobType(string lobType)
        {
            return lobType switch
            {
                "FTE" => new List<WeeklyStaffingRowConfig>
            {
                new() { MetricName = "Required HC", SourceRowHeader = "Required HC" },
                new() { MetricName = "Available FTE", SourceRowHeader = "Available FTE" },
                new() { MetricName = "Delta in HC", FormulaTemplate = "={0}-{1}" }
            },
                "Transaction" => new List<WeeklyStaffingRowConfig>
            {
                new() { MetricName = "Required HC", SourceRowHeader = "Required HC" },
                new() { MetricName = "Available FTE", SourceRowHeader = "Available FTE" },
                new() { MetricName = "Delta in HC", FormulaTemplate = "={0}-{1}" }
            },
                _ => throw new NotSupportedException($"LOB Type {lobType} not supported")
            };
        }

    }

}