using System;
using System.ServiceProcess;
using System.Threading;

using DotMatrix.Base.Logging;
using DotMatrix.WalletService.Implementation;

namespace DotMatrix.WalletService
{
	public partial class DepositService : ServiceBase
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(DepositService));
		private CancellationTokenSource _cancellationTokenSource;
		private DepositEngine _depositEngine;

		public DepositService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			StartService();
		}

		protected override void OnStop()
		{
			StopService();
		}

		public void StartService()
		{
			try
			{

				Log.Message(LogLevel.Info, "[OnStart] - Starting service...");
				_cancellationTokenSource = new CancellationTokenSource();
				_depositEngine = new DepositEngine(_cancellationTokenSource.Token);
				_depositEngine.Start();
				Log.Message(LogLevel.Info, "[OnStart] - Successfully started Wallet service.");
			}
			catch (Exception ex)
			{
				Log.Exception("[OnStart] - An exception occured starting service", ex);
				throw;
			}

			Log.Message(LogLevel.Info, "[OnStart] - Started service.");
		}

		public void StopService()
		{
			Log.Message(LogLevel.Info, "[OnStop] - Stopping service.");
			_depositEngine.Stop();
			Log.Message(LogLevel.Info, "[OnStop] - Service stopped.");
		}
	}
}
