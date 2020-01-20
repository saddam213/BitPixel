using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace DotMatrix.EmailService
{
	[RunInstaller(true)]
	public class WindowsServiceInstaller : Installer
	{
		/// <summary>
		/// Public Constructor for WindowsServiceInstaller.
		/// - Put all of your Initialization code here.
		/// </summary>
		public WindowsServiceInstaller()
		{
		
			var serviceProcessInstaller = new ServiceProcessInstaller();
			var serviceInstaller = new ServiceInstaller();

			//# Service Account Information
			serviceProcessInstaller.Account = ServiceAccount.LocalService;

			//# Service Information
			var serviceName = $"DotMatrix.EmailService";
			serviceInstaller.DisplayName = serviceName;
			serviceInstaller.ServiceName = serviceName;
			serviceInstaller.Description = serviceName;
			serviceInstaller.StartType = ServiceStartMode.Automatic;
			serviceInstaller.DelayedAutoStart = true;
			//# This must be identical to the WindowsService.ServiceBase name
			//# set in the constructor of WindowsService.cs

			Installers.Add(serviceProcessInstaller);
			Installers.Add(serviceInstaller);
		}
	}
}