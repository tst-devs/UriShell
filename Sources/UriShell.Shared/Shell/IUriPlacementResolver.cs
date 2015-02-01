using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Интерфейс сервиса, определяющего размещение объекта по заданному URI.
	/// </summary>
	[ContractClass(typeof(IUriPlacementResolverContract))]
	public interface IUriPlacementResolver
	{
		/// <summary>
		/// Определяет размещение объекта по заданному URI.
		/// </summary>
		/// <param name="resolved">Объект для размещения по заданному URI.</param>
		/// <param name="uri">URI, по которому требуется разместить объект.</param>
		/// <param name="attachmentSelector">Селектор, предоставляющий доступ к объектам,
		/// прикрепленным к URI через параметры.</param>
		/// <returns><see cref="IUriPlacementConnector"/>, позволяющий присоединить объект
		/// к пользовательскому интерфейсу, если размещение было определено; иначе null.</returns>
		IUriPlacementConnector Resolve(object resolved, Uri uri, UriAttachmentSelector attachmentSelector);
	}
}
