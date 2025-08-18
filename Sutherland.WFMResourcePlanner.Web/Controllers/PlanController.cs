using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sutherland.WFMResourcePlanner.Entities;
using Sutherland.WFMResourcePlanner.Entities.DTO;
using Sutherland.WFMResourcePlanner.Repository.Implementation;
using Sutherland.WFMResourcePlanner.Repository.Inerface;

namespace Sutherland.WFMResourcePlanner.Web.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanRepository _planRepository;
        public PlanController(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Plan plan)
        {
            if (plan == null || string.IsNullOrWhiteSpace(plan.Name))
                return BadRequest("Invalid plan data.");

            try
            {
                //plan = new Plan
                //{
                //    Name = "Test Plan July 2025 v6",
                //    Vertical = "Banking",
                //    Account = "ABC Corp",
                //    Geo = "NA",
                //    Site = "Dallas",
                //    BusinessUnit = "Customer Support",
                //    WeekStart = "Monday",
                //    PlanFrom = new DateTime(2026, 7, 1),
                //    PlanTo = new DateTime(2026, 10, 31),
                //    SOTracker = true,
                //    AssumptionSheet = true,
                //    LOBs = new List<LOB>
                //    {
                //        new LOB
                //        {
                //            Name = "R1 English",
                //            BillingModel = "FTE",
                //            ProjectId = "PRJ-ENG-001",
                //            TrainingWk = 3,
                //            NestingWk = 2,
                //            LearningWk = 0,

                //        },
                       
                //    },
                //};
                var planId = await _planRepository.CreateAsync(plan);
                return Ok(new { PlanId = planId, Message = "Plan created successfully." });

                //new LOB
                //{
                //    Name = "R2 Spanish",
                //    BillingModel = "Transaction",
                //    ProjectId = "PRJ-SPA-002",
                //    TrainingWk = 2,
                //    NestingWk = 3,
                //    LearningWk = 0,
                //}
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating plan: {ex.Message}");
            }
        }

        public IActionResult ViewPlan(int planId, string accountName, string planName)
        {
            ViewBag.PlanId = planId;
			ViewBag.AccountName = accountName;
			ViewBag.PlanName = planName;
			return View();
        }

        public async Task<IActionResult> GetPlanById(int id)
        {
            var plan = await _planRepository.GetPlanWithSheetsAsync(id);
            if (plan == null) return NotFound();

            return Ok(plan);
        }
        public async Task<IActionResult> SaveSheets([FromBody] List<SaveSheetDto> sheets)
        {
            if (sheets == null || !sheets.Any())
                return BadRequest("Invalid sheet data.");

            await _planRepository.SaveSheetsAsync(sheets);
            return Ok();
        }

		public IActionResult PlanSummary()
		{
			return View();
		}

		public async Task<IActionResult> GetAllPlans()
		{
			var plans = await _planRepository.GetAllPlansAsync();
			return Ok(plans);
		}

		[HttpPost]
		public async Task<IActionResult> CopyPlan([FromBody] CopyPlanDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.NewPlanTitle) || dto.SourcePlanId <= 0)
				return BadRequest("Invalid input.");

			var newPlanId = await _planRepository.CopyPlanAsync(dto);
			return Ok(new { newPlanId });
		}

	}


}
