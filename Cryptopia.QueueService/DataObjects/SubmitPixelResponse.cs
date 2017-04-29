using System;
using Cryptopia.QueueService.Implementation;
using System.Runtime.Serialization;

namespace Cryptopia.QueueService.DataObjects
{
	[DataContract]
	public class SubmitPixelResponse : IQueueResponse
	{
		[DataMember]
		public bool Success { get; set; }

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public decimal Balance { get; set; }
	}
}
