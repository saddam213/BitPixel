using System.Threading.Tasks;

namespace DotMatrix.QueueService.Common
{
	public interface IQueueHubClient
	{
		Task<SubmitPixelResponse> SubmitPixel(SubmitPixelRequest request);
		Task<SubmitClickResponse> SubmitClick(SubmitClickRequest submitClickRequest);
	}
}
