using System.Web.Mvc;

using BitPixel.ActionResults;
using BitPixel.Datatables.Models;

namespace BitPixel.Controllers
{
	public class BaseController : Controller
	{
		protected string GetLocalReturnUrl(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				if (returnUrl == "/")
					return string.Empty;

				return returnUrl;
			}
			return string.Empty;
		}

		protected DataTablesResult DataTable(DataTablesResponseData dataTablesResponse)
		{
			return new DataTablesResult(dataTablesResponse);
		}

		protected CloseModalResult CloseModal()
		{
			return new CloseModalResult();
		}

		protected CloseModalResult CloseModal(object data)
		{
			return new CloseModalResult(data);
		}

		protected CloseModalResult CloseModalSuccess(string message = null)
		{
			return new CloseModalResult(new { Success = true, Message = message });
		}

		protected CloseModalResult CloseModalError(string message = null)
		{
			return new CloseModalResult(new { Success = false, Message = message });
		}

		protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new JsonResult()
			{
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding,
				JsonRequestBehavior = behavior,
				MaxJsonLength = int.MaxValue
			};
		}
	}
}