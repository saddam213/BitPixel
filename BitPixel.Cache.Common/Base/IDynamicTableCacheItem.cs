namespace BitPixel.Cache.Common
{
	public interface IDynamicTableCacheItem<T> : ICacheItem<T>
	{
		byte[] LastUpdated { get; set; }
	}
}
