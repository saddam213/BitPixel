using System.Threading.Tasks;

using DotMatrix.Common.Results;

namespace DotMatrix.Common.Game
{
	public interface IGameWriter
	{
		Task<IWriterResult> CreateGame(CreateGameModel model);
		Task<IWriterResult> UpdateGame(UpdateGameModel model);
	}
}
