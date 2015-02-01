using System;
using System.Collections;
using System.Collections.Specialized;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Метаданные объекта, открытого оболочкой через URI.
	/// </summary>
	internal struct UriResolvedMetadata
	{		
		/// <summary>
		/// URI, по которому был открыт объект.
		/// </summary>
		private readonly Uri _uri;

		/// <summary>
		/// Сервис, позволяющий закрыть объект.
		/// </summary>
		private readonly IDisposable _disposable;

		/// <summary>
		/// Идентификатор объекта, открытого через URI.
		/// </summary>
		private readonly int _resolvedId;

		/// <summary>
		/// Инициализирует новый объект <see cref="UriResolvedMetadata"/>.
		/// </summary>
		/// <param name="uri">URI, по которому был открыт объект.</param>
		/// <param name="disposable">Сервис, позволяющий закрыть объект.</param>
		public UriResolvedMetadata(Uri uri, IDisposable disposable)
		{
			this._uri = uri;
			this._disposable = disposable;
			this._resolvedId = 0;
		}

		/// <summary>
		/// Инициализирует новый объект <see cref="UriResolvedMetadata"/>.
		/// </summary>
		/// <param name="source">Метаданные, из которых создаются новые.</param>
		/// <param name="resolvedId">Идентификатор объекта, открытого через URI.</param>
		private UriResolvedMetadata(UriResolvedMetadata source, int resolvedId)
		{
			this = source;
			this._resolvedId = resolvedId;
		}

		/// <summary>
		/// Возвращает новые метаданные с назначенным идентификатором объекта,
		/// открытого оболочкой через URI.
		/// </summary>
		/// <param name="resolvedId">Идентификатор объекта, открытого через URI.</param>
		/// <returns>Копию метаданных с назначенным идентификатором объекта.</returns>
		public UriResolvedMetadata AssignId(int resolvedId)
		{
			return new UriResolvedMetadata(this, resolvedId);
		}

		/// <summary>
		/// Возвращает URI, по которому был открыт объект.
		/// </summary>
		public Uri Uri
		{
			get
			{
				return this._uri;
			}
		}

		/// <summary>
		/// Возвращает сервис, позволяющий закрыть объект.
		/// </summary>
		public IDisposable Disposable
		{
			get
			{
				return this._disposable;
			}
		}

		/// <summary>
		/// Возвращает идентификатор объекта, открытого через URI.
		/// </summary>
		public int ResolvedId
		{
			get
			{
				return this._resolvedId;
			}
		}
	}
}
