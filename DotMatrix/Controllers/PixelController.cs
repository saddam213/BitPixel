﻿using DotMatrix.Common.DataContext;
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
	public class PixelController : Controller
	{
		public IUserReader UserReader { get; set; }
		public IPixelWriter PixelWriter { get; set; }
		public IPixelReader PixelReader { get; set; }

		public async Task<ActionResult> Index()
		{
			var user = await UserReader.GetUser(User.Identity.GetUserId());
			return View(new PixelViewlModel
			{
				Balance = user?.Balance ?? 0
			});
		}


		[HttpPost]
		[System.Web.Mvc.Authorize]
		public async Task<ActionResult> AddPixel(PixelModel model)
		{
			if (!ModelState.IsValid)
				return Json(new { Success = false });

			var result = await PixelWriter.AddOrUpdate(User.Identity.GetUserId(), model);
			if (!result.Success)
				return Json(result);


			var notificationHub = GlobalHost.ConnectionManager.GetHubContext<PixelHub>();
			if (notificationHub != null)
			{
				await notificationHub.Clients.All.SendPixelData(model.X, model.Y, model.Color);
				await notificationHub.Clients.User(User.Identity.GetUserId()).SendBalanceData(result.Balance);
			}
			return Json(result);
		}

		[HttpGet]
		public async Task<ActionResult> GetPixels()
		{
			return Json(await PixelReader.GetPixels(),  JsonRequestBehavior.AllowGet);
		}

		protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new JsonResult()
			{
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding,
				JsonRequestBehavior = behavior,
				MaxJsonLength = Int32.MaxValue
			};
		}
	}
}