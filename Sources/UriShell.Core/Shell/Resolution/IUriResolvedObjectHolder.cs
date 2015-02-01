using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Интерфейс холдера объектов, открытых оболочкой через URI, и их метаданных.
	/// </summary>
	[ContractClass(typeof(IUriResolvedObjectHolderContract))]
	internal interface IUriResolvedObjectHolder : IEnumerable<object>
	{
		/// <summary>
		/// Добавляет объект, открытый оболочкой через URI.
		/// </summary>
		/// <param name="resolved">Объект для добавления.</param>
		/// <param name="metadata">Метаданные добавляемого объекта.</param>
		void Add(object resolved, UriResolvedMetadata metadata);

		/// <summary>
		/// Удаляет объект, открытый оболочкой через URI.
		/// </summary>
		/// <param name="resolved">Объект для удаления.</param>
		void Remove(object resolved);

		/// <summary>
		/// Проверяет наличие объекта в холдере.
		/// </summary>
		/// <param name="resolved">Объект, проверяемый на наличие в холдере.</param>
		/// <returns>true, если объект содержится в холдере; иначе false.</returns>
		[Pure]
		bool Contains(object resolved);

		/// <summary>
		/// Возвращает заданный объект, находящийся в холдере, по его идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор запрашиваемого объекта.</param>
		/// <returns>Объект с заданным идентификатором.</returns>
		[Pure]
		object Get(int id);

		/// <summary>
		/// Возвращает метаданные заданного объекта, находящегося в холдере.
		/// </summary>
		/// <param name="resolved">Объект, для которого запрашиваются метаданные.</param>
		/// <returns>Метаданные заданного объекта.</returns>
		[Pure]
		UriResolvedMetadata GetMetadata(object resolved);

		/// <summary>
		/// Записывает новые метаданные заданного объекта, полученные путем замены URI.
		/// </summary>
		/// <param name="resolved">Объект, для которого записываются метаданные.</param>
		/// <param name="overrideUri">Новый URI для записи в метаданные.</param>
		void ModifyMetadata(object resolved, Uri overrideUri);
	}
}
