using System;
using System.Diagnostics.Contracts;

using Autofac;

using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Implementation of a factory of a service that makes setup of objects resolved via an URI. 
	/// Реализация фабрики сервиса настройки объектов, полученных через URI.
	/// </summary>
	internal sealed class ResolveSetupFactory : IResolveSetupFactory
	{
		/// <summary>
		/// The dependency injection container.
		/// </summary>
		private readonly IComponentContext _diContainer;

		/// <summary>
		/// Initializes a new instance of the class <see cref="ResolveSetupFactory"/>.
		/// </summary>
		/// <param name="diContainer">The dependency injection container.</param>
		public ResolveSetupFactory(IComponentContext diContainer)
		{
			Contract.Requires<ArgumentNullException>(diContainer != null);
			this._diContainer = diContainer;
		}

		/// <summary>
		/// Creates the service that implements setup of objects resolved via an URI. 
		/// </summary>
		/// <typeparam name="TResolved">The object's type expected from URI's resolution.</typeparam>
		/// <param name="args">Arguments for initialization <see cref="ResolveSetup{TResolved}"/>.</param>
		/// <returns>The service that allows to setup and open an object resolved via an URI.</returns>
		public IShellResolveSetup<TResolved> Create<TResolved>(ResolveSetupArgs args)
		{
			return this._diContainer.Resolve<IShellResolveSetup<TResolved>>(TypedParameter.From(args));
		}
	}
}
