using System.Threading.Tasks;
using System.Web.Mvc;

using DotMatrix.Common.Payment;

using Microsoft.AspNet.Identity;

namespace DotMatrix.Controllers
{
	public class PaymentController : BaseController
	{
		public IPaymentReader PaymentReader { get; set; }
		public IPaymentWriter PaymentWriter { get; set; }

		[HttpGet]
		[Authorize]
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
		public async Task<ActionResult> MethodModal(int id)
		{
			var userId = User.Identity.GetUserId<int>();
			var paymentMethod = await PaymentReader.GetMethod(userId, id);
			if (paymentMethod != null)
				return View(paymentMethod);

			var result = await PaymentWriter.CreateMethod(userId, id);
			if (result.Success)
			{
				paymentMethod = await PaymentReader.GetMethod(userId, id);
				if (paymentMethod != null)
					return View(paymentMethod);
			}

			return View("ErrorModal");
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
		public Task<ActionResult> ErrorModal()
		{
			return Task.FromResult<ActionResult>(View());
		}
	}
}