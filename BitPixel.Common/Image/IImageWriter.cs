using System.Threading.Tasks;

using BitPixel.Common.Results;
using BitPixel.Common.Users;

namespace BitPixel.Common.Image
{
	public interface IImageWriter
	{
		Task<IWriterResult> CreateFixedImage(CreateFixedImageModel model);
		Task<IWriterResult> CreateAvatarImage(UpdateAvatarModel model);
	}
}
