using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Disposables;

using UriShell.Extensions;
using UriShell.Logging;
using UriShell.Shell.Events;
using UriShell.Shell.Registration;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Позволяет использовать объект, полученный через URI.
	/// </summary>
	internal sealed partial class ResolveOpen : IShellResolve
	{
		/// <summary>
		/// URI, который требуется разрешить.
		/// </summary>
		private readonly Uri _unresolvedUri;

		/// <summary>
		/// Список объектов, прикрепляемых к URI с помощью идентификаторов.
		/// </summary>
		private readonly object[] _attachments;

		/// <summary>
		/// Фабрика сервиса, реализующего настройку объектов, полученных через URI.
		/// </summary>
		private readonly IResolveSetupFactory _resolveSetupFactory;

		/// <summary>
		/// Холдер объектов, открытых оболочкой через URI.
		/// </summary>
		private readonly IUriResolvedObjectHolder _uriResolvedObjectHolder;

		/// <summary>
		/// Объект, предоставляющий настраиваемые компоненты процесса открытия URI.
		/// </summary>
		private readonly IUriResolutionCustomization _uriResolutionCustomization;

		/// <summary>
		/// Объект для записи в лог.
		/// </summary>
		private readonly ILogSession _logSession;

		/// <summary>
		/// Функция для вызова настройки объекта, полученного через URI.
		/// </summary>
		private ResolveSetupPlayer _resolveSetupPlayer;

		/// <summary>
		/// Сервис рассылки событий.
		/// </summary>
		private readonly IEventBroadcaster _eventBroadcaster;

		/// <summary>
		/// Таблица отсоединения объектов от пользовательского интерфейса.
		/// </summary>
		private readonly IUriDisconnectTable _uriDisconnectTable;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="ResolveOpen"/>.
		/// </summary>
		/// <param name="uri">URI, который требуется разрешить.</param>
		/// <param name="attachments">Список объектов, прикрепляемых к URI с помощью идентификаторов.</param>
		/// <param name="resolveSetupFactory">Фабрика сервиса, реализующего настройку объектов, полученных через URI.</param>
		/// <param name="uriResolvedObjectHolder">Холдер объектов, открытых оболочкой через URI.</param>
		/// <param name="uriDisconnectTable">Таблица отсоединения объектов от пользовательского интерфейса.</param>
		/// <param name="uriResolutionCustomization">Объект, предоставляющий настраиваемые компоненты процесса открытия URI.</param>
		/// <param name="eventBroadcaster">Сервис рассылки событий.</param>
		/// <param name="logSession">Объект для записи в лог.</param>
		public ResolveOpen(
			Uri uri,
			object[] attachments,
			IResolveSetupFactory resolveSetupFactory,
			IUriResolvedObjectHolder uriResolvedObjectHolder,
			IUriDisconnectTable uriDisconnectTable,
			IUriResolutionCustomization uriResolutionCustomization,
			IEventBroadcaster eventBroadcaster,
			ILogSession logSession)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentNullException>(attachments != null);
			Contract.Requires<ArgumentNullException>(resolveSetupFactory != null);
			Contract.Requires<ArgumentNullException>(uriResolvedObjectHolder != null);
			Contract.Requires<ArgumentNullException>(uriDisconnectTable != null);
			Contract.Requires<ArgumentNullException>(uriResolutionCustomization != null);
			Contract.Requires<ArgumentNullException>(eventBroadcaster != null);
			Contract.Requires<ArgumentNullException>(logSession != null);

			this._unresolvedUri = uri;
			this._attachments = attachments;
			this._resolveSetupFactory = resolveSetupFactory;
			this._uriResolvedObjectHolder = uriResolvedObjectHolder;
			this._uriResolutionCustomization = uriResolutionCustomization;
			this._eventBroadcaster = eventBroadcaster;
			this._logSession = logSession;
			this._uriDisconnectTable = uriDisconnectTable;
		}

		/// <summary>
		/// Позволяет настроить объект, полученный через URI, если его тип совместим с заданным.
		/// </summary>
		/// <typeparam name="TResolved">Тип объекта, который ожидается от URI.</typeparam>
		/// <returns>Сервис для настройки объекта.</returns>
		public IShellResolveSetup<TResolved> Setup<TResolved>()
		{
			return this._resolveSetupFactory.Create<TResolved>(new ResolveSetupArgs(this, this.ReceiveResolveSetupPlayer));
		}

		/// <summary>
		/// Получает функцию для вызова настройки объекта, полученного через URI.
		/// </summary>
		/// <param name="player">Функция для вызова настройки объекта, полученного через URI.</param>
		private void ReceiveResolveSetupPlayer(ResolveSetupPlayer player)
		{
			if (player == null)
			{
				throw new InvalidOperationException(
					string.Format(Properties.Resources.ShellResolveSetupPlayerCannotBeNull, this._unresolvedUri));
			}

			if (this._resolveSetupPlayer != null)
			{
				throw new InvalidOperationException(
					string.Format(Properties.Resources.AttemptToOverwriteShellResolveSetupPlayer, this._unresolvedUri));
			}

			this._resolveSetupPlayer = player;
		}

		/// <summary>
		/// Формирует URI, в котором соответствующие значения параметров заменены сгенерированными
		/// идентификаторами прикрепляемых объектов, и предоставляет селектор для получения этих объектов.
		/// </summary>
		/// <param name="uri">На выходе из метода содержит URI, в котором соответствующие параметры значения
		/// заменены сгенерированными идентификаторами прикрепляемых объектов.</param>
		/// <param name="attachmentSelector">На выходе из метода содержит селектор, предоставляющий доступ к
		/// объектам, прикрепленным к URI с помощью идентификаторов.</param>
		private void EmbedAttachments(out Uri uri, out UriAttachmentSelector attachmentSelector)
		{
			if (this._attachments.Length == 0)
			{
				attachmentSelector = id => null;
				uri = this._unresolvedUri;

				return;
			}

			var attachmentDictionary = new HybridDictionary(this._attachments.Length);
			var idGenerator = new Random();

			// Заменяем значения параметров в URI на идентификаторы.
			var uriBuilder = new PhoenixUriBuilder(this._unresolvedUri);
			for (int i = 0; i < uriBuilder.Parameters.Count; i++)
			{
				var value = uriBuilder.Parameters[i];

				// Placeholder объекта определяем по формату "{индекс}",
				// где индекс - целое в диапазоне числа элементов attachments.

				if (value.Length <= 2)
				{
					continue;
				}
				if (value[0] != '{')
				{
					continue;
				}
				if (value[value.Length - 1] != '}')
				{
					continue;
				}

				value = value.Substring(1, value.Length - 2);

				int attachmentIndex;
				if (int.TryParse(value, out attachmentIndex))
				{
					if (attachmentIndex >= this._attachments.Length)
					{
						continue;
					}

					value = idGenerator.Next().ToString();
					attachmentDictionary[value] = this._attachments[attachmentIndex];
				}

				uriBuilder.Parameters[uriBuilder.Parameters.AllKeys[i]] = value;
			}

			attachmentSelector = id => id != null ? attachmentDictionary[id] : null;
			uri = uriBuilder.Uri;
		}

		/// <summary>
		/// Получает объект, на который указывает открываемый URI, используя <see cref="IUriModuleItemResolver"/>.
		/// </summary>
		/// <param name="uri">URI, указывающий на объект.</param>
		/// <param name="attachmentSelector">Селектор, предоставляющий доступ к объектам,
		/// прикрепленным к URI с помощью идентификаторов.</param>
		/// <returns>Объект, на который указывает открываемый URI.</returns>
		private object ResolveModuleItem(Uri uri, UriAttachmentSelector attachmentSelector)
		{
			var builder = new PhoenixUriBuilder(uri);
			var moduleItemResolverKey = new UriModuleItemResolverKey(builder.Module, builder.Item);

			IUriModuleItemResolver resolver;
			if (this._uriResolutionCustomization.ModuleItemResolvers.TryGetValue(moduleItemResolverKey, out resolver))
			{
				return resolver.Resolve(uri, attachmentSelector);
			}

			throw new UriResolutionException(
				uri, 
				string.Format(Properties.Resources.UriModuleItemResolverNotFound, typeof(IUriModuleItemResolver).Name));
		}

		/// <summary>
		/// Ищет размещение для объекта, на который указывает заданный URI.
		/// </summary>
		/// <param name="uri">URI, указывающий на объект.</param>
		/// <param name="attachmentSelector">Селектор, предоставляющий доступ к объектам,
		/// прикрепленным к URI с помощью идентификаторов.</param>
		/// <param name="resolved">Объект, полученный через URI.</param>
		/// <returns><see cref="IUriPlacementConnector"/> для присоединения объекта к пользовательскому интерфейсу.</returns>
		private IUriPlacementConnector ResolvePlacement(Uri uri, UriAttachmentSelector attachmentSelector, object resolved)
		{
			var placementConnector = this._uriResolutionCustomization
				.PlacementResolvers
				.Select(r => r.Resolve(resolved, uri, attachmentSelector))
				.FirstOrDefault(c => c != null);

			if (placementConnector != null)
			{
				return placementConnector;
			}

			throw new UriResolutionException(
				uri,
				string.Format(Properties.Resources.UriPlacementNotFound, typeof(IUriPlacementResolver).Name));
		}

		/// <summary>
		/// Пробует сделать объект, полученный через URI, доступным в оболочке.
		/// </summary>
		/// <param name="uri">URI, указывающий на объект.</param>
		/// <param name="resolved">Объект, полученный через URI.</param>
		/// <param name="placementConnector"><see cref="IUriPlacementConnector"/> для присоединения объекта
		/// к пользовательскому интерфейсу.</param>
		/// <param name="appendToDisposable">На выходе из метода содержит действие, позволяющее нарастить
		/// цепочку <see cref="IDisposable"/> в метаданных.</param>
		/// <returns><see cref="IDisposable"/>, зарегистрированный в метаданных.</returns>
		private IDisposable Connect(Uri uri, object resolved, IUriPlacementConnector placementConnector, out Action<IDisposable> appendToDisposable)
		{
			placementConnector.Connect(resolved);
			try
			{
				var composite = new CompositeDisposable();
				appendToDisposable = composite.Add;

				this._uriResolvedObjectHolder.Add(resolved, new UriResolvedMetadata(uri, composite));
				this._uriDisconnectTable[resolved] = placementConnector;

				return composite;
			}
			catch (Exception ex)
			{
				if (!ex.IsCritical())
				{
					// При невозможности внести объект в холдер, убираем его из UI.
					placementConnector.Disconnect(resolved);
				}

				throw;
			}
		}

		/// <summary>
		/// Вызывает настройку объекта, полученного через URI.
		/// </summary>
		/// <param name="uri">URI, указывающий на объект.</param>
		/// <param name="resolved">Объект, полученный через URI.</param>
		/// <returns>Сервис, вызываемый, когда в объекте больше нет необходимости.</returns>
		private IDisposable PlaySetup(Uri uri, object resolved)
		{
			if (this._resolveSetupPlayer != null)
			{
				try
				{
					return this._resolveSetupPlayer(uri, resolved, this._logSession);
				}
				catch (Exception ex)
				{
					if (ex.IsCritical())
					{
						throw;
					}

					// Успешность вызова настроек не должна влиять на вызывающий код.
					this._logSession.LogException(ex.Message, ex);
				}
			}

			return Disposable.Empty;
		}

		/// <summary>
		/// Создает <see cref="IDisposable"/> для закрытия объекта.
		/// </summary>
		/// <param name="resolved">Объект, полученный через URI.</param>
		/// <param name="uriResolvedObjectHolder">Холдер объектов, открытых оболочкой через URI.</param>
		/// <param name="uriDisconnectTable">Таблица отсоединения объектов от пользовательского интерфейса.</param>
		/// <param name="logSession">Объект для записи в лог.</param>
		/// <returns><see cref="IDisposable"/> для закрытия объекта.</returns>
		private static IDisposable CreateCloseDisposable(
			object resolved,
			IUriResolvedObjectHolder uriResolvedObjectHolder,
			IUriDisconnectTable uriDisconnectTable,
			ILogSession logSession)
		{
			return Disposable.Create(() =>
			{
				try
				{
					uriDisconnectTable[resolved].Disconnect(resolved);

					var disposableResolved = resolved as IDisposable;
					if (disposableResolved != null)
					{
						disposableResolved.Dispose();
					}

					uriDisconnectTable.Remove(resolved);
					uriResolvedObjectHolder.Remove(resolved);
				}
				catch (Exception ex)
				{
					if (ex.IsCritical())
					{
						throw;
					}

					logSession.LogException(ex.Message, ex);
				}
			});
		}

		/// <summary>
		/// Рассылает событие с целью обновления данных в заданном объекте, полученном через URI.
		/// </summary>
		/// <param name="resolved">Объект, полученный через URI.</param>
		/// <param name="placementConnector"><see cref="IUriPlacementConnector"/>, использованный
		/// для присоединения объекта к пользовательскому интерфейсу.</param>
		private void SendRefresh(object resolved, IUriPlacementConnector placementConnector)
		{
			if (placementConnector.IsResponsibleForRefresh)
			{
				return;
			}

			var id = this._uriResolvedObjectHolder.GetMetadata(resolved).ResolvedId;
			var args = new ResolvedIdBroadcastArgs(id);
			this._eventBroadcaster.Send(ShellEventKeys.RefreshResolved, args);
		}

		/// <summary>
		/// Открывает объект, полученный через URI.
		/// </summary>
		/// <returns>Сервис, позволяющий закрыть объект вызовом <see cref="IDisposable.Dispose"/>.</returns>
		public IDisposable Open()
		{
			try
			{
				return this.OpenOrThrow();
			}
			catch (Exception ex)
			{
				if (ex.IsCritical())
				{
					throw;
				}

				this._logSession.LogException(ex.Message, ex);
			}

			return Disposable.Empty;
		}

		/// <summary>
		/// Открывает объект, полученный через URI, позволяя вызывающему коду обработать
		/// исключение в случае неудачи.
		/// </summary>
		/// <returns>Сервис, позволяющий закрыть объект вызовом <see cref="IDisposable.Dispose"/>,
		/// если объект открыт успешно.</returns>
		public IDisposable OpenOrThrow()
		{
			Uri uri;
			UriAttachmentSelector attachmentSelector;
			this.EmbedAttachments(out uri, out attachmentSelector);

			var resolved = this.ResolveModuleItem(uri, attachmentSelector);
			var placementConnector = this.ResolvePlacement(uri, attachmentSelector, resolved);

			Action<IDisposable> appendToDisposable;
			var disposable = this.Connect(uri, resolved, placementConnector, out appendToDisposable);
				
			appendToDisposable(this.PlaySetup(uri, resolved));
			appendToDisposable(ResolveOpen.CreateCloseDisposable(
				resolved,
				this._uriResolvedObjectHolder,
				this._uriDisconnectTable,
				this._logSession));

			this._logSession.LogMessage(string.Format(Properties.Resources.ShellResolveOpenComplete, uri));
			this.SendRefresh(resolved, placementConnector);

			return disposable;
		}
	}
}