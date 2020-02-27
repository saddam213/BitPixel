using System.Threading.Tasks;

using BitPixel.Common.Results;
using BitPixel.Common.Team;

namespace BitPixel.Common.Game
{
	public interface IGameWriter
	{
		Task<IWriterResult> CreateGame(CreateGameModel model);
		Task<IWriterResult> UpdateGame(UpdateGameModel model);
		Task<IWriterResult> CreateTeam(CreateTeamModel model);
		Task<IWriterResult> UpdateTeam(UpdateTeamModel model);
		Task<IWriterResult> ChangeTeam(int userId, ChangeTeamModel model);
	}
}
