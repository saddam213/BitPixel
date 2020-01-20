using DotMatrix.Enums;

namespace DotMatrix.Common.Award
{
	public class AddUserAwardModel
	{
		public int UserId { get; set; }
		public AwardType Type { get; set; }
		public string Version { get; set; }
	}
}
