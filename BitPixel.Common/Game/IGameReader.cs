using System.Collections.Generic;
using System.Threading.Tasks;
using BitPixel.Common.Team;
using BitPixel.Datatables;
using BitPixel.Datatables.Models;
using BitPixel.Enums;

namespace BitPixel.Common.Game
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
