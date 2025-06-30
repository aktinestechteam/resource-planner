using Microsoft.AspNetCore.Mvc;

namespace W3CRM.Controllers
{
	public class W3CRMController : Controller
	{
		#region Extra Pages
		public IActionResult Profile()
		{
			return View();
		}

		public IActionResult BlogCategory() 
		{ 
			return View(); 
		}

		public IActionResult ContentAdd()
		{
			return View();
		}

		public IActionResult ForgotPassword()
		{
			return View();
		}
		#endregion

		#region Default 
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Index2()
		{
			return View();
		}

		public IActionResult Employee()
		{
			return View();
		}

		public IActionResult CoreHR()
		{
			return View();

		}
		public IActionResult Finance()
		{
			return View();
		}
		public IActionResult Task()
		{
			return View();
		}
		public IActionResult TaskSummary()
		{
			return View();
		}
		public IActionResult Performance()
		{
			return View();
		}

		public IActionResult Projects()
		{
			return View();
		}

		public IActionResult Reports()
		{
			return View();
		}

		public IActionResult ManageClients()
		{
			return View();
		}
		public IActionResult SVGIcons()
		{
			return View();
		}
		#endregion

		#region Fetured

		#region Applications 
		public IActionResult Chat()
		{
			return View();
		}

		#region Users Manager 
		public IActionResult Users() 
		{ 
			return View(); 
		}

		public IActionResult AddUser() 
		{ 
			return View(); 
		}

		public IActionResult RoleListing() 
		{ 
			return View(); 
		}

		public IActionResult AddRoles() 
		{ 
			return View(); 
		}

		public IActionResult Profile1() 
		{ 
			return View(); 
		}

		public IActionResult Profile2()
		{
			return View();
		}

		public IActionResult EditProfile()
		{
			return View();
		}

		public IActionResult PostDetails()
		{
			return View();
		}
		#endregion

		#region Customer Manager 
		public IActionResult Customer()
		{
			return View();
		}

		public IActionResult CustomerProfile()
		{
			return View();
		}
		#endregion

		public IActionResult Contacts()
		{
			return View();
		}

		#region Email 
		public IActionResult Compose()
		{
			return View();
		}

		public IActionResult Inbox()
		{
			return View();
		}

		public IActionResult Read()
		{
			return View();
		}
		#endregion

		public IActionResult Calendar()
		{
			return View();
		}

		#region Shop 
		public IActionResult ProductGrid()
		{
			return View();
		}

		public IActionResult ProductList()
		{
			return View();
		}

		public IActionResult ProductDetails()
		{
			return View();
		}

		public IActionResult Order()
		{
			return View();
		}

		public IActionResult Checkout()
		{
			return View();
		}

		public IActionResult Invoice()
		{
			return View();
		}

		public IActionResult EcomCustomers()
		{
			return View();
		}
		#endregion

		#endregion

		#region AIKit 
		public IActionResult AutoWriter()
		{
			return View();
		}

		public IActionResult Scheduler()
		{
			return View();
		}

		public IActionResult Repurpose()
		{
			return View();
		}

		public IActionResult RSS()
		{
			return View();
		}

		public IActionResult Chatbot()
		{
			return View();
		}

		public IActionResult FineTuneModels()
		{
			return View();
		}

		public IActionResult AIMenuPrompts()
		{
			return View();
		}

		public IActionResult Settings()
		{
			return View();
		}

		public IActionResult ExportImportSettings()
		{
			return View();
		}
		#endregion

		#region CMS

		public IActionResult Content()
		{
			return View();
		}

		public IActionResult Menus()
		{
			return View();
		}

		public IActionResult EmailTemplate()
		{
			return View();
		}

		public IActionResult Blog()
		{
			return View();
		}

		#endregion

		#region Charts

		public IActionResult Flot()
		{
			return View();
		}

		public IActionResult Morris()
		{
			return View();
		}

		public IActionResult Chartjs()
		{
			return View();
		}

		public IActionResult Chartist()
		{
			return View();
		}

		public IActionResult Sparkline()
		{
			return View();
		}

		public IActionResult Peity()
		{
			return View();
		}
		#endregion

		#region Bootstrap

		public IActionResult Accordion()
		{
			return View();
		}
		public IActionResult Alert()
		{
			return View();
		}
		public IActionResult Badge()
		{
			return View();
		}
		public IActionResult Button()
		{
			return View();
		}
		public IActionResult Modal()
		{
			return View();
		}
		public IActionResult ButtonGroup()
		{
			return View();
		}
		public IActionResult ListGroup()
		{
			return View();
		}
		public IActionResult Cards()
		{
			return View();
		}
		public IActionResult Carousel()
		{
			return View();
		}
		public IActionResult Dropdown()
		{
			return View();
		}
		public IActionResult Popover()
		{
			return View();
		}

		public IActionResult Progressbar()
		{
			return View();
		}

		public IActionResult Tab()
		{
			return View();
		}

		public IActionResult Typography()
		{
			return View();
		}

		public IActionResult Pagination()
		{
			return View();
		}

		public IActionResult Grid()
		{
			return View();
		}
		#endregion

		#region Plugins

		public IActionResult Select2()
		{
			return View();
		}

		public IActionResult Nestable()
		{
			return View();
		}

		public IActionResult NouiSlider()
		{
			return View();
		}

		public IActionResult SweetAlert()
		{
			return View();
		}

		public IActionResult Toastr()
		{
			return View();
		}

		public IActionResult JqvMap()
		{
			return View();
		}

		public IActionResult LightGallery()
		{
			return View();
		}
		#endregion

		public IActionResult Widget()
		{
			return View();
		}

		#region Forms

		public IActionResult FormElements()
		{
			return View();
		}

		public IActionResult Wizard()
		{
			return View();
		}

		public IActionResult CkEditor()
		{
			return View();
		}

		public IActionResult Pickers()
		{
			return View();
		}

		public IActionResult FormValidate()
		{
			return View();
		}
		#endregion

		#region Table

		public IActionResult Bootstrap()
		{
			return View();
		}

		public IActionResult Datatable()
		{
			return View();
		}
		#endregion

		#region Pages
		public IActionResult Login()
		{
			return View();
		}

		public IActionResult Register()
		{
			return View();
		}

		#region Error
		public IActionResult Error400()
		{
			return View();
		}
		public IActionResult Error403()
		{
			return View();
		}
		public IActionResult Error404()
		{
			return View();
		}
		public IActionResult Error500()
		{
			return View();
		}
		public IActionResult Error503()
		{
			return View();
		}
		#endregion

		public IActionResult LockScreen()
		{
			return View();
		}

		public IActionResult EmptyPage()
		{
			return View();
		}
		#endregion

		#endregion
	}
}
