using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Represents a search result of an object that implements a view for a view model.
	/// </summary>
	[ContractClass(typeof(IViewModelViewMatchContract))]
	public interface IViewModelViewMatch
	{
		/// <summary>
		/// Gets the found view.
		/// </summary>
		[Pure]
		object View
		{
			get;
		}

		/// <summary>
		/// Gets whether a found view supports view model's change.
		/// </summary>
		[Pure]
		bool SupportsModelChange
		{
			get;
		}

		/// <summary>
		/// Gets whether the object implements a view for the given model.
		/// </summary>
		/// <param name="viewModel">The view model to be checked.</param>
		/// <returns>true, if the given object implements a view for the given model;
		/// otherwise false.</returns>
		[Pure]
		bool IsMatchToModel(object viewModel);

		/// <summary>
		/// Changes a view model of a view.
		/// </summary>
		/// <param name="viewModel">The new view model for a view.</param>
		void ChangeModel(object viewModel);
	}
}
