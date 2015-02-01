using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Представляет собой гиперссылку для открытия URI из пользовательского интерфейса.
	/// </summary>
	public class PhoenixHyperlink
	{
		/// <summary>
		/// URI, который открывает гиперссылка.
		/// </summary>
		private readonly Uri _uri; 

		/// <summary>
		/// Текст, показываемый в гиперссылке.
		/// </summary>
		private readonly string _text;

		/// <summary>
		/// URI иконки, показываемой в гиперссылке.
		/// </summary>
		private readonly Uri _icon;

		/// <summary>
		/// Инициализирует новый объект <see cref="PhoenixHyperlink"/>
		/// </summary>
		/// <param name="uri">URI, который открывает гиперссылка.</param>
		public PhoenixHyperlink(Uri uri)
			: this(uri, null, null)
		{
		}

		/// <summary>
		/// Инициализирует новый объект <see cref="PhoenixHyperlink"/>
		/// </summary>
		/// <param name="uri">URI, который открывает гиперссылка.</param>
		/// <param name="text">Текст, показываемый в гиперссылке.</param>
		public PhoenixHyperlink(Uri uri, string text)
			: this(uri, text, null)
		{
		}

		/// <summary>
		/// Инициализирует новый объект <see cref="PhoenixHyperlink"/>
		/// </summary>
		/// <param name="uri">URI, который открывает гиперссылка.</param>
		/// <param name="text">Текст, показываемый в гиперссылке.</param>
		/// <param name="icon">URI иконки, показываемой в гиперссылке.</param>
		public PhoenixHyperlink(Uri uri, string text, Uri icon)
		{
			this._uri = uri;
			this._text = text;
			this._icon = icon;
		}

		/// <summary>
		/// Задает инвариант класса.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.Uri != null);
			Contract.Invariant(!string.IsNullOrWhiteSpace(this.Text));
		}

		/// <summary>
		/// Возвращает URI, который открывает гиперссылка.
		/// </summary>
		public Uri Uri
		{
			get
			{
				return this._uri;
			}
		}

		/// <summary>
		/// Возвращает текст, показываемый в гиперссылке.
		/// </summary>
		public string Text
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this._text))
				{
					return this._uri.ToString();
				}

				return this._text;
			}
		}

		/// <summary>
		/// Возвращает URI иконки, показываемой в гиперссылке.
		/// </summary>
		public Uri Icon
		{
			get
			{
				return this._icon;
			}
		}
	}
}
