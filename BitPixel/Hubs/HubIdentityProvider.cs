using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace BitPixel.Hubs
{
	public class HubIdentityProvider : IUserIdProvider
	{
		public string GetUserId(IRequest request)
		{
			if (request.User != null && request.User.Identity.IsAuthenticated)
			{
				return request.User.Identity.GetUserId();
			}
			return "0";
		}
	}

}
