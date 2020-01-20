using DotMatrix.Api;
using DotMatrix.Common.Api;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace DotMatrix.Api
{
	public class ApiAuthenticationAttribute : Attribute, IAuthenticationFilter
	{
		public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			try
			{
				await context.Request.Content.LoadIntoBufferAsync().ConfigureAwait(false);
				var request = context.Request;
				var requestParameters = await request.ReadRequestNameValueCollectionAsync().ConfigureAwait(false);
				var apiKey = request.Headers.GetValue("apiKey") ?? requestParameters["apiKey"];
				var requestSignature = request.Headers.GetValue("signature") ?? requestParameters["signature"];
				requestParameters.Remove("apiKey");
				requestParameters.Remove("signature");

				var apiAuthKey = ApiKeyStore.GetApiAuthKey(apiKey);
				if (apiAuthKey == null)
				{
					context.ErrorResult = CreateErrorResponse(context.Request, HttpStatusCode.Unauthorized, "Invalid Apikey.");
					return;
				}

				//// Check throttle for key
				//var throttleResult = await ThrottlePolicyStore.CheckThrottlePolicy(apiKeyInfo.UserId, apiKeyInfo.SubscriptionPolicyId);
				//if (throttleResult.ThrottleRequest)
				//{
				//	context.ErrorResult = CreateErrorResponse(context.Request, (HttpStatusCode)429, throttleResult.Message);
				//	return;
				//}

				//// Check Replay
				//var nonce = requestParameters["nonce"];
				//if (!await ApiKeyStore.ValidateRequestNonce(apiAuthKey.Key, nonce))
				//{
				//	context.ErrorResult = CreateErrorResponse(context.Request, HttpStatusCode.Unauthorized, "Invalid nonce, possible replay");
				//	return;
				//}

				// Check signature
				var serverSignature = AuthExtensions.CreateSignature(apiAuthKey.Secret, requestParameters.ToString());
				if (!serverSignature.Equals(requestSignature))
				{
					context.ErrorResult = CreateErrorResponse(context.Request, HttpStatusCode.Unauthorized, "Invalid signature");
					return;
				}

				// We are authenticated, Build Identity
				var identity = new ClaimsIdentity("ApiKey", ClaimTypes.NameIdentifier, ClaimTypes.Role);
				identity.AddClaim(new Claim(ClaimTypes.Name, apiAuthKey.UserId.ToString()));
				identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, apiAuthKey.UserId.ToString()));
				context.Principal = new ClaimsPrincipal(identity);
			}
			catch (Exception ex)
			{
				context.ErrorResult = CreateErrorResponse(context.Request, HttpStatusCode.Unauthorized, ex.Message);
			}
		}

		/// <summary>
		/// Challenges the asynchronous.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			context.Result = new ApiResultWithChallenge(context.Result);
			return Task.FromResult(0);
		}

		/// <summary>
		/// Gets or sets a value indicating whether more than one instance of the indicated attribute can be specified for a single program element.
		/// </summary>
		/// <returns>true if more than one instance is allowed to be specified; otherwise, false. The default is false.</returns>
		public bool AllowMultiple
		{
			get { return false; }
		}





		private AuthenticationErrorResult CreateErrorResponse(HttpRequestMessage request, HttpStatusCode statusCode, string error)
		{
			return new AuthenticationErrorResult(request, error, statusCode);
		}
	}

	public class AuthenticationErrorResult : IHttpActionResult
	{
		public AuthenticationErrorResult(HttpRequestMessage request, string message, HttpStatusCode status = HttpStatusCode.InternalServerError)
		{
			Request = request;
			Message = message;
			Status = status;
		}

		public HttpStatusCode Status { get; private set; }

		public string Message { get; private set; }

		public HttpRequestMessage Request { get; private set; }

		public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(Execute());
		}

		private HttpResponseMessage Execute()
		{
			var response = Request.CreateResponse(Status, new ErrorResult(Message));
			response.RequestMessage = Request;
			response.ReasonPhrase = Message;
			return response;
		}
	}
	public class ErrorResult
	{
		public ErrorResult(string error)
		{
			Error = error;
		}

		public string Error { get; }
	}
	public static class AuthExtensions
	{
		public static string CreateSignature(string secretKey, string dataToSign)
		{
			using (var hmacSha512 = new HMACSHA512(Encoding.ASCII.GetBytes(secretKey)))
			{
				return BitConverter.ToString(hmacSha512.ComputeHash(Encoding.ASCII.GetBytes(dataToSign))).Replace("-", string.Empty);
			}
		}

		public static string GetValue(this HttpRequestHeaders headers, string name)
		{
			if (headers.TryGetValues(name, out var values))
			{
				return values.FirstOrDefault();
			}
			return null;
		}

		public static async Task<NameValueCollection> ReadRequestNameValueCollectionAsync(this HttpRequestMessage request)
		{
			var requestContent = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
			var requestContentCollection = request.RequestUri.ParseQueryString();
			if (!string.IsNullOrEmpty(requestContent))
			{
				if (request.Content.IsFormData())
				{
					requestContentCollection.Add(HttpUtility.ParseQueryString(requestContent));
				}
				else
				{
					try
					{
						var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestContent);
						foreach (var value in values)
						{
							requestContentCollection.Add(value.Key, value.Value);
						}
					}
					catch { }
				}
			}
			return requestContentCollection;
		}
	}
}
