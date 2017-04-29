﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.ImageService
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
			serviceInstaller.DisplayName = "Cryptopia.ImageService";
			serviceInstaller.StartType = ServiceStartMode.Manual;
			serviceInstaller.Description = "Cryptopia.ImageService";
			//# This must be identical to the WindowsService.ServiceBase name
			//# set in the constructor of WindowsService.cs
			serviceInstaller.ServiceName = "DotMatrix.ImageService";
			this.Installers.Add(serviceProcessInstaller);
			this.Installers.Add(serviceInstaller);
		}
	}
}
