using DotMatrix.Common.DataContext;
using DotMatrix.Common.Pixel;
using DotMatrix.Common.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DotMatrix.Controllers
{
	public class HomeController : Controller
	{
		public IPixelWriter PixelWriter { get; set; }
		public async Task<ActionResult> Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> AddPixel(PixelModel model)
		{
			if (!ModelState.IsValid)
				return Json(new { Success = false });

			var result = await PixelWriter.AddOrUpdate(User.Identity.GetUserId(), model);
			if (!result)
				return Json(new { Success = false });


			var notificationHub = GlobalHost.ConnectionManager.GetHubContext<PixelHub>();
			if (notificationHub != null)
			{
				await notificationHub.Clients.All.SendPixelData(model.X, model.Y, model.R, model.G, model.B);
			}
			return Json(new { Success = true });
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}