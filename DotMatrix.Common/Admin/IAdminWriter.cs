using System.Threading.Tasks;

namespace DotMatrix.Common.Admin
{
	public interface IAdminWriter
	{
		Task<CreatePrizePoolResult> CreatePrizePool(CreatePrizePoolModel model);
	}
}
