using System;

namespace Cryptopia.QueueService.Implementation
{
	public class QueueException : Exception
	{
		public IQueueResponse Response { get; set; }
		public QueueException(string messge)
				: base(messge)
		{
		}

		public QueueException(string messge, params object[] para)
				: base(string.Format(messge, para))
		{
		}

		
	}
}
