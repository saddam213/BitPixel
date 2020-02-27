using System.Collections.Generic;

namespace BitPixel.Common.Team
{
	public class ChangeTeamModel
	{
		public int GameId { get; set; }
		public int? TeamId { get; set; }
		public List<TeamModel> Teams { get; set; }
	}
}
