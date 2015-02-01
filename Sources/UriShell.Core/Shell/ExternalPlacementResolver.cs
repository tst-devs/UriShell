using System;
using System.ComponentModel;
using System.Diagnostics;

namespace UriShell.Shell
{
	/// <summary>
	/// Реализует размещение объектов во внешнем процессе.
	/// </summary>
	internal sealed class ExternalPlacementResolver : IUriPlacementResolver, IUriPlacementConnector
	{
		/// <summary>
		/// Определяет размещение объекта по заданному URI.
		/// </summary>
		/// <param name="resolved">Объект для размещения по заданному URI.</param>
		/// <param name="uri">URI, по которому требуется разместить объект.</param>
		/// <param name="attachmentSelector">Селектор, предоставляющий доступ к объектам,
		/// прикрепленным к URI через параметры.</param>
		/// <returns><see cref="IUriPlacementConnector"/>, позволяющий присоединить объект
		/// к пользовательскому интерфейсу, если размещение было определено; иначе null.</returns>
		public IUriPlacementConnector Resolve(object resolved, Uri uri, UriAttachmentSelector attachmentSelector)
		{
			var builder = new PhoenixUriBuilder(uri);
			if (builder.Placement == "external" && resolved is Process)
			{
				return this;
			}

			return null;
		}

		/// <summary>
		/// Присоединяет заданный объект к пользовательскому интерфейсу.
		/// </summary>
		/// <param name="resolved">Объект для присоединения к UI.</param>
		public void Connect(object resolved)
		{
			var process = (Process)resolved;
			try
			{
				process.Start();
			}
			catch (Win32Exception ex)
			{
				var message = string.Format(Properties.Resources.OpenExternalFileError, process.StartInfo.FileName, ex.Message);
				throw new Exception(message, ex);
			}
		}

		/// <summary>
		/// Отсоединяет заданный объект от пользовательского интерфейса.
		/// </summary>
		/// <param name="resolved">Объект для отсоединения от UI.</param>
		public void Disconnect(object resolved)
		{
			((Process)resolved).Kill();
		}

		/// <summary>
		/// Возвращает признак того, что данный коннектор сам отвечает за 
		/// обновление данных в присоединенных объектах.
		/// </summary>
		public bool IsResponsibleForRefresh
		{
			get 
			{
				return true;
			}
		}
	}
}
