using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Содержит объекты, открытые оболочкой через URI, и их метаданные.
	/// </summary>
	internal sealed class UriResolvedObjectHolder : IUriResolvedObjectHolder
	{
		/// <summary>
		/// Объекты, открытые оболочкой через URI, и их метаданными.
		/// </summary>
		private readonly Dictionary<object, UriResolvedMetadata> _data = new Dictionary<object, UriResolvedMetadata>();

		/// <summary>
		/// Содержит идентификаторы объектов, используемые в настоящий момент.
		/// </summary>
		private readonly HashSet<int> _usedIds = new HashSet<int>();

		/// <summary>
		/// Генератор идентификаторов объектов.
		/// </summary>
		private readonly Random _random = new Random(PhoenixUriBuilder.MinResolvedId);

		/// <summary>
		/// Генерирует новый уникальный идентификатор объекта.
		/// </summary>
		/// <returns>Значение сгенерированного идентификатора.</returns>
		private int GenerateNewId()
		{
			if (this._usedIds.Count > PhoenixUriBuilder.MaxResolvedId - PhoenixUriBuilder.MinResolvedId)
			{
				throw new InvalidOperationException(Properties.Resources.NoAvailableUriResolutionId);
			}

			int id;
			do
			{
				id = this._random.Next(PhoenixUriBuilder.MaxResolvedId + 1);
			}
			while (!this._usedIds.Add(id));

			return id;
		}

		/// <summary>
		/// Добавляет объект, открытый оболочкой через URI.
		/// </summary>
		/// <param name="resolved">Объект для добавления.</param>
		/// <param name="metadata">Метаданные добавляемого объекта.</param>
		public void Add(object resolved, UriResolvedMetadata metadata)
		{
			try
			{
				this._data.Add(resolved, metadata.AssignId(this.GenerateNewId()));
			}
			catch (ArgumentException)
			{
				throw new ArgumentException(
					string.Format(Properties.Resources.UriResolvedObjectAlreadyExists, resolved, typeof(IUriResolvedObjectHolder).Name),
					"resolved");
			}
		}

		/// <summary>
		/// Удаляет объект, открытый оболочкой через URI.
		/// </summary>
		/// <param name="resolved">Объект для удаления.</param>
		public void Remove(object resolved)
		{
			UriResolvedMetadata metadata;

			if (this._data.TryGetValue(resolved, out metadata))
			{
				this._data.Remove(resolved);
				this._usedIds.Remove(metadata.ResolvedId);
			}
		}

		/// <summary>
		/// Проверяет наличие объекта в холдере.
		/// </summary>
		/// <param name="resolved">Объект, проверяемый на наличие в холдере.</param>
		/// <returns>true, если объект содержится в холдере; иначе false.</returns>
		public bool Contains(object resolved)
		{
			if (resolved == null)
			{
				return false;
			}

			if (this._data.ContainsKey(resolved))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Возвращает заданный объект, находящийся в холдере, по его идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор запрашиваемого объекта.</param>
		/// <returns>Объект с заданным идентификатором.</returns>
		public object Get(int id)
		{
			foreach (var item in this._data)
			{
				if (item.Value.ResolvedId == id)
				{
					return item.Key;
				}
			}

			throw new ArgumentOutOfRangeException(
				"id",
				string.Format(Properties.Resources.UriResolvedObjectIdDoesntExist, id, typeof(IUriResolvedObjectHolder).Name));
		}

		/// <summary>
		/// Возвращает метаданные заданного объекта, находящегося в холдере.
		/// </summary>
		/// <param name="resolved">Объект, для которого запрашиваются метаданные.</param>
		/// <returns>Метаданные заданного объекта.</returns>
		public UriResolvedMetadata GetMetadata(object resolved)
		{
			UriResolvedMetadata metadata;

			if (this._data.TryGetValue(resolved, out metadata))
			{
				return metadata;
			}

			throw new ArgumentOutOfRangeException(
				"resolved",
				string.Format(Properties.Resources.UriResolvedObjectDoesntExist, resolved, typeof(IUriResolvedObjectHolder).Name));
		}

		/// <summary>
		/// Записывает новые метаданные заданного объекта, полученные путем замены URI.
		/// </summary>
		/// <param name="resolved">Объект, для которого записываются метаданные.</param>
		/// <param name="overrideUri">Новый URI для записи в метаданные.</param>
		public void ModifyMetadata(object resolved, Uri overrideUri)
		{
			throw new NotImplementedException();
		}

		#region IEnumerable<object> Members

		/// <summary>
		/// Возвращает итератор для обхода объектов, открытых оболочкой через URI.
		/// </summary>
		/// <returns>Итератор для обхода объектов, открытых оболочкой через URI.</returns>
		public IEnumerator<object> GetEnumerator()
		{
			return this._data.Keys.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Возвращает итератор для обхода объектов, открытых оболочкой через URI.
		/// </summary>
		/// <returns>Итератор для обхода объектов, открытых оболочкой через URI.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}