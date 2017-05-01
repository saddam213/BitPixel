using System.ServiceModel;
using System.Threading.Tasks;

namespace DotMatrix.DepositService.Service
{
	[ServiceContract]
	public interface IWalletService
	{
		[OperationContract]
		Task<string> GenerateAddress(string userId);
	}
}
