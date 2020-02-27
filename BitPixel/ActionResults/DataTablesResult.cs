using BitPixel.Datatables.Models;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;

namespace BitPixel.ActionResults
{
	public class DataTablesResult : ActionResult
	{
		public DataTablesResponseData Data { get; set; }

		public DataTablesResult(DataTablesResponseData data)
		{
			Data = data;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			HttpResponseBase response = context.HttpContext.Response;
			response.Write(JsonConvert.SerializeObject(Data));
		}
	}
}