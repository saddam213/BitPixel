﻿using Castle.MicroKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DotMatrix.DI.Installers
{
	public class DotMatrixControllerFactory : DefaultControllerFactory
	{
		private readonly IKernel kernel;

		public DotMatrixControllerFactory(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public override void ReleaseController(IController controller)
		{
			kernel.ReleaseComponent(controller);
		}

		protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
		{
			if (controllerType == null)
			{
				throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
			}
			return (IController)kernel.Resolve(controllerType);
		}
	}
}
