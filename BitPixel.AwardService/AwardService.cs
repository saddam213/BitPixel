using BitPixel.Base.Logging;
using BitPixel.AwardService.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitPixel.AwardService
{
	public partial class AwardService : ServiceBase
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(AwardService));
		private AwardEngine _awardEngine;

		public AwardService()
		{
			InitializeComponent();
			ServiceName = Program.ServiceName;
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
			Log.Message(LogLevel.Info, "[OnStart] - Starting service...");
			_awardEngine = new AwardEngine();
			_awardEngine.Start();
			Log.Message(LogLevel.Info, "[OnStart] - Started service.");
		}

		public void StopService()
		{
			Log.Message(LogLevel.Info, "[OnStop] - Stopping service.");
			_awardEngine.Stop().Wait();
			Log.Message(LogLevel.Info, "[OnStop] - Service stopped.");
		}
	}
}
