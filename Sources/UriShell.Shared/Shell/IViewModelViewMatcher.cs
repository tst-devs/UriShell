using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Интерфейс сервиса поиска объектов, реализующих представление по заданной модели.
	/// </summary>
	[ContractClass(typeof(IViewModelViewMatcherContract))]
	public interface IViewModelViewMatcher
	{
		/// <summary>
		/// Выполняет поиск объекта, реализующего представление по заданной модели.
		/// </summary>
		/// <param name="viewModel">Модель искомого представления.</param>
		/// <returns>Результат поиска, который в случае успеха содержит экземпляр
		/// <see cref="IViewModelViewMatch"/>, или null, когда ничего не найдено.</returns>
		IViewModelViewMatch Match(object viewModel);
	}
}
