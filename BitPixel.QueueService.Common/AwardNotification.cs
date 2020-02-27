using BitPixel.Enums;

namespace BitPixel.QueueService.Common
{
	public class AwardNotification
	{
		public int UserId { get; set; }
		public int AwardId { get; set; }
		public string Name { get; set; }
		public int Points { get; set; }
		public AwardLevel Level { get; set; }
		public string UserName { get; set; }
	}
}
