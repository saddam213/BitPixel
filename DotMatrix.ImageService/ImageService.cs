using DotMatrix.Base.Logging;
using DotMatrix.ImageService.Implementation;
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

namespace DotMatrix.ImageService
{
	public partial class ImageService : ServiceBase
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(ImageService));
		private CancellationTokenSource _cancellationTokenSource;
		private ImageEngine _imageEngine;

		public ImageService()
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
			Log.Message(LogLevel.Info, "[OnStart] - Starting service...");
			_cancellationTokenSource = new CancellationTokenSource();
			var outputPath = ConfigurationManager.AppSettings["OutputPath"];
			_imageEngine = new ImageEngine(_cancellationTokenSource.Token, outputPath);
			_imageEngine.Start();
			Log.Message(LogLevel.Info, "[OnStart] - Started service.");
		}

		public void StopService()
		{
			Log.Message(LogLevel.Info, "[OnStop] - Stopping service.");
			_imageEngine.Stop();
			Log.Message(LogLevel.Info, "[OnStop] - Service stopped.");
		}
	}
}
