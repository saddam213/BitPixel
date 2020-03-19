using BitPixel.Base.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BitPixel.WalletService
{
	static class Program
	{
		public const string ServiceName = "BitPixel.WalletService";

		static void Main()
		{
			var level = LoggingManager.LogLevelFromString(ConfigurationManager.AppSettings["LogLevel"]);
			var location = ConfigurationManager.AppSettings["LogLocation"];

#if DEBUG
			LoggingManager.AddLog(new ConsoleLogger(LogLevel.Info));
			using (var processor = new DepositService())
			{
				Console.WriteLine("Starting ImageService Service ...");
				processor.StartService();
				Console.WriteLine("Press Enter to terminate ...");
				Console.ReadLine();
				processor.StopService();
			}
#else

			LoggingManager.AddLog(new FileLogger(location, "WalletService", level));
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
								new DepositService()
			};
			ServiceBase.Run(ServicesToRun);
			LoggingManager.Destroy();
#endif
		}
	}
}
