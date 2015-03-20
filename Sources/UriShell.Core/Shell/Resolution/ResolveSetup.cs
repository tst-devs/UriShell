using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

using UriShell.Disposables;
using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Allows to setup an object of the given type resolved via an URI.
	/// </summary>
	/// <typeparam name="TResolved">The object's type, expected from an URI.</typeparam>
	internal sealed class ResolveSetup<TResolved> : IShellResolveSetup<TResolved>
	{
		/// <summary>
		/// The service that allows to open an object resolved via an URI.
		/// </summary>
		private readonly IShellResolveOpen _resolveOpen;

		/// <summary>
		/// The action for passing a delegate for calling object's setup.
		/// </summary>
		private readonly Action<ResolveSetupPlayer> _playerSender;
		
		/// <summary>
		/// The action, accepting an object resolved via an URI, called before opening.
		/// </summary>
		private Action<TResolved> _onReady;

		/// <summary>
		/// The action, accepting an object resolved via an URI, called when an object is disposed.
		/// </summary>
		private Action<TResolved> _onFinished;

		/// <summary>
		/// Initializes a new instance of the class <see cref="ResolveSetup{TResolved}"/>.
		/// </summary>
		/// <param name="args">Arguments for initialization of <see cref="ResolveSetup{TResolved}"/>.</param>
		public ResolveSetup(ResolveSetupArgs args)
		{
			Contract.Requires<ArgumentNullException>(args != null);

			this._resolveOpen = args.ResolveOpen;
			this._playerSender = args.PlayerSender;
		}

		/// <summary>
		/// Allows to assign an action invoked before object's opening.
		/// </summary>
		/// <param name="action">Action invoked with a resolved object before object's opening.</param>
		/// <returns>The service that allows to setup or open an object resolved from an URI.</returns>
		public IShellResolveSetup<TResolved> OnReady(Action<TResolved> action)
		{
			this._onReady = action;
			return this;
		}

		/// <summary>
		/// Allows to assign an action invoked when a resolved object is being closed. 
		/// </summary>
		/// <param name="action">The action invoked with a resolved object when the latter is being closed.</param>
		/// <returns>The service that allows to setup or open an object resolved from an URI.</returns>
		public IShellResolveSetup<TResolved> OnFinished(Action<TResolved> action)
		{
			this._onFinished = action;
			return this;
		}

		/// <summary>
		/// Opens an object resolved from an URI.
		/// </summary>
		/// <returns>The service for object's closing via <see cref="IDisposable.Dispose"/>.</returns>
		public IDisposable Open()
		{
			return this.SendPlayerAndInvoke(_ => _.Open());
		}

		/// <summary>
		/// Opens an object resolved from an URI and allows the calling site to handle an exception 
		/// when it occurs.
		/// </summary>
		/// <returns>The service for object's closing via <see cref="IDisposable.Dispose"/>,
		/// if an object was opened successfully.</returns>
		public IDisposable OpenOrThrow()
		{
			return this.SendPlayerAndInvoke(_ => _.OpenOrThrow());
		}

		/// <summary>
		/// Открывает объект, полученный через URI, заданной функцией, предварительно передав
		/// делегат вызова настроек.
		/// </summary>
		/// <param name="launchAction">Функция, открывающая объект, полученный через URI.</param>
		/// <returns>Результат вызова заданной функции.</returns>
		private IDisposable SendPlayerAndInvoke(Func<IShellResolveOpen, IDisposable> launchAction)
		{
			var player = ResolveSetup<TResolved>.CreatePlayer(this._onReady, this._onFinished);
			if (player != null)
			{
				this._playerSender(player);
			}

			return launchAction(this._resolveOpen);
		}

		/// <summary>
		/// Create a function, accepting an object resolved via an URI, 
		/// that conducts object's setup and returns an object for object's disposal.
		/// </summary>
		/// <param name="onReady">The action, accepting an object resolved via an URI, 
		/// called before opening.</param>
		/// <param name="onFinished">The action, accepting an object resolved via an URI, 
		/// called when an object is disposed.</param>
		/// <returns>The function conducting object's setup; or null when setup actions are not set.</returns>
		private static ResolveSetupPlayer CreatePlayer(Action<TResolved> onReady, Action<TResolved> onFinished)
		{
			if (onReady == null && onFinished == null)
			{
				return null;
			}

			return (uri, resolved) =>
				{
					// If the type of the object resolved via the URI 
					// mismatch the given type - log this fact and cancel setup process for the object. 
					if (!typeof(TResolved).IsInstanceOfType(resolved))
					{
						Trace.TraceWarning(
							Properties.Resources.ShellResolveSetupAbortedDueToIncompatibility,
							uri,
							typeof(TResolved).Name,
							resolved != null ? resolved.GetType().Name : "(null)");

						return Disposable.Empty;
					}

					var cast = (TResolved)resolved;

					if (onReady != null)
					{
						onReady(cast);
					}

					if (onFinished != null)
					{
						return Disposable.Create(() => onFinished(cast));
					}

					return Disposable.Empty;
				};
		}
	}
}
