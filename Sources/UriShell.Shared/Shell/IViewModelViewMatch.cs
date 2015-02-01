using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Результат поиска объекта, реализующего представление по заданной модели.
	/// </summary>
	[ContractClass(typeof(IViewModelViewMatchContract))]
	public interface IViewModelViewMatch
	{
		/// <summary>
		/// Возвращает найденное представление.
		/// </summary>
		[Pure]
		object View
		{
			get;
		}

		/// <summary>
		/// Указывает, что найденное представление поддерживает присваивание
		/// другой модели.
		/// </summary>
		[Pure]
		bool SupportsModelChange
		{
			get;
		}

		/// <summary>
		/// Проверяет, реализует ли объект представление по заданной модели.
		/// </summary>
		/// <param name="viewModel">Модель представления для проверки.</param>
		/// <returns>true, если объект реализует представление по заданной модели;
		/// иначе false.</returns>
		[Pure]
		bool IsMatchToModel(object viewModel);

		/// <summary>
		/// Присваивает представлению заданную модель.
		/// </summary>
		/// <param name="viewModel">Модель представления для присваивания.</param>
		void ChangeModel(object viewModel);
	}
}
