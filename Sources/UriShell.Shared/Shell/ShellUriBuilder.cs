using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Text;

using UriShell.Extensions;

namespace UriShell.Shell
{
	/// <summary>
	/// Позволяет считывать и настраивать компоненты <see cref="Uri"/> для открытия объектов в Фениксе.
	/// </summary>
	public sealed partial class ShellUriBuilder
	{
		/// <summary>
		/// Минимально допустимое значение идентификаторов объектов, открытых через URI.
        /// </summary>
        public static readonly int MinResolvedId = 0;

        /// <summary>
        /// Максимально допустимое значение идентификаторов объектов, открытых через URI.
        /// </summary>
        public static readonly int MaxResolvedId = ushort.MaxValue;

		/// <summary>
		/// Компонент, определяющий размещение.
		/// </summary>
		private string _placement = string.Empty;

		/// <summary>
		/// Компонент, уточняющий размещение идентификатором объекта,
		/// к которому относится объект, указанный в URI.
		/// </summary>
		private int _ownerId = 0;

		/// <summary>
		/// Компонент, определяющий модуль, который содержит объект.
		/// </summary>
		private string _module = string.Empty;

		/// <summary>
		/// Компонент, определяющий название объекта.
		/// </summary>
		private string _item = string.Empty;

		/// <summary>
		/// Коллекция параметров, передаваемых через URI.
		/// </summary>
		private readonly NameValueCollection _parameters = new NameValueCollection();

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ShellUriBuilder"/>.
		/// </summary>
		public ShellUriBuilder()
		{
		}

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ShellUriBuilder"/>.
		/// </summary>
		/// <param name="uri"><see cref="Uri"/>, содержимое которой будет использовано
		/// для инициализации свойств создаваемого <see cref="ShellUriBuilder"/>.</param>
		public ShellUriBuilder(Uri uri)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentOutOfRangeException>(uri.IsUriShell());

			this.Placement = uri.Host;
			if (!uri.IsDefaultPort)
			{
				this.OwnerId = uri.Port;
			}

			var path = uri.AbsolutePath.Substring(1);
			var moduleItemDelim = path.IndexOf('/');
			if (moduleItemDelim >= 0)
			{
				this.Module = path.Substring(0, moduleItemDelim);
				this.Item = path.Substring(moduleItemDelim + 1).TrimEnd('/');
			}

			this.ParametersFromUriQuery(uri.Query);
		}

		/// <summary>
		/// Задает инвариант класса.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.Placement != null);
			Contract.Invariant(this.Module != null);
			Contract.Invariant(this.Item != null);
			Contract.Invariant(this.Parameters != null);
			
			Contract.Invariant(this.OwnerId >= ShellUriBuilder.MinResolvedId);
			Contract.Invariant(this.OwnerId <= ShellUriBuilder.MaxResolvedId);
		}

		/// <summary>
		/// Начинает построение URI в виде последовательности его компонентов.
		/// </summary>
		/// <returns>Объект для построения URI в виде последовательности его компонентов.</returns>
		public static Writer StartUri()
		{
			return new Writer();
		}

		/// <summary>
		/// Заполняет свойство <see cref="Parameters"/> по строке запроса URI.
		/// </summary>
		/// <param name="query">Строка запроса URI.</param>
		private void ParametersFromUriQuery(string query)
		{
			// Содержимое метода основано на коде System.Web.HttpValueCollection.FillFromString,
			// который находится в сборке System.Web, отсутствующей в .Net Client Profile.

			if (query.Length == 0)
			{
				return;
			}

			var i = 1; // Запросная строка URI всегда начинается с '?'.

			while (i < query.Length)
			{
				// Определяем позицию ближайшего символа '&',
				// попутно запоминая позицию встретившегося символа '='.

				var si = i;
				var ti = -1;

				while (i < query.Length)
				{
					var ch = query[i];

					if (ch == '&')
					{
						break;
					}

					if (ch == '=' && ti < 0)
					{
						ti = i;
					}

					i++;
				}

				// Извлекаем и добавляем пару ключ-значение.

				string name = null;
				string value;

				if (ti >= 0)
				{
					name = query.Substring(si, ti - si);
					value = query.Substring(ti + 1, i - ti - 1);
				}
				else
				{
					value = query.Substring(si, i - si);
				}

				this._parameters.Add(name, Uri.UnescapeDataString(value));

				// Учитываем возможный пустой параметр,
				// заданный амперсандом в конце URI.

				if (i == query.Length - 1 && query[i] == '&')
				{
					this._parameters.Add(null, string.Empty);
				}

				i++;
			}
		}

		/// <summary>
		/// Собирает строку запроса URI по содержимому свойства <see cref="Parameters"/>.
		/// </summary>
		/// <returns>Строку запроса URI.</returns>
		private string ParametersToUriQuery()
		{
			if (this._parameters.Count == 0)
			{
				return string.Empty;
			}

			var sb = new StringBuilder();
			foreach (string key in this._parameters.Keys)
			{
				if (sb.Length > 0)
				{
					sb.Append('&');
				}
				sb.AppendFormat("{0}={1}", key, Uri.EscapeDataString(this._parameters[key]));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Возвращает URI, построенный при помощи данного <see cref="ShellUriBuilder" />.
		/// </summary>
		public Uri Uri
		{
			get
			{
				var ub = new UriBuilder(
					Settings.Instance.Scheme,
					this.Placement,
					this.OwnerId, 
					string.Format("/{0}/{1}", this.Module, this.Item));
				ub.Query = this.ParametersToUriQuery();

				return ub.Uri;
			}
		}

		/// <summary>
		/// Гарантирует, что заданное значение строки, если оно null, будет заменено пустой строкой.
		/// </summary>
		/// <param name="value">Строка, которую нужно проверить на null.</param>
		private static void ProtectFromNull(ref string value)
		{
			if (value == null)
			{
				value = string.Empty;
			}
		}

		/// <summary>
		/// Возвращает компонент, определяющий размещение.
		/// </summary>
		public string Placement
		{
			get
			{
				return this._placement;
			}
			set
			{
				ShellUriBuilder.ProtectFromNull(ref value);
				this._placement = value;
			}
		}

		/// <summary>
		/// Возвращает компонент, уточняющий размещение идентификатором
		/// объекта, к которому относится объект, указанный в URI.
		/// </summary>
		public int OwnerId
		{
			get
			{
				return this._ownerId;
			}
			set
			{
				Contract.Requires<ArgumentOutOfRangeException>(value >= ShellUriBuilder.MinResolvedId);
				Contract.Requires<ArgumentOutOfRangeException>(value <= ShellUriBuilder.MaxResolvedId);

				this._ownerId = value;
			}
		}

		/// <summary>
		/// Возвращает компонент, определяющий модуль, который содержит объект.
		/// </summary>
		public string Module
		{
			get
			{
				return this._module;
			}
			set
			{
				ShellUriBuilder.ProtectFromNull(ref value);
				this._module = value;
			}
		}

		/// <summary>
		/// Возвращает компонент, определяющий название объекта.
		/// </summary>
		public string Item
		{
			get
			{
				return this._item;
			}
			set
			{
				ShellUriBuilder.ProtectFromNull(ref value);
				this._item = value;
			}
		}

		/// <summary>
		/// Возвращает коллекцию параметров, передаваемых через URI.
		/// </summary>
		public NameValueCollection Parameters
		{
			get
			{
				return this._parameters;
			}
		}
	}
}