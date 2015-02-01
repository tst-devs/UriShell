using System;
using System.Diagnostics.Contracts;

namespace UriShell
{
    /// <summary>
    /// Данные оповещения пользователя.
    /// </summary>
    public class NotificationPack
    {
        /// <summary>
        /// Инициализирует новый объект класса <see cref="NotificationPack"/>.
        /// </summary>
        /// <param name="briefContent">Содержимое для отображения краткого сообщения.</param>
        public NotificationPack(object briefContent)
            : this(briefContent, null, null)
        {
        }

        /// <summary>
        /// Инициализирует новый объект класса <see cref="NotificationPack"/>.
        /// </summary>
        /// <param name="briefContent">Содержимое для отображения краткого сообщения.</param>
        /// <param name="detailedContent">Содержимое для отображения подробного сообщения.</param>
        public NotificationPack(object briefContent, object detailedContent)
            : this(briefContent, detailedContent, null)
        {
        }

        /// <summary>
        /// Инициализирует новый объект класса <see cref="NotificationPack"/>.
        /// </summary>
        /// <param name="briefContent">Содержимое для отображения краткого сообщения.</param>
        /// <param name="exception">Исключение, для которого показывается сообщение.</param>
        public NotificationPack(object briefContent, Exception exception)
            : this(briefContent, null, exception)
        {
        }

        /// <summary>
        /// Инициализирует новый объект класса <see cref="NotificationPack"/>.
        /// </summary>
        /// <param name="briefContent">Содержимое для отображения краткого сообщения.</param>
        /// <param name="detailedContent">Содержимое для отображения подробного сообщения.</param>
        /// <param name="exception">Исключение, для которого показывается сообщение.</param>
        public NotificationPack(object briefContent, object detailedContent, Exception exception)
        {
            Contract.Requires<ArgumentNullException>(briefContent != null);

            this.BriefContent = briefContent;
            this.DetailedContent = detailedContent;
            this.Exception = exception;
        }

        /// <summary>
        /// Возвращает содержимое для отображения краткого сообщения.
        /// </summary>
        public object BriefContent
        {
            get;
            private set;
        }

        /// <summary>
        /// Возвращает содержимое для отображения подробного сообщения.
        /// </summary>
        public object DetailedContent
        {
            get;
            private set;
        }

        /// <summary>
        /// Возвращает исключение, для которого показывается сообщение.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }
    }
}