using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;

using UriShell.Extensions;
using UriShell.Shell;

namespace UriShell.Input
{
	/// <summary>
	/// Реализует команду открытия заданного URI.
	/// </summary>
	public sealed class OpenUriCommand : ICommand
	{
		/// <summary>
		/// Интерфейс оболочки АРМа.
		/// </summary>
		private readonly IShell _shell;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="OpenUriCommand"/>.
		/// </summary>
		/// <param name="shell">Интерфейс оболочки АРМа.</param>
		public OpenUriCommand(IShell shell)
		{
			Contract.Requires<ArgumentNullException>(shell != null);

			this._shell = shell;
		}

		/// <summary>
		/// Определяет, может ли команда выполняться в ее текущем состоянии.
		/// </summary>
		/// <param name="parameter">Данные, используемые командой. Если команда не требует
		/// передачи данных, этот объект может быть установлен в null.</param>
		/// <returns>true, если команда может быть выполнена; иначе false.</returns>
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
		/// Выполняет данную команду.
		/// </summary>
		/// <param name="parameter">Данные, используемые командой. Если команда не требует
		/// передачи данных, этот объект может быть установлен в null.</param>
		public void Execute(object parameter)
		{
			var uri = parameter as Uri;
			if (uri == null)
			{
				uri = new Uri(parameter.ToString(), UriKind.Absolute);
			}

			// Если URI не соответствует схеме Феникса, открываем ее вовне.
			if (!uri.IsPhoenix())
			{
				uri = PhoenixUriBuilder.StartUri()
					.Placement("external")
					.Module("arm")
					.Item("open")
					.Parameter("fileName", uri.ToString())
					.End();
			}

			this._shell.Resolve(uri).Open();
		}

		/// <summary>
		/// Вызывается при изменениях, влияющих на то, должна ли выполняться команда. 
		/// </summary>
		event EventHandler ICommand.CanExecuteChanged
		{
			add
			{
				// команда доступна всегда,
				// поэтому нет необходимости что-либо делать.
			}
			remove
			{
			}
		}
	}
}
