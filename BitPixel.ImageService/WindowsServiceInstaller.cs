using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace BitPixel.ImageService
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
			serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
			serviceInstaller.StartType = ServiceStartMode.Automatic;

			serviceInstaller.DisplayName = Program.ServiceName;
			serviceInstaller.Description = Program.ServiceName;
			serviceInstaller.ServiceName = Program.ServiceName;

			Installers.Add(serviceProcessInstaller);
			Installers.Add(serviceInstaller);
		}
	}
}
