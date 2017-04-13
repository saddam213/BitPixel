﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace DotMatrix.DI.Installers
{
	public class ControllersInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromAssemblyNamed("DotMatrix")
				.BasedOn<IController>()
				.LifestyleTransient()
				);

			container.Register(Classes.FromAssemblyNamed("DotMatrix")
				.BasedOn<IHttpController>()
				.LifestyleTransient()
				);
		}
	}
}
