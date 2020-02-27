using System.Collections.Generic;
using System.Threading.Tasks;
using DotMatrix.Common.Team;
using DotMatrix.Datatables;
using DotMatrix.Datatables.Models;
using DotMatrix.Enums;

namespace DotMatrix.Common.Game
{
	public interface IGameReader
	{
		Task<GameModel> GetGame(int gameId);
		Task<List<GameModel>> GetGames();
		Task<List<string>> GetPlayers(int gameId);

		Task<TeamModel> GetTeam(int teamId);
		Task<List<TeamModel>> GetTeams(int gameId);
		Task<DataTablesResponseData> GetTeams(DataTablesParam model);

		Task<TeamModel> GetUserTeam(int userId, int gameId);

		Task<GameStatsModel> GetStats(int gameId);
	}
}
