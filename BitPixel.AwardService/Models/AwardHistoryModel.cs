using BitPixel.Enums;

namespace BitPixel.AwardService.Implementation
{
	public class AwardHistoryModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int? GameId { get; set; }
		public string Version { get; set; }
		public string VersionData { get; set; }
		public AwardType AwardType { get; set; }
		public int AwardId { get; internal set; }
		public ClickType ClickType { get; internal set; }
	}
}
