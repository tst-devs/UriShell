using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Text;
using System.Text.RegularExpressions;

using UriShell.Extensions;

namespace UriShell.Shell
{
	partial class ShellUriBuilder
	{
		/// <summary>
		/// Позволяет создать URI, для открытия объектов в Фениксе, в виде последовательности его компонентов.
		/// </summary>
		public sealed class Writer
		{
			/// <summary>
			/// <see cref="ShellUriBuilder"/> для настройки компонентов URI.
			/// </summary>
			private readonly ShellUriBuilder _builder = new ShellUriBuilder();

			/// <summary>
			/// Инициализирует новый объект класса <see cref="Writer"/>.
			/// </summary>
			internal Writer()
			{
			}

			/// <summary>
			/// Задает компонент, определяющий размещение.
			/// </summary>
			/// <param name="placement">Компонент, определяющий размещение.</param>
			/// <returns>Объект для построения URI в виде последовательности его компонентов.</returns>
			public Writer Placement(string placement)
			{
				this._builder.Placement = placement;

				return this;
			}

			/// <summary>
			/// Задает компонент, уточняющий размещение идентификатором объекта,
			/// к которому относится объект, указанный в URI.
			/// </summary>
			/// <param name="ownerId">Компонент, уточняющий размещение идентификатором объекта.</param>
			/// <returns>Объект для построения URI в виде последовательности его компонентов.</returns>
			public Writer OwnerId(int ownerId)
			{
				this._builder.OwnerId = ownerId;

				return this;
			}

			/// <summary>
			/// Задает компонент, определяющий модуль, который содержит объект.
			/// </summary>
			/// <param name="module">Компонент, определяющий модуль, который содержит объект.</param>
			/// <returns>Объект для построения URI в виде последовательности его компонентов.</returns>
			public Writer Module(string module)
			{
				this._builder.Module = module;

				return this;
			}

			/// <summary>
			/// Задает компонент, определяющий название объекта.
			/// </summary>
			/// <param name="item">Компонент, определяющий название объекта.</param>
			/// <returns>Объект для построения URI в виде последовательности его компонентов.</returns>
			public Writer Item(string item)
			{
				this._builder.Item = item;

				return this;
			}

			/// <summary>
			/// Добавляет параметр, передаваемый через URI.
			/// </summary>
			/// <param name="name">Наименование параметра, передаваемого через URI.</param>
			/// <param name="value">Значение параметра, передаваемого через URI.</param>
			/// <returns>Объект для построения URI в виде последовательности его компонентов.</returns>
			public Writer Parameter(string name, string value)
			{
				this._builder.Parameters.Set(name, value);

				return this;
			}

			/// <summary>
			/// Добавляет параметр со ссылкой на прикрепляемый объект, передаваемый через URI.
			/// </summary>
			/// <param name="name">Наименование параметра, передаваемого через URI.</param>
			/// <param name="index">Индекс прикрепляемого к URI объекта.</param>
			/// <returns>Объект для построения URI в виде последовательности его компонентов.</returns>
			public Writer Attachment(string name, int index)
			{
				this._builder.Parameters.Set(name, string.Format("{{{0}}}", index));

				return this;
			}

			/// <summary>
			/// Возвращает URI с заданными ранее компонентами.
			/// </summary>
			/// <returns>Объект <see cref="Uri" />, содержащий построенный URI.</returns>
			public Uri End()
			{
				return this._builder.Uri;
			}
		}
	}
}