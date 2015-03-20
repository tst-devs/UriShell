using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

using UriShell.Disposables;
using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Позволяет настроить объект заданного типа, полученный через URI.
	/// </summary>
	/// <typeparam name="TResolved">Тип объекта, который ожидается от URI.</typeparam>
	internal sealed class ResolveSetup<TResolved> : IShellResolveSetup<TResolved>
	{
		/// <summary>
		/// Сервис, позволяющий открыть объект, полученный через URI.
		/// </summary>
		private readonly IShellResolveOpen _resolveOpen;

		/// <summary>
		/// Действие, позволяющее передать делегат вызова настроек.
		/// </summary>
		private readonly Action<ResolveSetupPlayer> _playerSender;
		
		/// <summary>
		/// Действие, вызываемое с объектом, полученным через URI, перед открытием.
		/// </summary>
		private Action<TResolved> _onReady;

		/// <summary>
		/// Действие, вызываемое с объектом, полученным через URI, когда в нем больше
		/// нет необходимости.
		/// </summary>
		private Action<TResolved> _onFinished;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ResolveSetup{TResolved}"/>.
		/// </summary>
		/// <param name="args">Аргументы, необходимые для инициализации <see cref="ResolveSetup{TResolved}"/>.</param>
		public ResolveSetup(ResolveSetupArgs args)
		{
			Contract.Requires<ArgumentNullException>(args != null);

			this._resolveOpen = args.ResolveOpen;
			this._playerSender = args.PlayerSender;
		}

		/// <summary>
		/// Позволяет задать действие, вызываемое с объектом, полученным через URI,
		/// перед открытием.
		/// </summary>
		/// <param name="action">Действие, вызываемое с объектом, полученным через URI,
		/// перед открытием.</param>
		/// <returns>Сервис, позволяющий настроить и открыть объект, полученный через URI.</returns>
		public IShellResolveSetup<TResolved> OnReady(Action<TResolved> action)
		{
			this._onReady = action;
			return this;
		}

		/// <summary>
		/// Позволяет задать действие, вызываемое с объектом, полученным через URI,
		/// когда в нем больше нет необходимости.
		/// </summary>
		/// <param name="action">Действие, вызываемое с объектом, полученным через URI,
		/// когда в нем больше нет необходимости.</param>
		/// <returns>Сервис, позволяющий настроить и открыть объект, полученный через URI.</returns>
		public IShellResolveSetup<TResolved> OnFinished(Action<TResolved> action)
		{
			this._onFinished = action;
			return this;
		}

		/// <summary>
		/// Открывает объект, полученный через URI.
		/// </summary>
		/// <returns>Сервис, позволяющий закрыть объект вызовом <see cref="IDisposable.Dispose"/>.</returns>
		public IDisposable Open()
		{
			return this.SendPlayerAndInvoke(_ => _.Open());
		}		
		
		/// <summary>
		/// Открывает объект, полученный через URI, позволяя вызывающему коду обработать
		/// исключение в случае неудачи.
		/// </summary>
		/// <returns>Сервис, позволяющий закрыть объект вызовом <see cref="IDisposable.Dispose"/>,
		/// если объект открыт успешно.</returns>
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
		/// Создает функцию, принимающую на вход полученный через URI объект, и выполняющую
		/// его настройку, результатом которой служит сервис, вызываемый, когда в объекте
		/// больше нет необходимости. 
		/// </summary>
		/// <param name="onReady">Действие, вызываемое с объектом, полученным через URI,
		/// перед открытием.</param>
		/// <param name="onFinished">Действие, вызываемое с объектом, полученным через URI,
		/// когда в нем больше нет необходимости.</param>
		/// <returns>Созданную функцию, выполняющую настройку; или null, если настройки не заданы.</returns>
		private static ResolveSetupPlayer CreatePlayer(Action<TResolved> onReady, Action<TResolved> onFinished)
		{
			if (onReady == null && onFinished == null)
			{
				return null;
			}

			return (uri, resolved) =>
				{
					// Если тип объекта, полученного через URI, отличается от
					// заданного, логируем этот факт, и отменяем вызов настроек.
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
