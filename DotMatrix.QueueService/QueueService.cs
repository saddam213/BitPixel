using System;
using System.Configuration;
using System.ServiceProcess;
using DotMatrix.Base.Logging;
using Microsoft.Owin.Hosting;

namespace DotMatrix.QueueService
{
	public partial class QueueService : ServiceBase
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(QueueService));
		private IDisposable _signalrHost;

		public QueueService()
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
				Log.Message(LogLevel.Info, "[OnStart] - Starting queue service...");
				var connectionUrl = ConfigurationManager.AppSettings["QueueService_Url"];
				Log.Message(LogLevel.Info, $"[OnStart] - Starting Queue service at address {connectionUrl}");
				_signalrHost = WebApp.Start<Startup>(connectionUrl);
				Log.Message(LogLevel.Info, "[OnStart] - Successfully started Queue service.");

			}
			catch (Exception ex)
			{
				Log.Exception("[OnStart] - An exception occured starting service", ex);
				throw;
			}
		}

		public void StopService()
		{
			Log.Message(LogLevel.Info, "[OnStop] - Stopping service.");
			_signalrHost.Dispose();
			Log.Message(LogLevel.Info, "[OnStop] - Service stopped.");
		}
	}
}
