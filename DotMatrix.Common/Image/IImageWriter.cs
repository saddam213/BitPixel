using System.Threading.Tasks;

using DotMatrix.Common.Results;
using DotMatrix.Common.Users;

namespace DotMatrix.Common.Image
{
	public interface IImageWriter
	{
		Task<IWriterResult> CreateFixedImage(CreateFixedImageModel model);
		Task<IWriterResult> CreateAvatarImage(UpdateAvatarModel model);
	}
}
