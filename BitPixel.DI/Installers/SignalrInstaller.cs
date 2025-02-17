﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPixel.DI.Installers
{
	public class SignalrInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromAssemblyNamed("BitPixel")
				.BasedOn<IHub>()
				.LifestyleTransient()
				);
		}
	}
}
