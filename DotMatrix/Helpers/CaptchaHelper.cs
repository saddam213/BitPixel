using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using hbehr.recaptcha;

namespace DotMatrix.Helpers
{
	public class CaptchaHelper
	{
		private static readonly string PublicKey = ConfigurationManager.AppSettings["RecaptchaPublicKey"];
		private static readonly string SecretKey = ConfigurationManager.AppSettings["RecaptchaPrivateKey"];

		public static bool Validate()
		{
			try
			{
#if !DEBUG
				return ReCaptcha.ValidateCaptcha(HttpContext.Current.Request.Params["g-recaptcha-response"]);
#else
				return true;
#endif
			}
			catch { }
			return false;
		}

		public static void Configure()
		{
			ReCaptcha.Configure(PublicKey, SecretKey);
		}
	}
}