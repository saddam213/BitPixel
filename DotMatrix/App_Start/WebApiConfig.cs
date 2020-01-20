using System.Net.Http.Headers;
using System.Web.Http;

namespace DotMatrix
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

			config.Routes.MapHttpRoute(name: "AddPixel", routeTemplate: "Api/AddPixel", defaults: new { controller = "ApiPrivate", action = "AddPixel" });
			config.Routes.MapHttpRoute(name: "GetPixel", routeTemplate: "Api/GetPixel", defaults: new { controller = "ApiPrivate", action = "GetPixel" });
		}
	}
}
