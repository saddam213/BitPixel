using System.Collections.Generic;
using System.Threading.Tasks;
using DotMatrix.Enums;

namespace DotMatrix.Common.Game
{
	public interface IGameReader
	{
		Task<GameModel> GetGame(int gameId);
		Task<List<GameModel>> GetGames();
		Task<List<GameModel>> GetGames(GameStatus status);
		Task<List<string>> GetPlayers(int gameId);
	}
}
