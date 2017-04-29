using Cryptopia.QueueService.Implementation;
using System.Collections.Generic;
using System;

namespace Cryptopia.QueueService.DataObjects
{
	public class ErrorResponse : IQueueResponse
	{
		public ErrorResponse(string error)
		{
			Success = false;
			Message = error;
		}

		public bool Success { get; set; }
		public string Message { get; set; }
	}
}
