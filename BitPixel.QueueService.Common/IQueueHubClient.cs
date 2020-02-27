using System.Threading.Tasks;

namespace BitPixel.QueueService.Common
{
	public interface IQueueHubClient
	{
		Task<SubmitPixelResponse> SubmitPixel(SubmitPixelRequest request);
		Task<SubmitClickResponse> SubmitClick(SubmitClickRequest submitClickRequest);
	}
}
