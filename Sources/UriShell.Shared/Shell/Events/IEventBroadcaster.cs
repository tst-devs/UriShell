using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Events
{
	/// <summary>
	/// Allows to send and receive events without direct interaction between a sender and a receiver.
	/// </summary>
	[ContractClass(typeof(IEventBroadcasterContract))]
	public interface IEventBroadcaster
	{
		/// <summary>
		/// Adds event's subscription. 
		/// </summary>
		/// <param name="key">The identifier of the subscribed event.</param>
		/// <param name="handler">The event handler.</param>
		/// <returns>The object for subscription's stop.</returns>
		IDisposable Subscribe(EventKey key, Action handler);

		/// <summary>
		/// Adds event's subscription.
		/// </summary>
		/// <typeparam name="TEventArgs">Тип объекта, содержащего аргументы события.</typeparam>
		/// <param name="key">The identifier of the subscribed event.</param>
		/// <param name="handler">The event handler.</param>
		/// <returns>The object for subscription's stop.</returns>
		IDisposable Subscribe<TEventArgs>(EventKey<TEventArgs> key, Action<TEventArgs> handler);
		
		/// <summary>
		/// Sends the argumentless event.
		/// </summary>
		/// <param name="key">The identifier of the event being sent.</param>
		void Send(EventKey key);

		/// <summary>
		/// Sends the event with arguments.
		/// </summary>
		/// <typeparam name="TEventArgs">The type of the object with event's arguments.</typeparam>
		/// <param name="key">The identifier of the event being sent.</param>
		/// <param name="args">The object with event's arguments.</param>
		void Send<TEventArgs>(EventKey<TEventArgs> key, TEventArgs args);
	}
}
