using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Entities.DTO
{
	public class CopyPlanDto
	{
		public int SourcePlanId { get; set; }
		public string NewPlanTitle { get; set; }
	}
}
