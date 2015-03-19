using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace UriShell.Shell
{
	/// <summary>
	/// The result of search of an object implementing a view for the given view model
	/// and accepting the view model as a constructor parameter.
	/// </summary>
	internal sealed class ViewModelParameterMatch : IViewModelViewMatch
	{
		/// <summary>
		/// Tries to find a match between one of constructor's parameters of the given type 
		/// and the given view model. 
		/// </summary>
		/// <param name="viewModel">The view model of a view being looked for.</param>
		/// <param name="viewType">The constructor parameter's type.</param>
		/// <param name="viewFactory">The view's factory being called when match happens 
		/// and accepting info about a parameter.</param>
		/// <returns>The result of search of an object implementing a view for the given view model; 
		/// otherwise null.</returns>
		public static ViewModelParameterMatch TryMatch(object viewModel, Type viewType, Func<ParameterInfo, object> viewFactory)
		{
			Contract.Requires<ArgumentNullException>(viewModel != null);
			Contract.Requires<ArgumentNullException>(viewType != null);
			Contract.Requires<ArgumentNullException>(viewFactory != null);

			var viewModelParameter = viewType
				.GetConstructors()
				.SelectMany(c => c.GetParameters())
				.FirstOrDefault(pi => pi.IsDefined(typeof(ViewModelAttribute), false));

			if (viewModelParameter == null)
			{
				return null;
			}

			if (!viewModelParameter.ParameterType.IsInstanceOfType(viewModel))
			{
				return null;
			}

			return new ViewModelParameterMatch(viewFactory(viewModelParameter));
		}

		/// <summary>
		/// Initializes a new instance of the class <see cref="ViewModelPropertyMatch"/>.
		/// </summary>
		/// <param name="view">The found view.</param>
		private ViewModelParameterMatch(object view)
		{
			this.View = view;
		}

		/// <summary>
		/// Gets the found view.
		/// </summary>
		public object View
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets whether the found view supports view model's change.
		/// </summary>
		public bool SupportsModelChange
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets whether the object implements a view for the given model.
		/// </summary>
		/// <param name="viewModel">The view model to be checked.</param>
		/// <returns>true, if the given object implements a view for the given model;
		/// otherwise false.</returns>
		public bool IsMatchToModel(object viewModel)
		{
			return false;
		}

		/// <summary>
		/// Changes a view model of the view.
		/// </summary>
		/// <param name="viewModel">The new view model of the view.</param>
		public void ChangeModel(object viewModel)
		{
			throw new NotSupportedException();
		}
	}
}
