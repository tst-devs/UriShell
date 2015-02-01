using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace UriShell.Shell
{
	/// <summary>
	/// Результат поиска объекта, реализующего представление по заданной модели,
	/// использующего для присваивания модели параметр конструктора.
	/// </summary>
	internal sealed class ViewModelParameterMatch : IViewModelViewMatch
	{
		/// <summary>
		/// Пробует определить соответствие одного из параметров конструктора заданного
		/// типа заданной модели представления.
		/// </summary>
		/// <param name="viewModel">Модель искомого представления.</param>
		/// <param name="viewType">Тип объекта, проверяемого на соответствие параметра конструктора.</param>
		/// <param name="viewFactory">Фабрика представления, принимающая на вход информацию о соответствующем
		/// параметре, вызываемая в случае соответствия.</param>
		/// <returns>Результат поиска объекта, реализующего представление по заданной модели,
		/// или null, если ни одного из свойств заданного типа не подходит для заданной модели.</returns>
		public static ViewModelParameterMatch TryMatch(object viewModel, Type viewType, Func<ParameterInfo, object> viewFactory)
		{
			Contract.Requires<ArgumentNullException>(viewModel != null);
			Contract.Requires<ArgumentNullException>(viewType != null);
			Contract.Requires<ArgumentNullException>(viewFactory != null);

			var viewModelParameter = viewType
				.GetConstructors()
				.SelectMany(c => c.GetParameters())
				.FirstOrDefault(pi => pi.IsDefined(typeof(ViewModelAttribute), false));

			if (viewModelParameter == null)
			{
				return null;
			}

			if (!viewModelParameter.ParameterType.IsInstanceOfType(viewModel))
			{
				return null;
			}

			return new ViewModelParameterMatch(viewFactory(viewModelParameter));
		}

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ViewModelPropertyMatch"/>.
		/// </summary>
		/// <param name="view">Найденное представление.</param>
		private ViewModelParameterMatch(object view)
		{
			this.View = view;
		}

		/// <summary>
		/// Возвращает найденное представление.
		/// </summary>
		public object View
		{
			get;
			private set;
		}

		/// <summary>
		/// Указывает, что найденное представление поддерживает присваивание
		/// другой модели.
		/// </summary>
		public bool SupportsModelChange
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Проверяет, реализует ли объект представление по заданной модели.
		/// </summary>
		/// <param name="viewModel">Модель представления для проверки.</param>
		/// <returns>true, если объект реализует представление по заданной модели;
		/// иначе false.</returns>
		public bool IsMatchToModel(object viewModel)
		{
			return false;
		}

		/// <summary>
		/// Присваивает представлению заданную модель.
		/// </summary>
		/// <param name="viewModel">Модель представления для присваивания.</param>
		public void ChangeModel(object viewModel)
		{
			throw new NotSupportedException();
		}
	}
}
