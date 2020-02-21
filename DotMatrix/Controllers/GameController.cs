using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotMatrix.Cache.Common;
using DotMatrix.Common.Game;
using DotMatrix.Common.Pixel;
using DotMatrix.Common.Users;
using DotMatrix.Enums;
using DotMatrix.Helpers;
using Microsoft.AspNet.Identity;

namespace DotMatrix.Controllers
{
	public class GameController : BaseController
	{
		public IGameReader GameReader { get; set; }
		public IUserReader UserReader { get; set; }
		public IPixelReader PixelReader { get; set; }

		public async Task<ActionResult> Index(int gameId)
		{
			var game = await GameReader.GetGame(gameId);
			var user = await UserReader.GetUser(User.Identity.GetUserId<int>());
			return View(new PixelViewlModel
			{
				Game = game,
				Points = user?.Points ?? 0
			});
		}

		[HttpGet]
		public async Task<ActionResult> GetPixels(int gameId)
		{
			return Json(await PixelReader.GetPixels(gameId), JsonRequestBehavior.AllowGet);
		}
	}
}