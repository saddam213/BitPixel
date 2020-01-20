using System.Threading.Tasks;

namespace DotMatrix.Common.Award
{
	public interface IAwardWriter
	{
		Task<AddUserAwardResult> AddAward(AddUserAwardModel model);
	}
}
