using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Представляет собой идентификатор записи истории активности
	/// представлений объектов в SidebarView.
	/// </summary>
	[Obsolete("Not sure if this class should be common")]
	internal sealed class SidebarActivityHistoryId
	{
		/// <summary>
		/// Ключи объектов, для которых сделана запись.
		/// </summary>
		public readonly IEnumerable<object> _connectedKeys;

		/// <summary>
		/// Рассчитанный хэш-код записи.
		/// </summary>
		private readonly int _hashCode;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="SidebarActivityHistoryId"/>.
		/// </summary>
		public SidebarActivityHistoryId(IEnumerable<object> connectedKeys)
		{
			Contract.Requires<ArgumentNullException>(connectedKeys != null);
			
			var connectedKeysArray = connectedKeys.ToArray();
			Array.Sort(connectedKeysArray);

			this._connectedKeys = connectedKeysArray;
			this._hashCode = this._connectedKeys.Aggregate(0, (hash, key) => key.GetHashCode() ^ hash);
		}

		/// <summary>
		/// Определяет, является ли заданный объект равным по значению текущему.
		/// </summary>
		/// <param name="obj">Объект для сравнения с текущим.</param>
		/// <returns><see langword="true"/>, если заданный объект равен по значению текущему; иначе <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			var otherId = obj as SidebarActivityHistoryId;
			if (otherId == null)
			{
				return false;
			}

			return otherId._connectedKeys.SequenceEqual(this._connectedKeys);
		}

		/// <summary>
		/// Возвращает хэш-код данного <see cref="SidebarActivityHistoryId"/>.
		/// </summary>
		/// <returns>Хэш-код данного <see cref="SidebarActivityHistoryId"/>.</returns>
		public override int GetHashCode()
		{
			return this._hashCode;
		}

		/// <summary>
		/// Возвращает строковое представление данного объекта.
		/// </summary>
		/// <returns>Строковое представление данного объекта.</returns>
		public override string ToString()
		{
			return string.Join(", ", this._connectedKeys);
		}
	}
}