using DotMatrix.Base.Logging;
using DotMatrix.EmailService.Implementation;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace DotMatrix.EmailService
{
	public partial class EmailService : ServiceBase
	{
		private EmailProcessor _emailProcessor;
		private readonly Log Log = LoggingManager.GetLog(typeof(EmailService));

		public EmailService()
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
			Log.Message(LogLevel.Info, "[EmailService] - Email service starting...");
			_emailProcessor = new EmailProcessor();
			_emailProcessor.Start().Wait();
			Log.Message(LogLevel.Info, "[EmailService] - Email service started.");
		}

		public void StopService()
		{
			Log.Message(LogLevel.Info, "[EmailService] - Email service stopping ...");
			_emailProcessor.Stop().Wait();
			Log.Message(LogLevel.Info, "[EmailService] - Email service stopped.");
		}
	}
}
