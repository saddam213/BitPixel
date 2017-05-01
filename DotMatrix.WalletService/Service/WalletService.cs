using Cryptopia.Base.Logging;
using Cryptopia.WalletAPI.Base;
using System;
using System.Configuration;
using System.ServiceModel;
using System.Threading.Tasks;


namespace DotMatrix.DepositService.Service
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.PerCall)]
	public class WalletService : IWalletService
	{
		private static readonly Log Log = LoggingManager.GetLog(typeof(WalletService));
		private static readonly string WalletUser = ConfigurationManager.AppSettings["WalletUser"];
		private static readonly string WalletPass = ConfigurationManager.AppSettings["WalletPass"];
		private static readonly string WalletHost = ConfigurationManager.AppSettings["WalletHost"];
		private static readonly string WalletPort = ConfigurationManager.AppSettings["WalletPort"];

		public async Task<string> GenerateAddress(string userId)
		{
			try
			{
				Log.Message(LogLevel.Verbose, "GenerateAddress received.");
				var walletApi = new WalletConnector(WalletHost, int.Parse(WalletPort), WalletUser, WalletPass);
				return await walletApi.GenerateAddressAsync(userId);
			}
			catch (Exception ex)
			{
				Log.Exception("An unknown exception occurred processing GenerateAddress.", ex);
				return string.Empty;
			}
		}
	}
}