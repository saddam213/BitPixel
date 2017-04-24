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
		public IPixelReader PixelReader { get; set; }
		public async Task<ActionResult> Index()
		{
			return View();
		}


		[HttpPost]
		[System.Web.Mvc.Authorize]
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
				await notificationHub.Clients.All.SendPixelData(model.X, model.Y, model.Color);
			}
			return Json(new { Success = true });
		}

		[HttpGet]
		public async Task<ActionResult> GetPixels()
		{
			return Json(await PixelReader.GetPixels(),  JsonRequestBehavior.AllowGet);
		}
	}
}