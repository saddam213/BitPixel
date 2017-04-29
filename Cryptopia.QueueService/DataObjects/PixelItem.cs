using System.Runtime.Serialization;

namespace Cryptopia.QueueService.DataObjects
{
	[DataContract]
	public class PixelItem
	{
		[DataMember]
		public int X { get; internal set; }

		[DataMember]
		public int Y { get; internal set; }

		[DataMember]
		public string Color { get; internal set; }
	}
}

