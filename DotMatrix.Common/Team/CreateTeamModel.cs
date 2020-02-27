using System.Collections.Generic;
using DotMatrix.Common.Game;

namespace DotMatrix.Common.Team
{
	public class CreateTeamModel
	{
		public int GameId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Icon { get; set; }
		public string Color { get; set; }
		public int Rank { get; set; }

		public List<GameModel> Games { get; set; }
	}
}
