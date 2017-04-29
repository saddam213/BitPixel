using Cryptopia.QueueService.DataObjects;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Cryptopia.QueueService.Service
{
	[ServiceContract]
	public interface IQueueProcessor
	{
		[OperationContract]
		Task<SubmitPixelResponse> SubmitPixel(SubmitPixelRequest request);

		[OperationContract]
		Task<SubmitPixelsResponse> SubmitPixels(SubmitPixelsRequest request);
	}
}
