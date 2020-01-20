using System.Threading.Tasks;
using System.Web.Mvc;

using DotMatrix.Common.Payment;

using Microsoft.AspNet.Identity;

namespace DotMatrix.Controllers
{
	public class PaymentController : Controller
	{
		public IPaymentReader PaymentReader { get; set; }
		public IPaymentWriter PaymentWriter { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var paymentMethods = await PaymentReader.GetMethods();
			return View(new PaymentModel
			{
				Methods = paymentMethods
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Method(int id)
		{
			var userId = User.Identity.GetUserId<int>();
			var paymentMethod = await PaymentReader.GetMethod(userId, id);
			if (paymentMethod != null)
				return View(paymentMethod);

			if (await PaymentWriter.CreateMethod(userId, id))
			{
				paymentMethod = await PaymentReader.GetMethod(userId, id);
				if (paymentMethod != null)
					return View(paymentMethod);
			}

			return RedirectToAction("Error");
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> History()
		{
			var userId = User.Identity.GetUserId<int>();
			var receipts = await PaymentReader.GetReceipts(userId);
			return View(new PaymentHistoryModel
			{
				Receipts = receipts
			});
		}

		[HttpGet]
		[Authorize]
		public Task<ActionResult> Error()
		{
			return Task.FromResult<ActionResult>(View());
		}
	}
}