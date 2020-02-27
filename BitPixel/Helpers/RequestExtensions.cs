using System.Collections.Specialized;
using System.Net.Http;
using System.Web;

using Microsoft.Owin;

namespace BitPixel.Helpers
{
	public static class RequestExtensions
	{
		private const string MS_HttpContext = "MS_HttpContext";
		private const string MS_OwinContext = "MS_OwinContext";
		private const string Header_X_Forwarded_For = "X-FORWARDED-FOR";
		private const string Header_CloudFlare_IP = "CF-CONNECTING-IP";
		private const string Header_Cloudfare_Country = "CF-IPCOUNTRY";

		public static string GetIPAddress(this HttpRequestMessage request)
		{
			if (request.Properties.ContainsKey(MS_HttpContext))
			{
				if (request.Properties[MS_HttpContext] is HttpContextBase context)
				{
					return context.GetIPAddress();
				}
			}
			if (request.Properties.ContainsKey(MS_OwinContext))
			{
				if (request.Properties[MS_OwinContext] is OwinContext context)
				{
					return context.GetIPAddress();
				}
			}
			return null;
		}


		public static string GetIPAddress(this HttpContextBase context)
		{
			var headerIPAddress = GetIPAddressFromHeader(context.Request.Headers);
			if (!string.IsNullOrEmpty(headerIPAddress))
				return headerIPAddress;

			return context.Request.UserHostAddress;
		}

		public static string GetIPAddress(this IOwinContext context)
		{
			var headerIPAddress = GetIPAddressFromHeader(context.Request.Headers);
			if (!string.IsNullOrEmpty(headerIPAddress))
				return headerIPAddress;

			return context.Request.RemoteIpAddress;
		}

		public static string GetIPAddress(this HttpRequestBase request)
		{
			var headerIPAddress = GetIPAddressFromHeader(request.Headers);
			if (!string.IsNullOrEmpty(headerIPAddress))
				return headerIPAddress;

			return request.UserHostAddress;
		}


		public static string GetIPAddressFromHeader(IHeaderDictionary headers)
		{
			var headerValue = headers[Header_CloudFlare_IP];
			if (!string.IsNullOrEmpty(headerValue))
				headerValue = headers[Header_X_Forwarded_For];

			return GetIPAddressFromHeader(headerValue);
		}

		private static string GetIPAddressFromHeader(NameValueCollection headers)
		{
			var headerValue = headers[Header_CloudFlare_IP];
			if (string.IsNullOrEmpty(headerValue))
				headerValue = headers[Header_X_Forwarded_For];

			return GetIPAddressFromHeader(headerValue);
		}

		private static string GetIPAddressFromHeader(string headerValue)
		{
			if (!string.IsNullOrEmpty(headerValue))
			{
				string[] addresses = headerValue.Split(',');
				if (addresses.Length != 0)
				{
					return addresses[0];
				}
			}
			return string.Empty;
		}

		//public static string GetDevice(this HttpRequestBase request)
		//{
		//	var uaParser = Parser.GetDefault();
		//	var clientInfo = uaParser.Parse(request.UserAgent);
		//	return $"{clientInfo.UA} ({clientInfo.OS})";
		//}

		public static string GetLocation(this HttpRequestBase request)
		{
			var header = request.Headers[Header_Cloudfare_Country];
			if (string.IsNullOrEmpty(header))
				return "-";

			return header;
		}
	}
}