using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.DI.Installers
{
	public class CoreInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{


			container.Register(Classes.FromAssemblyContaining<DotMatrix.Core.Core>()
				.Pick()
				.WithService
				.DefaultInterfaces()
				.LifestyleTransient()
			);

			container.Register(Classes.FromAssemblyContaining<DotMatrix.Data.Data>()
				.Pick()
				.WithService
				.DefaultInterfaces()
				.LifestyleTransient()
			);

			//container.Register(Classes.FromAssemblyContaining<DotMatrix.Cache.Cache>()
			//	.Pick()
			//	.WithService
			//	.DefaultInterfaces()
			//	.LifestyleSingleton()
			//);
		}
	}
}
