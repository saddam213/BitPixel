using System.Threading.Tasks;

namespace DotMatrix.Common.Score
{
	public interface IScoreReader
	{
		Task<ScoreViewModel> GetScoreboard();
	}
}
