using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DotMatrix.Hubs
{
	public class HubIdentityProvider : IUserIdProvider
	{
		public string GetUserId(IRequest request)
		{
			if (request.User != null && request.User.Identity.IsAuthenticated)
			{
				return request.User.Identity.GetUserId();
			}
			return Guid.Empty.ToString();
		}
	}

}
