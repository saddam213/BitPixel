using System.Runtime.Serialization;

namespace Cryptopia.QueueService.DataObjects
{
	[DataContract]
	public class PixelItem
	{
		[DataMember]
		public int X { get; set; }

		[DataMember]
		public int Y { get; set; }

		[DataMember]
		public byte R { get; set; }

		[DataMember]
		public byte G { get; set; }

		[DataMember]
		public byte B { get; set; }
	}
}

