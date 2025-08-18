namespace Sutherland.WFMResourcePlanner.Entities
{
    public class PlanSheet : BaseEntity
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public int? LOBId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string JsonData { get; set; }
    }
}
