using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Disposables;

namespace UriShell.Shell.Events
{
	/// <summary>
	/// Позволяет рассылать и получать события без непосредственного взаимодействия
	/// между источником и получателем.
	/// </summary>
	[Obsolete("Will be replaced with the more specific interface")]
	internal sealed partial class EventBroadcaster : IEventBroadcaster
	{
		/// <summary>
		/// Список обработчиков событий, добавленных в <see cref="EventBroadcaster"/>.
		/// </summary>
		private readonly Dictionary<object, HandlerList> _handlers = new Dictionary<object, HandlerList>();

		/// <summary>
		/// Интерфейс оболочки АРМа.
		/// </summary>
		private readonly IShell _shell;

		/// <summary>
		/// Инициализирует новый объект <see cref="EventBroadcaster"/>.
		/// </summary>
		/// <param name="shell">Интерфейс оболочки АРМа.</param>
		public EventBroadcaster(IShell shell)
		{
			Contract.Requires<ArgumentNullException>(shell != null);
			this._shell = shell;
		}

		/// <summary>
		/// Возвращает список обработчиков заданного события.
		/// </summary>
		/// <param name="key">Идентификатор события, для которого запрашивается список.</param>
		/// <returns>Список обработчиков, связанный с заданным событием.</returns>
		private HandlerList GetHandlerList(object key)
		{
			HandlerList result = null;

			lock (this._handlers)
			{
				if (!this._handlers.TryGetValue(key, out result))
				{
					result = new HandlerList();
					this._handlers.Add(key, result);
				}
			}

			return result;
		}

		/// <summary>
		/// Добавляет подписку на событие.
		/// </summary>
		/// <param name="key">Идентификатор события, на которое осуществляется подписка.</param>
		/// <param name="handler">Подписываемый обработчик события.</param>
		private void SubscribeCommon(object key, Delegate handler)
		{
			var handlerList = this.GetHandlerList(key);

			lock (handlerList)
			{
				handlerList.Add(handler);
			}
		}

		/// <summary>
		/// Удаляет подписку на событие.
		/// </summary>
		/// <param name="key">Идентификатор события, от которого осуществляется отписка.</param>
		/// <param name="handler">Отписываемый обработчик события.</param>
		private void UnsubscribeCommon(object key, Delegate handler)
		{
			var handlerList = this.GetHandlerList(key);

			lock (handlerList)
			{
				handlerList.Remove(handler);
			}
		}

		/// <summary>
		/// Рассылает событие, используя заданную функцию.
		/// </summary>
		/// <param name="key">Идентификатор события для рассылки.</param>
		/// <param name="deliveryMethod">Метод для вызова обработчика события.</param>
		private void SendCommon(object key, Action<Delegate> deliveryMethod)
		{
			Delegate[] handlers;

			var handlerList = this.GetHandlerList(key);

			lock (handlerList)
			{
				handlers = handlerList.EnumerateHandlers().ToArray();
			}

			foreach (var handler in handlers)
			{
				deliveryMethod(handler);
			}
		}

		/// <summary>
		/// Добавляет подписку на событие.
		/// </summary>
		/// <param name="key">Идентификатор события, на которое осуществляется подписка.</param>
		/// <param name="handler">Подписываемый обработчик события.</param>
		/// <returns>Объект для прекращения подписки.</returns>
		public IDisposable Subscribe(EventKey key, Action handler)
		{
			this.SubscribeCommon(key, handler);
			return Disposable.Create(() => this.UnsubscribeCommon(key, handler));
		}

		/// <summary>
		/// Добавляет подписку на событие.
		/// </summary>
		/// <typeparam name="TEventArgs">Тип объекта, содержащего аргументы события.</typeparam>
		/// <param name="key">Идентификатор события, на которое осуществляется подписка.</param>
		/// <param name="handler">Подписываемый обработчик события.</param>
		/// <returns>Объект для прекращения подписки.</returns>
		public IDisposable Subscribe<TEventArgs>(EventKey<TEventArgs> key, Action<TEventArgs> handler)
		{
			this.SubscribeCommon(key, handler);
			return Disposable.Create(() => this.UnsubscribeCommon(key, handler));
		}

		/// <summary>
		/// Рассылает событие с заданными аргументами.
		/// </summary>
		/// <param name="key">Идентификатор события для рассылки.</param>
		public void Send(EventKey key)
		{
			this.SendCommon(key, handler => ((Action)handler)());
		}

		/// <summary>
		/// Рассылает событие с заданными аргументами.
		/// </summary>
		/// <typeparam name="TEventArgs">Тип объекта, содержащего аргументы события.</typeparam>
		/// <param name="key">Идентификатор события для рассылки.</param>
		/// <param name="args">Объект, содержащий аргументы события.</param>
		public void Send<TEventArgs>(EventKey<TEventArgs> key, TEventArgs args)
		{
			// Если аргументы предназначены для объекта, отрытого через URI,
			// событие должно быть разослано только этому объекту при условии,
			// что он действительно открыт.
			var resolvedIdArgs = args as ResolvedIdBroadcastArgs;

			this.SendCommon(key, handler =>
			{
				if (resolvedIdArgs != null)
				{
					if (!this._shell.IsResolvedOpen(handler.Target))
					{
						return;
					}

					if (this._shell.GetResolvedId(handler.Target) != resolvedIdArgs.ResolvedId)
					{
						return;
					}
				}

				((Action<TEventArgs>)handler)(args);
			});
		}
	}
}