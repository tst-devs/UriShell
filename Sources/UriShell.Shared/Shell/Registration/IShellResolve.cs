using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace UriShell.Shell.Registration
{
	/// <summary>
	/// Позволяет использовать объект, полученный через URI.
	/// </summary>
	[ContractClass(typeof(IShellResolveContract))]
	public interface IShellResolve : IShellResolveOpen
	{
		/// <summary>
		/// Позволяет настроить объект, полученный через URI, если его тип совместим с заданным.
		/// </summary>
		/// <typeparam name="TResolved">Тип объекта, который ожидается от URI.</typeparam>
		/// <returns>Сервис для настройки объекта.</returns>
		IShellResolveSetup<TResolved> Setup<TResolved>();
	}
}
