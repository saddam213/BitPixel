using System.Threading.Tasks;

namespace DotMatrix.Common.Score
{
	public interface IScoreReader
	{
		Task<ScoresModel> GetScoreboard();
		Task<ScoresModel> GetScoreboard(int gameId);
	}
}
