using Cryptopia.QueueService.Implementation;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cryptopia.QueueService.DataObjects
{
	[DataContract]
	public class SubmitPixelsRequest : IQueueItem
	{
		[DataMember]
		public bool IsApi { get; set; }

		[DataMember]
		public string UserId { get; set; }

		[DataMember]
		public List<PixelItem> Pixels { get; set; }
	}
}
