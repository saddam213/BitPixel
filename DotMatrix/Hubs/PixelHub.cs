using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DotMatrix
{
	public class PixelHub : Hub
	{
		public async Task SendPixelData(int x, int y, string color)
		{
			await Clients.All.SendPixelData(x, y, color);
		}
	}
}