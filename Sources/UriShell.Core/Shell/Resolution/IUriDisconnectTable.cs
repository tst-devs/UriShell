using System.Diagnostics.Contracts;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Таблица отсоединения объектов от пользовательского интерфейса.
	/// </summary>
	[ContractClass(typeof(IUriDisconnectTableContract))]
	internal interface IUriDisconnectTable
	{
		/// <summary>
		/// Возвращает или присваивает <see cref="IUriPlacementConnector"/>, который отсоединяет
		/// объект от пользовательского интерфейса.
		/// </summary>
		/// <param name="resolved">Объект, для которого запрашивается или задается значение.</param>
		IUriPlacementConnector this[object resolved]
		{
			get;
			set;
		}

		/// <summary>
		/// Удаляет запись об отсоединении заданного объекта.
		/// </summary>
		/// <param name="resolved">Объект, для которого удаляется запись.</param>
		void Remove(object resolved);
	}
}
