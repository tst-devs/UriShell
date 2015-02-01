using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace UriShell.Logging
{
    /// <summary>
    /// Задает виды категорий записей лога.
    /// </summary>
    public enum LogCategory
    {
        /// <summary>
        /// Категория, предназначенная для отладочных записей.
        /// </summary>
        Debug,

        /// <summary>
        /// Категория для записи исключений, критических ошибок.
        /// </summary>
        Exception,

        /// <summary>
        /// Категория информационных сообщений.
        /// </summary>
        Information,

        /// <summary>
        /// Категория для записи сообщений, требующих повышенного внимания.
        /// </summary>
        Warning
    }
}