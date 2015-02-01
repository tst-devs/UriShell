using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Events
{
	/// <summary>
	/// Позволяет рассылать и получать события без непосредственного взаимодействия
	/// между источником и получателем.
	/// </summary>
	[ContractClass(typeof(IEventBroadcasterContract))]
	public interface IEventBroadcaster
	{
		/// <summary>
		/// Добавляет подписку на событие.
		/// </summary>
		/// <param name="key">Идентификатор события, на которое осуществляется подписка.</param>
		/// <param name="handler">Подписываемый обработчик события.</param>
		/// <returns>Объект для прекращения подписки.</returns>
		IDisposable Subscribe(EventKey key, Action handler);

		/// <summary>
		/// Добавляет подписку на событие.
		/// </summary>
		/// <typeparam name="TEventArgs">Тип объекта, содержащего аргументы события.</typeparam>
		/// <param name="key">Идентификатор события, на которое осуществляется подписка.</param>
		/// <param name="handler">Подписываемый обработчик события.</param>
		/// <returns>Объект для прекращения подписки.</returns>
		IDisposable Subscribe<TEventArgs>(EventKey<TEventArgs> key, Action<TEventArgs> handler);
		
		/// <summary>
		/// Рассылает событие без аргументов.
		/// </summary>
		/// <param name="key">Идентификатор события для рассылки.</param>
		void Send(EventKey key);

		/// <summary>
		/// Рассылает событие с заданными аргументами.
		/// </summary>
		/// <typeparam name="TEventArgs">Тип объекта, содержащего аргументы события.</typeparam>
		/// <param name="key">Идентификатор события для рассылки.</param>
		/// <param name="args">Объект, содержащий аргументы события.</param>
		void Send<TEventArgs>(EventKey<TEventArgs> key, TEventArgs args);
	}
}
