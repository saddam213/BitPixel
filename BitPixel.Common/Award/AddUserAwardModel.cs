using BitPixel.Enums;

namespace BitPixel.Common.Award
{
	public class AddUserAwardModel
	{
		public int UserId { get; set; }
		public AwardType Type { get; set; }
		public string Version { get; set; }
		public int? GameId { get; set; }
	}
}
