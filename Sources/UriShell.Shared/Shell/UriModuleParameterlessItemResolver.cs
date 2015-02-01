using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Реализует <see cref="IUriModuleItemResolver"/> создавая объект с помощью фабрики
	/// заданного типа, безотносительно к URI.
	/// </summary>
	/// <typeparam name="T">Тип создаваемого объекта.</typeparam>
	public sealed class UriModuleParameterlessItemResolver<T> : IUriModuleItemResolver
	{
		/// <summary>
		/// Фабрика для создания объекта.
		/// </summary>
		private readonly Func<T> _factory;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="UriModuleParameterlessItemResolver{T}"/>.
		/// </summary>
		/// <param name="factory">Фабрика для создания объекта.</param>
		public UriModuleParameterlessItemResolver(Func<T> factory)
		{
			Contract.Requires<ArgumentNullException>(factory != null);

			this._factory = factory;
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
			return this._factory();
		}
	}
}
