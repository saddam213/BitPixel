using Cryptopia.Base.Logging;
using DotMatrix.DepositService.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotMatrix.WalletService
{
	public partial class DepositService : ServiceBase
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(DepositService));
		private CancellationTokenSource _cancellationTokenSource;
		private DepositEngine _depositEngine;
		private ServiceHost _serviceHost;

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
				if (_serviceHost != null)
					_serviceHost.Close();

				Log.Message(LogLevel.Info, "[OnStart] - Starting service...");
				_cancellationTokenSource = new CancellationTokenSource();
				_depositEngine = new DepositEngine(_cancellationTokenSource.Token);
				_depositEngine.Start();

				// open service for incomming calls
				Log.Message(LogLevel.Info, "[OnStart] - Opening service host");
				_serviceHost = new ServiceHost(typeof(DotMatrix.DepositService.Service.WalletService));
				_serviceHost.Open();
				Log.Message(LogLevel.Info, "[OnStart] - Service host opened.");
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
			if (_serviceHost != null)
			{
				Log.Message(LogLevel.Info, "[OnStop] - Closing service host.");
				_serviceHost.Close();
				_serviceHost = null;
			}
			Log.Message(LogLevel.Info, "[OnStop] - Service stopped.");
		}
	}
}
