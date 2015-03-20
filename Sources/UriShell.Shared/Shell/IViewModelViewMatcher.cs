using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Interface of a service that looks for an object 
	/// implementing a view for a given view model.  
	/// </summary>
	[ContractClass(typeof(IViewModelViewMatcherContract))]
	public interface IViewModelViewMatcher
	{
		/// <summary>
		/// Looks for an object implementing a view for the given view model.  
		/// </summary>
		/// <param name="viewModel">The view model whose view is looked for.</param>
		/// <returns>Instance of <see cref="IViewModelViewMatch"/> if search was successful;
		/// otherwise null.</returns>
		IViewModelViewMatch Match(object viewModel);
	}
}
