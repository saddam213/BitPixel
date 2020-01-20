using System;

namespace DotMatrix.QueueService.Implementation
{
	public class QueueException : Exception
	{
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
