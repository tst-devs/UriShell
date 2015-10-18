using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using Autofac;
using Autofac.Core;

namespace UriShell.Shell
{
	/// <summary>
	/// Implements <see cref="IViewModelViewMatcher"/> looking for views inside 
	/// the Autofac dependency injection container.
	/// </summary>
	public sealed class AutofacViewModelViewMatcher : IViewModelViewMatcher
	{
		/// <summary>
		/// Autofac main dependency injection container.
		/// </summary>
		private readonly IComponentContext _coreContainer;

        /// <summary>
        /// Autofac additional dependency injection containers.
        /// </summary>
		private readonly List<IComponentContext> _moduleContainers = new List<IComponentContext>();

		/// <summary>
		/// Initializes a new instance of the class <see cref="AutofacViewModelViewMatcher"/>.
		/// </summary>
		/// <param name="coreContainer">Autofac dependency injection container.</param>
		public AutofacViewModelViewMatcher(IComponentContext coreContainer)
		{
			Contract.Requires<ArgumentNullException>(coreContainer != null);

			this._coreContainer = coreContainer;
		}

		/// <summary>
		/// Looks for an object implementing a view for the given view model.  
		/// </summary>
		/// <param name="viewModel">The view model whose view is looked for.</param>
		/// <returns>Instance of <see cref="IViewModelViewMatch"/> if search was successful;
		/// otherwise null.</returns>
		public IViewModelViewMatch Match(object viewModel)
		{
			var containers = Enumerable.Concat(
				Enumerable.Repeat(this._coreContainer, 1), 
				this._moduleContainers);

			foreach (var container in containers)
			{
				var match = AutofacViewModelViewMatcher.Match(viewModel, container);
				if (match != null)
				{
					return match;
				}
			}

			return null;
		}

		/// <summary>
		/// Remembers the additional DI container, where this matcher will look for views. 
		/// </summary>
		/// <param name="container">The additional DI container, where this matcher will look for views. </param>
		public void AddContainer(IComponentContext container)
        {
            this._moduleContainers.Add(container);
        }

		/// <summary>
		/// Looks for an object implementing a view for the given view model 
		/// inside the given dependency injection container.  
		/// </summary>
		/// <param name="viewModel">The view model whose view is looked for.</param>
		/// <param name="diContainer">The dependency injection container.</param>
		/// <returns>Instance of <see cref="IViewModelViewMatch"/> if search was successful;
		/// otherwise null.</returns>
		private static IViewModelViewMatch Match(object viewModel, IComponentContext diContainer)
		{
			// When looking for a view in the container, watch all registrations 
			// connected with the type. Then look for ViewModelAttribute 
			// among all type's properties and constructors

			var serviceTypes = diContainer
				.ComponentRegistry
				.Registrations
				.SelectMany(r => r.Services)
				.OfType<IServiceWithType>()
				.Select(s => s.ServiceType);

			foreach (var serviceType in serviceTypes)
			{
				var propertyMatch = ViewModelPropertyMatch.TryMatch(
					viewModel,
					serviceType,
					() => diContainer.Resolve(serviceType));

				if (propertyMatch != null)
				{
					return propertyMatch;
				}

				var parameterMatch = ViewModelParameterMatch.TryMatch(
					viewModel,
					serviceType,
					pi => diContainer.Resolve(serviceType, new NamedParameter(pi.Name, viewModel)));

				if (parameterMatch != null)
				{
					return parameterMatch;
				}
			}

			return null;
		}
	}
}
