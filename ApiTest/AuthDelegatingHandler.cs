using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiTest
{
	public class AuthDelegatingHandler : DelegatingHandler
	{
		private string _apiKey;
		private string _apiSecret;

		public AuthDelegatingHandler(string key, string secret)
		{
			_apiKey = key;
			_apiSecret = secret;
		}

		protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			string requestContentBase64String = string.Empty;
			string requestUri = System.Web.HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());

			//Checking if the request contains body, usually will be null wiht HTTP GET
			if (request.Content != null)
			{
				var content = await request.Content.ReadAsByteArrayAsync();
				requestContentBase64String = Convert.ToBase64String(content);
			}

			//create random nonce for each request
			var nonce = Guid.NewGuid().ToString("N");

			//Creating the raw signature string
			var signatureRawData = string.Concat(_apiKey, "POST", requestUri, nonce, requestContentBase64String);
			var secretKeyByteArray = Convert.FromBase64String(_apiSecret);
			var signature = Encoding.UTF8.GetBytes(signatureRawData);
			using (var hmac = new HMACSHA256(secretKeyByteArray))
			{
				var signatureBytes = hmac.ComputeHash(signature);
				var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

				//Setting the values in the Authorization header using custom scheme (amx)
				request.Headers.Authorization = new AuthenticationHeaderValue("amx", string.Format("{0}:{1}:{2}", _apiKey, requestSignatureBase64String, nonce));
			}
			return await base.SendAsync(request, cancellationToken);
		}
	}
}
