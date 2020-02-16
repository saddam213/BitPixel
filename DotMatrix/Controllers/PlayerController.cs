using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DotMatrix.Common.Award;
using DotMatrix.Common.Users;

namespace DotMatrix.Controllers
{
	public class PlayerController : Controller
	{
		public IUserReader UserReader { get; set; }
		public IAwardReader AwardReader { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index(string name)
		{
			var user = await UserReader.GetUserProfile(name);
			var awards = await AwardReader.GetUserAwardList(user.Id);
			return View(new UserProfileViewModel
			{
				Id = user.Id,
				UserName = user.UserName,
				Clicks = user.Clicks,
				Pixels = user.Pixels,
				Awards = user.Awards,
				AwardList = awards
			});
		}
	}
}