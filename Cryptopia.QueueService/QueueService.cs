using Cryptopia.QueueService.Service;
using Cryptopia.Base.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.QueueService
{
	public partial class QueueService : ServiceBase
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(QueueService));
		private ServiceHost _serviceHost;

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
			Log.Message(LogLevel.Info, "[OnStart] - Starting queue service...");
			if (_serviceHost != null)
			{
				_serviceHost.Close();
			}

			try
			{
				// open service for incomming calls
				Log.Message(LogLevel.Info, "[OnStart] - Opening queue host");
				_serviceHost = new ServiceHost(typeof(QueueProcessorService));
				_serviceHost.Open();
				Log.Message(LogLevel.Info, "[OnStart] - Inbound queue started.");
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
