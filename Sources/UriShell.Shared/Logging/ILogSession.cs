using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.IO;

namespace UriShell.Logging
{
	/// <summary>
	/// Интерфейс сессии логирования, объекта позволяющего производить запись в лог.
	/// </summary>
	[ContractClass(typeof(ILogSessionContract))]
	public interface ILogSession
	{
		/// <summary>
		/// Возвращает отдельную сессию логирования, которая пишет в данную, снабжая записи уникальным идентификатором.
		/// </summary>
		/// <param name="subject">Предмет логирования, требующий идентификации.</param>
		/// <returns>Новую сессию для логирования с уникальным идентификатором.</returns>
		ILogSession SupplyId(string subject);

		/// <summary>
		/// Записывает текстовое сообщение в лог.
		/// </summary>
		/// <param name="message">Сообщение для записи в лог.</param>
		void LogMessage(string message);

		/// <summary>
		/// Записывает текстовое сообщение в лог.
		/// </summary>
		/// <param name="message">Сообщение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого сообщения.</param>
		void LogMessage(string message, LogCategory category);
		
		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void LogValue(string title, bool value);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void LogValue(string title, bool value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void LogValue(string title, byte value);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void LogValue(string title, byte value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void LogValue(string title, char value);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void LogValue(string title, char value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void LogValue(string title, int value);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void LogValue(string title, int value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void LogValue(string title, long value);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void LogValue(string title, long value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void LogValue(string title, double value);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void LogValue(string title, double value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void LogValue(string title, decimal value);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void LogValue(string title, decimal value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void LogValue(string title, string value);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void LogValue(string title, string value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void LogValue(string title, DateTime value);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void LogValue(string title, DateTime value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void LogValue(string title, object value);

		/// <summary>
		/// Записывает заданное значение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void LogValue(string title, object value, LogCategory category);

		/// <summary>
		/// Записывает заданный список в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="list">Список для записи в лог.</param>
		void LogEnumerable(string title, IEnumerable list);

		/// <summary>
		/// Записывает заданный список в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="list">Список для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого списка.</param>
		void LogEnumerable(string title, IEnumerable list, LogCategory category);

		/// <summary>
		///Записывает заданный словарь в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="dictionary">Словарь для записи в лог.</param>
		void LogDictionary(string title, IDictionary dictionary);

		/// <summary>
		///Записывает заданный словарь в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="dictionary">Словарь для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого словаря.</param>
		void LogDictionary(string title, IDictionary dictionary, LogCategory category);

		/// <summary>
		/// Записывает заданный поток в лог в бинарной форме.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="stream">Поток для записи в лог.</param>
		void LogBinaryStream(string title, Stream stream);

		/// <summary>
		/// Записывает заданный поток в лог в бинарной форме.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="stream">Поток для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого потока.</param>
		void LogBinaryStream(string title, Stream stream, LogCategory category);

		/// <summary>
		/// Записывает заданный поток в лог в текстовой форме.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="stream">Поток для записи в лог.</param>
		void LogTextStream(string title, Stream stream);

		/// <summary>
		/// Записывает заданное исключение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="exception">Исключение для записи в лог.</param>
		void LogException(string title, Exception exception);

		/// <summary>
		/// Записывает заданное исключение в лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="exception">Исключение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого исключения.</param>
		void LogException(string title, Exception exception, LogCategory category);

		/// <summary>
		/// Записывает заданный поток в лог в текстовой форме.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="stream">Поток для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого потока.</param>
		void LogTextStream(string title, Stream stream, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void WatchValue(string title, bool value);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void WatchValue(string title, bool value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void WatchValue(string title, byte value);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void WatchValue(string title, byte value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void WatchValue(string title, char value);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void WatchValue(string title, char value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void WatchValue(string title, int value);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void WatchValue(string title, int value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void WatchValue(string title, long value);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void WatchValue(string title, long value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void WatchValue(string title, double value);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void WatchValue(string title, double value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void WatchValue(string title, decimal value);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void WatchValue(string title, decimal value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void WatchValue(string title, string value);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void WatchValue(string title, string value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void WatchValue(string title, DateTime value);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void WatchValue(string title, DateTime value, LogCategory category);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		void WatchValue(string title, object value);

		/// <summary>
		/// Записывает заданное значение в Watch-лог.
		/// </summary>
		/// <param name="title">Заголовок записи лога.</param>
		/// <param name="value">Значение для записи в лог.</param>
		/// <param name="category">Категория значимости записываемого значения.</param>
		void WatchValue(string title, object value, LogCategory category);
	}
}
