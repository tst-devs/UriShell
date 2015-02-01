using System.Diagnostics.Contracts;

namespace UriShell.Shell.Connectors
{
	/// <summary>
	/// Интерфейс объекта, предоставляющего ключи, по которым определяется
	/// идентичность объектов, показываемых в SidebarView.
	/// </summary>
	[ContractClass(typeof(ISidebarPlacementKeySourceContract))]
	public interface ISidebarPlacementKeySource
	{
		/// <summary>
		/// Возвращает ключ для заданного объекта, присоединенного к SidebarView.
		/// </summary>
		/// <param name="connected">Объект, для которого запрашивается ключ.</param>
		/// <returns>Ключ заданного объекта или null, если объект не обладает ключом.</returns>
		object GetKey(object connected);
	}
}
