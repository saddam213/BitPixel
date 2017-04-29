using System;
using Cryptopia.QueueService.Implementation;

namespace Cryptopia.QueueService.DataObjects
{
	public class SubmitPixelResponse : IQueueResponse
	{
		public string Message { get;  set; }

		public bool Success { get;  set; }
	}
}
