namespace Sutherland.WFMResourcePlanner.Entities
{
    public class LOB : BaseEntity
    {
        public int LOBId { get; set; }
        public int PlanId { get; set; }
        public string Name { get; set; }
        public string BillingModel { get; set; }
        public string ProjectId { get; set; }
        public int TrainingWk { get; set; }
        public int NestingWk { get; set; }
        public int LearningWk { get; set; }
        public string Geo { get; set; }
        public string Site { get; set; }

    }
}

