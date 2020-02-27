using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitPixel.Cache.Common
{
	public interface IPixelCache
	{
		Task<byte[]> Initialize();
		Task<PixelCacheItem> GetPixel(int gameId, int x, int y);
		Task<IEnumerable<PixelCacheItem>> GetPixels(int gameId);
	}
}
