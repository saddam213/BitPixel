using BitPixel.Base.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BitPixel.QueueService
{
	static class Program
	{
		public const string ServiceName = "BitPixel.QueueService";

		static void Main()
		{
			var level = LoggingManager.LogLevelFromString(ConfigurationManager.AppSettings["LogLevel"]);
			var location = ConfigurationManager.AppSettings["LogLocation"];

#if DEBUG
			LoggingManager.AddLog(new ConsoleLogger(level));
			using (var processor = new QueueService())
			{
				Console.WriteLine("Starting QueueProcessor Service ...");
				processor.StartService();
				Console.WriteLine("Press Enter to terminate ...");
				Console.ReadLine();
				processor.StopService();
			}
#else

			LoggingManager.AddLog(new FileLogger(location, "QueueService", level));
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
								new QueueService()
			};
			ServiceBase.Run(ServicesToRun);
			LoggingManager.Destroy();
#endif
		}
	}
}
