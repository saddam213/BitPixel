using DotMatrix.Common.Api;
using DotMatrix.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Api
{
	public class ApiKeyStore
	{
		private static ConcurrentDictionary<string, UserApiAuthKey> _apiAuthKeys = new ConcurrentDictionary<string, UserApiAuthKey>();
		private static ConcurrentDictionary<string, UserApiAuthKey> ApiAuthKeys
		{
			get
			{
				if (_apiAuthKeys == null || _apiAuthKeys.Count <= 0)
				{
					using (var context = new ApplicationDbContext())
					{
						var users = context.Users.Where(x => x.IsApiEnabled && !string.IsNullOrEmpty(x.ApiKey));
						if (users != null)
						{
							foreach (var user in users)
							{
								_apiAuthKeys.TryAdd(user.ApiKey, new UserApiAuthKey
								{
									Key = user.ApiKey,
									Secret = user.ApiSecret,
									UserId = user.Id
								});
							}
						}
					}
				}
				return _apiAuthKeys;
			}
		}

		public static async Task<bool> UpdateApiAuthKey(string userId, UpdateApiModel newKey)
		{
			using (var context = new ApplicationDbContext())
			{
				var user = context.Users.FirstOrDefault(x => x.Id == userId);
				if (user == null)
					return false;

				user.ApiKey = newKey.ApiKey;
				user.ApiSecret = newKey.ApiSecret;
				user.IsApiEnabled = newKey.IsApiEnabled;
				await context.SaveChangesAsync();
			}

			UserApiAuthKey authKey = null;
			if (ApiAuthKeys.Values.Any(x => x.UserId == userId))
			{
				var key = ApiAuthKeys.FirstOrDefault(x => x.Value.UserId == userId);
				ApiAuthKeys.TryRemove(key.Key, out authKey);
			}

			if (!newKey.IsApiEnabled)
			{
				return true;
			}

			return _apiAuthKeys.TryAdd(newKey.ApiKey, new UserApiAuthKey
			{
				Key = newKey.ApiKey,
				Secret = newKey.ApiSecret,
				UserId = userId
			});
		}

		public static UserApiAuthKey GetApiAuthKey(string apiKey)
		{
			UserApiAuthKey authKey = null;
			if (!ApiAuthKeys.TryGetValue(apiKey, out authKey))
			{
			}
			return authKey;
		}

		public static UserApiKeyPair GenerateApiKeyPair()
		{
			using (var cryptoProvider = new RNGCryptoServiceProvider())
			{
				var key = Guid.NewGuid().ToString("N");
				byte[] secretKeyByteArray = new byte[32]; //256 bit
				cryptoProvider.GetBytes(secretKeyByteArray);
				var secret = Convert.ToBase64String(secretKeyByteArray);
				var result = new UserApiKeyPair
				{
					Key = key,
					Secret = secret
				};

				return result;
			}
		}
	}
}
