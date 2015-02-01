using System;
using System.Diagnostics.Contracts;
using UriShell.Shell.Registration;

namespace UriShell.Shell
{
	/// <summary>
	/// Интерфейс оболочки АРМа.
	/// </summary>
	[ContractClass(typeof(IShellContract))]
	public interface IShell
	{
		/// <summary>
		/// Добавляет в оболочку заданный <see cref="IUriPlacementResolver"/>.
		/// </summary>
		/// <param name="uriPlacementResolver"><see cref="IUriPlacementResolver"/>
		/// для добавления с использованием слабой ссылки.</param>
		void AddUriPlacementResolver(IUriPlacementResolver uriPlacementResolver);

		/// <summary>
		/// Начинает процесс открытия заданного URI.
		/// </summary>
		/// <param name="uri">URI, который требуется открыть.</param>
		/// <param name="attachments">Список объектов, прикрепляемых к URI с помощью идентификаторов.</param>
		/// <returns>Сервис, позволяющий настроить и открыть объект, полученный через URI.</returns>
		IShellResolve Resolve(Uri uri, params object[] attachments);

		/// <summary>
		/// Проверяет, открыт ли заданный объект.
		/// </summary>
		/// <param name="resolved">Объект, для проверки, открыт ли он.</param>
		/// <returns>true, если объект открыт; иначе false.</returns>
		[Pure]
		bool IsResolvedOpen(object resolved);

		/// <summary>
		/// Возвращает идентификатор заданного объекта, открытого через URI.
		/// </summary>
		/// <param name="resolved">Объект, для которого запрашивается идентификатор.</param>
		/// <returns>Идентификатор заданного объекта.</returns>
		[Pure]
		int GetResolvedId(object resolved);

		/// <summary>
		/// Возвращает URI заданного открытого объекта.
		/// </summary>
		/// <param name="resolved">Объект, для которого запрашивается URI.</param>
		/// <returns>URI заданного объекта.</returns>
		[Pure]
		Uri GetResolvedUri(object resolved);

		/// <summary>
		/// Закрывает указанный объект.
		/// </summary>
		/// <param name="resolved">Объект, который необходимо закрыть.</param>
		void CloseResolved(object resolved);

		/// <summary>
		/// Пробует создать гиперссылку из текстового описания.
		/// </summary>
		/// <param name="hyperlink">Текстовое описание гиперссылки в формате гиперссылки HTML.</param>
		/// <param name="ownerId">Идентификатор объекта, к которому относится объект, получаемый
		/// при открытии гиперссылки.</param>
		/// <returns>Гиперссылку, если указанное текстовое описание гиперссылки верно; иначе null.</returns>
		[Pure]
		PhoenixHyperlink TryParseHyperlink(string hyperlink, int ownerId);

		/// <summary>
		/// Создает гиперссылку на основе указанного <see cref="Uri"/>. 
		/// </summary>
		/// <param name="uri">URI, для которого создается гиперссылка.</param>
		/// <returns>Гиперссылку, созданную на основе указанного <see cref="Uri"/>.</returns>
		[Pure]
		PhoenixHyperlink CreateHyperlink(Uri uri);
	}
}
