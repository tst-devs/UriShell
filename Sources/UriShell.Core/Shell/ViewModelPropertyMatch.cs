using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace UriShell.Shell
{
	/// <summary>
	/// Результат поиска объекта, реализующего представление по заданной модели,
	/// использующий для замены модели заданный <see cref="PropertyInfo"/>.
	/// </summary>
	internal sealed class ViewModelPropertyMatch : IViewModelViewMatch
	{
		/// <summary>
		/// Пробует определить соответствие одного из свойств заданного типа,
		/// указывающего на модель представления, заданной модели представления.
		/// </summary>
		/// <param name="viewModel">Модель искомого представления.</param>
		/// <param name="viewType">Тип объекта, проверяемого на соответствие свойств.</param>
		/// <param name="viewFactory">Фабрика представления, вызываемая в случае соответствия.</param>
		/// <returns>Результат поиска объекта, реализующего представление по заданной модели,
		/// или null, если ни одного из свойств заданного типа не подходит для заданной модели.</returns>
		public static ViewModelPropertyMatch TryMatch(object viewModel, Type viewType, Func<object> viewFactory)
		{
			Contract.Requires<ArgumentNullException>(viewModel != null);
			Contract.Requires<ArgumentNullException>(viewType != null);
			Contract.Requires<ArgumentNullException>(viewFactory != null);

			var viewModelProperty = viewType
				.GetProperties()
				.Where(pi => pi.IsDefined(typeof(ViewModelAttribute), false))
				.FirstOrDefault(pi => ViewModelPropertyMatch.IsPropertyMatchToModel(pi, viewModel));

			if (viewModelProperty == null)
			{
				return null;
			}

			// Создаем представление и задаем ему модель.
			var match = new ViewModelPropertyMatch(viewFactory(), viewModelProperty);
			match.ChangeModel(viewModel);

			return match;
		}

		/// <summary>
		/// Проверяет, реализует ли объект с заданным свойством представление по заданной модели.
		/// </summary>
		/// <param name="viewModelProperty">Свойство для проверки совместимости с заданной моделью.</param>
		/// <param name="viewModel">Модель представления для проверки.</param>
		/// <returns>true, если объект с заданным свойством реализует представление по заданной модели;
		/// иначе false.</returns>
		private static bool IsPropertyMatchToModel(PropertyInfo viewModelProperty, object viewModel)
		{
			if (!viewModelProperty.CanWrite)
			{
				return false;
			}

			return viewModelProperty.PropertyType.IsInstanceOfType(viewModel);
		}

		/// <summary>
		/// Найденное представление, имеющее свойство с моделью представления.
		/// </summary>
		private readonly object _view;

		/// <summary>
		/// <see cref="PropertyInfo"/>, указывающее на свойство с моделью представления.
		/// </summary>
		private readonly PropertyInfo _viewModelProperty;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ViewModelPropertyMatch"/>.
		/// </summary>
		/// <param name="view">Найденное представление, имеющее свойство с моделью
		/// представления.</param>
		/// <param name="viewModelProperty"><see cref="PropertyInfo"/>, указывающее
		/// на свойство с моделью представления.</param>
		private ViewModelPropertyMatch(object view, PropertyInfo viewModelProperty)
		{
			this._view = view;
			this._viewModelProperty = viewModelProperty;
		}

		/// <summary>
		/// Возвращает найденное представление.
		/// </summary>
		public object View
		{
			get
			{
				return this._view;
			}
		}

		/// <summary>
		/// Указывает, что найденное представление поддерживает присваивание
		/// другой модели.
		/// </summary>
		public bool SupportsModelChange
		{
			get
			{
				return true;
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
			return ViewModelPropertyMatch.IsPropertyMatchToModel(this._viewModelProperty, viewModel);
		}

		/// <summary>
		/// Присваивает представлению заданную модель.
		/// </summary>
		/// <param name="viewModel">Модель представления для присваивания.</param>
		public void ChangeModel(object viewModel)
		{
			this._viewModelProperty.SetValue(this._view, viewModel, new object[0]);
		}
	}
}
