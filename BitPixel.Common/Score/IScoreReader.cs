using System.Threading.Tasks;

namespace BitPixel.Common.Score
{
	public interface IScoreReader
	{
		Task<ScoresModel> GetScoreboard();
		Task<ScoresModel> GetScoreboard(int gameId);
	}
}
