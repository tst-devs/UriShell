using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace UriShell.Shell
{
	/// <summary>
	/// Исключение, выбрасываемое оболочкой, когда открываемый URI содержит ошибки.
	/// </summary>
	[Serializable]
	public class UriResolutionException : Exception
	{
		/// <summary>
		/// Наименование значения, содержащего <see cref="Uri"/> в сериализованных данных.
		/// </summary>
		private const string UriSerializationName = "URI";

		/// <summary>
		/// URI, в котором была обнаружена ошибка.
		/// </summary>
		private readonly Uri _uri;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="UriResolutionException"/>
		/// с указанным сообщением об ошибке и URI.
		/// </summary>
		/// <param name="uri">URI, в котором была обнаружена ошибка.</param>
		/// <param name="message">Сообщение, описывающее ошибку.</param>
		public UriResolutionException(Uri uri, string message)
			: base(message)
		{
			this._uri = uri;
		}

		/// <summary>
		/// Инициализирует новый объект класса <see cref="UriResolutionException"/>
		/// с указанным сообщением об ошибке, URI и ссылкой на внутреннее исключение,
		/// ставшее причиной возникновения создаваемого исключения.
		/// </summary>
		/// <param name="uri">URI, в котором была обнаружена ошибка.</param>
		/// <param name="message">Сообщение, описывающее ошибку.</param>
		/// <param name="innerException">Исключение, ставшее причиной возникновения создаваемого исключения.</param>
		public UriResolutionException(Uri uri, string message, Exception innerException)
			: base(message, innerException)
		{
			this._uri = uri;
		}

		/// <summary>
		/// Выполняет инициализацию нового экземпляра класса <see cref="UriResolutionException"/>
		/// с сериализованными данными.
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/>, содержащий сериализованные данные объекта
		/// о выбрасываемом исключении. </param>
		/// <param name="context"><see cref="StreamingContext"/>, содержащий контекстные сведения об
		/// источнике или назначении.</param>
		protected UriResolutionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._uri = (Uri)info.GetValue(UriResolutionException.UriSerializationName, typeof(Uri));
		}

		/// <summary>
		/// Задает инвариант класса.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.Uri != null);
		}

		/// <summary>
		/// Задает <see cref="SerializationInfo"/> с дополнительными сведениями об исключении.
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/>, содержащий сериализованные данные объекта
		/// о выбрасываемом исключении. </param>
		/// <param name="context"><see cref="StreamingContext"/>, содержащий контекстные сведения об
		/// источнике или назначении.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			Contract.Requires<ArgumentNullException>(info != null);

			base.GetObjectData(info, context);

			info.AddValue(UriResolutionException.UriSerializationName, this._uri, typeof(Uri));
		}

		/// <summary>
		/// Возвращает URI, в котором была обнаружена ошибка.
		/// </summary>
		public Uri Uri
		{
			get
			{
				return this._uri;
			}
		}

		/// <summary>
		/// Возвращает сообщение, описывающее исключение.
		/// </summary>
		public override string Message
		{
			get
			{
				var uriInfo = string.Format(Properties.Resources.UriResolutionExceptionUri, this.Uri);

				return string.Format("{0}{1}{2}", base.Message, Environment.NewLine, uriInfo);
			}
		}
	}
}
