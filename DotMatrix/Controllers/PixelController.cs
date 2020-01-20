using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using DotMatrix.Common.Pixel;
using DotMatrix.Common.Users;
using DotMatrix.Enums;

using Microsoft.AspNet.Identity;

namespace DotMatrix.Controllers
{
	public class PixelController : Controller
	{
		public IUserReader UserReader { get; set; }
		public IPixelWriter PixelWriter { get; set; }
		public IPixelReader PixelReader { get; set; }

		public async Task<ActionResult> Index()
		{
			var user = await UserReader.GetUser(User.Identity.GetUserId<int>());
			return View(new PixelViewlModel
			{
				Points = user?.Points ?? 0
			});
		}


		[HttpPost]
		[Authorize]
		public async Task<ActionResult> AddPixel(AddPixelRequest model)
		{
			if (!ModelState.IsValid)
				return Json(new { Success = false });

			var userId = User.Identity.GetUserId<int>();
			var result = await PixelWriter.AddPixel(userId, model);
			if (!result.Success)
				return Json(new { Success = false, Message = result.Message });

			await PixelHub.UpdateFrontEndPixel(new PixelHubAddPixelRequest
			{
				X = model.X,
				Y = model.Y,
				Color = model.Color,
				Type = PixelType.User,

				PixelId = result.PixelId,
				UserId = result.UserId,
				UserName = result.UserName,
				UserPoints = result.UserPoints,
				TeamId = result.TeamId,
				TeamName = result.TeamName,
				NewPoints = result.NewPoints,

				IsApi = false
			});

			var clickResult = await PixelWriter.AddClick(userId, new AddClickRequest(PixelClickType.AddPixel, model.X, model.Y));
			if (!clickResult.Success)
				return Json(new { Success = false, Message = clickResult.Message });

			if (clickResult.IsPrizeWinner)
			{
				await PixelHub.UpdateFrontEndPrize(new PixelHubAddPrizeRequest
				{
					X = model.X,
					Y = model.Y,
					UserId = userId,
					PrizeId = clickResult.PrizeId,
					PrizeName = clickResult.PrizeName,
					PrizeUser = User.Identity.Name,
					PrizePoints = clickResult.PrizePoints,
					PrizeDescription = clickResult.PrizeDescription,
					UserPoints = clickResult.UserPoints
				});
			}



			return Json(new { Success = true });
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> GetPixel(GetPixelRequest model)
		{
			if (!ModelState.IsValid)
				return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

			var userId = User.Identity.GetUserId<int>();
			var clickResult = await PixelWriter.AddClick(userId, new AddClickRequest(PixelClickType.GetPixel, model.X, model.Y));
			if (!clickResult.Success)
				return Json(new { Success = false, Message = clickResult.Message });

			var result = await PixelReader.GetPixel(model.X, model.Y);
			if (result == null)
				return Json(new { Success = false, Message = "Pixel not found." }, JsonRequestBehavior.AllowGet);

			return Json(new
			{
				Success = true,
				Data = result
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public async Task<ActionResult> GetPixels()
		{
			return Json(await PixelReader.GetPixels(), JsonRequestBehavior.AllowGet);
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