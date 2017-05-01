using DotMatrix.Common.DataContext;
using DotMatrix.Common.Wallet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Core.Wallet
{
	public class WalletService : IWalletService
	{
		private static readonly string WalletServiceUsername = ConfigurationManager.AppSettings["WalletServiceUsername"];
		private static readonly string WalletServicePassword = ConfigurationManager.AppSettings["WalletServicePassword"];
		private static readonly string WalletServiceDomain = ConfigurationManager.AppSettings["WalletServiceDomain"];

		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<string> GenerateAddress(string userId)
		{
			try
			{
				using (var client = CreateService())
				using (var context = DataContextFactory.CreateContext())
				{
					var address = await client.GenerateAddressAsync(userId);
					if (string.IsNullOrEmpty(address))
						return string.Empty;

					var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
					if (user == null)
						return string.Empty;

					user.Address = address;
					await context.SaveChangesAsync();
					return address;
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public DotMatrix.Core.Cryptopia.WalletService.WalletServiceClient CreateService()
		{
			var client = new DotMatrix.Core.Cryptopia.WalletService.WalletServiceClient();
#if !DEBUG
			client.ClientCredentials.Windows.ClientCredential.UserName = WalletServiceUsername;
			client.ClientCredentials.Windows.ClientCredential.Password = WalletServicePassword;
			client.ClientCredentials.Windows.ClientCredential.Domain = WalletServiceDomain;
#endif
			return client;
		}
	}
}
