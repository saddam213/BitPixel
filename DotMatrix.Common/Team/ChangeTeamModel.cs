using System.Collections.Generic;

namespace DotMatrix.Common.Team
{
	public class ChangeTeamModel
	{
		public int GameId { get; set; }
		public int? TeamId { get; set; }
		public List<TeamModel> Teams { get; set; }
	}
}
