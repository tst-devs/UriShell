using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Интерфейс сервиса, умеющего создавать объект по заданному URI.
	/// </summary>
	[ContractClass(typeof(IUriModuleItemResolverContract))]
	public interface IUriModuleItemResolver
	{
		/// <summary>
		/// Создает объект по заданному URI.
		/// </summary>
		/// <param name="uri">URI, по которому требуется создать объект.</param>
		/// <param name="attachmentSelector">Селектор, предоставляющий доступ к объектам,
		/// прикрепленным к URI через параметры.</param>
		/// <returns>Объект, созданный по заданному URI.</returns>
		object Resolve(Uri uri, UriAttachmentSelector attachmentSelector);
	}
}
