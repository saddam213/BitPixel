using Cryptopia.QueueService.Implementation;
using System.Runtime.Serialization;

namespace Cryptopia.QueueService.DataObjects
{
	[DataContract]
	public class SubmitPixelRequest : IQueueItem
	{
		[DataMember]
		public bool IsApi { get; set; }

		[DataMember]
		public string UserId { get; set; }

		[DataMember]
		public PixelItem Pixel { get; set; }
	}
}
