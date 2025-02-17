﻿using Castle.MicroKernel;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPixel.DI.Installers
{
	public class SignalrDependencyResolver : DefaultDependencyResolver
	{
		private readonly IKernel _kernel;

		public SignalrDependencyResolver(IKernel kernel)
		{
			_kernel = kernel;
		}

		public override object GetService(System.Type serviceType)
		{
			//check if component exists in your container, if not use base to resolve
			return _kernel.HasComponent(serviceType) ? _kernel.Resolve(serviceType) : base.GetService(serviceType);
		}

		public override IEnumerable<object> GetServices(Type serviceType)
		{
			var objects = _kernel.HasComponent(serviceType) ? _kernel.ResolveAll(serviceType).Cast<object>() : new object[] { };
			return objects.Concat(base.GetServices(serviceType));
		}
	}
}
