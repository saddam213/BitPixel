﻿using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using DotMatrix.DI.Installers;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;

namespace DotMatrix.DI
{
	public static class DependencyRegistrar
	{
		private static IWindsorContainer _container;

		static DependencyRegistrar()
		{
			if (_container == null)
			{
				_container = BootstrapContainer();
			}
		}

		private static IWindsorContainer BootstrapContainer()
		{
			IWindsorContainer container = new WindsorContainer();
			container.Install(FromAssembly.This());
			return container;
		}

		public static void Register()
		{
			var cryptopiaDependencyResolver = new DotMatrixDependencyResolver(_container.Kernel);
			DependencyResolver.SetResolver(cryptopiaDependencyResolver);
			GlobalHost.DependencyResolver = new SignalrDependencyResolver(_container.Kernel);
			GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WebApiDependencyResolver(_container));
		}

		public static void Deregister()
		{
			_container.Dispose();
		}

		public static void RegisterTransientComponent<T>(Func<T> factoryCreate) where T : class
		{
			_container
				.Register(Component.For<T>()
					.UsingFactoryMethod(factoryCreate)
					.LifestyleTransient());
		}

		public static IWindsorContainer Container
		{
			get { return _container; }
		}
	}
}
