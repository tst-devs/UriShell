using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace UriShell.Shell
{
	/// <summary>
	/// The result of search of an object implementing a view for the given view model
	/// and accepting the view model as a property.
	/// </summary>
	public sealed class ViewModelPropertyMatch : IViewModelViewMatch
	{
		/// <summary>
		/// Tries to find a match between a property of the given type 
		/// and the given view model.
		/// </summary>
		/// <param name="viewModel">The view model of a view being looked for.</param>
		/// <param name="viewType">The type of an object which properties are analyzed.</param>
		/// <param name="viewFactory">The view factory called when a match happens.</param>
		/// <returns>The result of search of an object implementing a view for the given view model; 
		/// otherwise null.</returns>
		public static ViewModelPropertyMatch TryMatch(object viewModel, Type viewType, Func<object> viewFactory)
		{
			Contract.Requires<ArgumentNullException>(viewModel != null);
			Contract.Requires<ArgumentNullException>(viewType != null);
			Contract.Requires<ArgumentNullException>(viewFactory != null);

			var viewModelProperty = viewType
				.GetProperties()
				.Where(pi => pi.IsDefined(typeof(ViewModelAttribute), false))
				.FirstOrDefault(pi => ViewModelPropertyMatch.IsPropertyMatchToModel(pi, viewModel));

			if (viewModelProperty == null)
			{
				return null;
			}

			// Создаем представление и задаем ему модель.
			var match = new ViewModelPropertyMatch(viewFactory(), viewModelProperty);
			match.ChangeModel(viewModel);

			return match;
		}

		/// <summary>Checks whether the given property implements the given view model.
		/// </summary>
		/// <param name="viewModelProperty">The property to be checked.</param>
		/// <param name="viewModel">The view model to be checked.</param>
		/// <returns>true, if the given property implements the given view model;
		/// otherwise false.</returns>
		private static bool IsPropertyMatchToModel(PropertyInfo viewModelProperty, object viewModel)
		{
			if (!viewModelProperty.CanWrite)
			{
				return false;
			}

			return viewModelProperty.PropertyType.IsInstanceOfType(viewModel);
		}

		/// <summary>
		/// The found view that has a property for the view model.
		/// </summary>
		private readonly object _view;

		/// <summary>
		/// <see cref="PropertyInfo"/> of the property for the view model.
		/// </summary>
		private readonly PropertyInfo _viewModelProperty;

		/// <summary>
		/// Initializes a new instance of the class <see cref="ViewModelPropertyMatch"/>.
		/// </summary>
		/// <param name="view">The found view that has a property for the view model.</param>
		/// <param name="viewModelProperty"><see cref="PropertyInfo"/> 
		/// of the property for the view model.</param>
		private ViewModelPropertyMatch(object view, PropertyInfo viewModelProperty)
		{
			this._view = view;
			this._viewModelProperty = viewModelProperty;
		}

		/// <summary>
		/// Gets the found view.
		/// </summary>
		public object View
		{
			get
			{
				return this._view;
			}
		}

		/// <summary>
		/// Gets whether the found view supports view model's change.
		/// </summary>
		public bool SupportsModelChange
		{
			get
			{
				return true;
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
			return ViewModelPropertyMatch.IsPropertyMatchToModel(this._viewModelProperty, viewModel);
		}

		/// <summary>
		/// Changes a view model of the view.
		/// </summary>
		/// <param name="viewModel">The new view model of the view.</param>
		public void ChangeModel(object viewModel)
		{
			this._viewModelProperty.SetValue(this._view, viewModel, new object[0]);
		}
	}
}
