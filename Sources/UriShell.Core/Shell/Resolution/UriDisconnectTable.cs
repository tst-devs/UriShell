using System.Collections.Generic;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Реализация таблицы отсоединения объектов от пользовательского интерфейса.
	/// </summary>
	internal sealed class UriDisconnectTable : IUriDisconnectTable
	{
		/// <summary>
		/// Словарь, в котором каждому объекту сопоставлен <see cref="IUriPlacementConnector"/>,
		/// которые отсоединяют его от пользовательского интерфейса.
		/// </summary>
		private readonly Dictionary<object, IUriPlacementConnector> _connectors = new Dictionary<object, IUriPlacementConnector>();

		/// <summary>
		/// Возвращает или присваивает <see cref="IUriPlacementConnector"/>, который отсоединяет
		/// объект от пользовательского интерфейса.
		/// </summary>
		/// <param name="resolved">Объект, для которого запрашивается или задается значение.</param>
		public IUriPlacementConnector this[object resolved]
		{
			get
			{
				IUriPlacementConnector connector;
				if (this._connectors.TryGetValue(resolved, out connector))
				{
					return connector;
				}

				throw new KeyNotFoundException(string.Format(
					Properties.Resources.UriResolvedNotRegisteredForDisconnect,
					resolved));
			}
			set
			{
				this._connectors[resolved] = value;
			}
		}

		/// <summary>
		/// Удаляет запись об отсоединении заданного объекта.
		/// </summary>
		/// <param name="resolved">Объект, для которого удаляется запись.</param>
		public void Remove(object resolved)
		{
			if (!this._connectors.Remove(resolved))
			{
				throw new KeyNotFoundException(string.Format(
					Properties.Resources.UriResolvedNotRegisteredForDisconnect,
					resolved));
			}
		}
	}
}
