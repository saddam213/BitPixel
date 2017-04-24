using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DotMatrix
{
	public class DataHub : Hub
	{
		public async Task SendData(object data)
		{
			await Clients.All.SendData(data);
		}
	}
}