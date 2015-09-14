using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;

using UriShell.Extensions;
using UriShell.Shell;

namespace UriShell.Input
{
	/// <summary>
	/// Implements the command for the given URI's opening.
	/// </summary>
	[Obsolete("ICommand requires the PresentationCore assembly -> It can't be used separately from WPF. ")]
	public sealed class OpenUriCommand : ICommand
	{
		/// <summary>
		/// The interface of the application shell.
		/// </summary>
		private readonly IShell _shell;

		/// <summary>
		/// Implements a new instance of the class <see cref="OpenUriCommand"/>.
		/// </summary>
		/// <param name="shell">The interface of the application shell.</param>
		public OpenUriCommand(IShell shell)
		{
			Contract.Requires<ArgumentNullException>(shell != null);

			this._shell = shell;
		}

		/// <summary>
		/// Checks if the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">The command's parameter. 
		/// If the command doesn't require a parameter, could be null.</param>
		/// <returns>true, if the command can execute; otherwise false.</returns>
		public bool CanExecute(object parameter)
		{
			if (parameter == null)
			{
				return false;
			}

			if (parameter is Uri)
			{
				return true;
			}

			Uri uri;
			return Uri.TryCreate(parameter.ToString(), UriKind.Absolute, out uri);
		}

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="parameter">The command's parameter. 
		/// If the command doesn't require a parameter, could be null.</param>
		public void Execute(object parameter)
		{
			var uri = parameter as Uri;
			if (uri == null)
			{
				uri = new Uri(parameter.ToString(), UriKind.Absolute);
			}

			// If the URI isn't for Phoenix, open it externally.
			if (!uri.IsUriShell())
			{
				uri = ShellUriBuilder.StartUri()
					.Placement("external")
					.Module("arm")
					.Item("open")
					.Parameter("fileName", uri.ToString())
					.End();
			}

			this._shell.Resolve(uri).Open();
		}

		/// <summary>
		/// Invoked when a change, that affect command's availability, happens.
		/// </summary>
		event EventHandler ICommand.CanExecuteChanged
		{
			add
			{
				// The command is available always. 
				// Nothing to do.
			}
			remove
			{
			}
		}
	}
}
