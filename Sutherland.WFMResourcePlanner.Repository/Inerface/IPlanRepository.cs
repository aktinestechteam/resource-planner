using Sutherland.WFMResourcePlanner.Entities;
using Sutherland.WFMResourcePlanner.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Repository.Inerface
{
    public interface IPlanRepository
    {
        Task<int> CreateAsync(Plan plan);
        Task<PlanWithSheetsDto?> GetPlanWithSheetsAsync(int planId);
        Task SaveSheetsAsync(List<SaveSheetDto> sheets);
        Task<IEnumerable<Plan>> GetAllPlansAsync();
        Task<int> CopyPlanAsync(CopyPlanDto dto);

	}
}
