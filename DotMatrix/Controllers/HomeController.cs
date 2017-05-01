using DotMatrix.Common.DataContext;
using DotMatrix.Common.Pixel;
using DotMatrix.Common.Users;
using DotMatrix.Common.Wallet;
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

		public async Task<ActionResult> Index()
		{
			return View();
		}
	}
}