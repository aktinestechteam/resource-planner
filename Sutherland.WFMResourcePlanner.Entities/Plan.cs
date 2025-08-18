namespace Sutherland.WFMResourcePlanner.Entities
{
    public class Plan : BaseEntity
    {
        public int PlanId { get; set; }
        public string Name { get; set; }
        public string Vertical { get; set; }
        public string Account { get; set; }
        public bool SOTracker { get; set; }
        public bool AssumptionSheet { get; set; }
        public string WeekStart { get; set; }
        public DateTime PlanFrom { get; set; }
        public DateTime PlanTo { get; set; }
        public List<LOB> LOBs { get; set; }
        public int? CopiedFrom { get; set; }
    }

   

    

}
