using System;
using System.Threading.Tasks;

namespace BitPixel.Cache.Common
{
	public interface IThrottleCache
	{
		Task<bool> ShouldExcecute(int userId, string method, TimeSpan period, int maxCalls);
		Task<bool> ShouldExcecute(string ipAddress, string method, TimeSpan period, int maxCalls);
	}
}
