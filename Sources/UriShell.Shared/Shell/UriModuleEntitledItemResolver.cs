using System;
using System.Linq;

namespace UriShell.Shell
{
	/// <summary>
	/// Реализует <see cref="IUriModuleItemResolver"/> создавая объект с помощью фабрики
	/// заданного типа, принимающей на вход параметр title, указанный в URI.
	/// </summary>
	/// <typeparam name="T">Тип создаваемого объекта.</typeparam>
	public sealed class UriModuleEntitledItemResolver<T> : IUriModuleItemResolver
	{
		/// <summary>
		/// Фабрика для создания объекта.
		/// </summary>
		private readonly Func<string, T> _entitledItemFactory;

		/// <summary>
		/// Инициализирует новый объект <see cref="UriModuleEntitledItemResolver{T}"/>.
		/// </summary>
		/// <param name="entitledItemFactory">Фабрика для создания объекта.</param>
		public UriModuleEntitledItemResolver(Func<string, T> entitledItemFactory)
		{
			this._entitledItemFactory = entitledItemFactory;
		}

		/// <summary>
		/// Создает объект по заданному URI.
		/// </summary>
		/// <param name="uri">URI, по которому требуется создать объект.</param>
		/// <param name="attachmentSelector">Селектор, предоставляющий доступ к объектам,
		/// прикрепленным к URI через параметры.</param>
		/// <returns>Объект, созданный по заданному URI.</returns>
		public object Resolve(Uri uri, UriAttachmentSelector attachmentSelector)
		{
			var uriBuilder = new PhoenixUriBuilder(uri);
			var title = uriBuilder.Parameters["title"] ?? uri.ToString();

			return this._entitledItemFactory(title);	
		}
	}
}
