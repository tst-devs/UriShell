using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

using Autofac.Features.Indexed;

using UriShell.Extensions;
using UriShell.Shell.Registration;
using UriShell.Shell.Resolution;

namespace UriShell.Shell
{
	using ShellResolveFactory = Func<Uri, object[], IShellResolve>;
	using UriModuleItemResolverIndex = IIndex<UriModuleItemResolverKey, IUriModuleItemResolver>;

#warning Change to default Uri Provider
    internal interface ISecurityService
    {

    }

	/// <summary>
	/// Реализации оболочки <see cref="IShell"/>.
	/// </summary>
	internal sealed partial class Shell : IShell, IUriResolutionCustomization
	{
		/// <summary>
		/// Регулярное выражение для разбора гиперссылок.
		/// </summary>
		private static readonly Regex _HyperLinkRegex = new Regex("<a\\s+href=\"([^\"]+)\">(.*)</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// Фабрика объекта, начинающего процесс открытия URI.
		/// </summary>
		private readonly ShellResolveFactory _shellResolveFactory;

		/// <summary>
		/// Холдер объектов, открытых оболочкой через URI.
		/// </summary>
		private readonly IUriResolvedObjectHolder _uriResolvedObjectHolder;
	
		/// <summary>
		/// Список сервисов, определяющих размещение объектов по заданному URI.
		/// </summary>
		private readonly WeakBucket<IUriPlacementResolver> _uriPlacementResolvers = new WeakBucket<IUriPlacementResolver>();
		
		/// <summary>
		/// Фабрика списка сервисов, умеющих создавать объекты по заданному URI.
		/// </summary>
		private readonly Func<UriModuleItemResolverIndex> _uriModuleItemResolversFactory;

		/// <summary>
		/// Сервис авторизации и настроек безопасности. 
		/// </summary>
		private readonly ISecurityService _securityService;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="Shell"/>.
		/// </summary>
		/// <param name="securityService">Сервис авторизации и настроек безопасности.</param>
		/// <param name="uriModuleItemResolversFactory">Фабрика списка сервисов, умеющих создавать
		/// объекты по заданному URI.</param>
		/// <param name="uriResolvedObjectHolder">Холдер объектов, открытых оболочкой через URI.</param>
		/// <param name="shellResolveFactory">Фабрика объекта, начинающего процесс открытия URI.</param>
		public Shell(
			ISecurityService securityService,
			Func<UriModuleItemResolverIndex> uriModuleItemResolversFactory,
			IUriResolvedObjectHolder uriResolvedObjectHolder,
			ShellResolveFactory shellResolveFactory)
		{
			Contract.Requires<ArgumentNullException>(securityService != null);
			Contract.Requires<ArgumentNullException>(shellResolveFactory != null);
			Contract.Requires<ArgumentNullException>(uriResolvedObjectHolder != null);
			Contract.Requires<ArgumentNullException>(uriModuleItemResolversFactory != null);

			this._securityService = securityService;
			this._shellResolveFactory = shellResolveFactory;
			this._uriResolvedObjectHolder = uriResolvedObjectHolder;
			this._uriModuleItemResolversFactory = uriModuleItemResolversFactory;
		}

		/// <summary>
		/// Возвращает список сервисов, умеющих создавать объекты по заданному URI,
		/// зарегистрированные через <see cref="UriModuleItemResolverKey"/>.
		/// </summary>
		public IIndex<UriModuleItemResolverKey, IUriModuleItemResolver> ModuleItemResolvers
		{
			get
			{
				return this._uriModuleItemResolversFactory();
			}
		}

		/// <summary>
		/// Возвращает список сервисов, определяющих размещение объектов по заданному URI.
		/// </summary>
		public IEnumerable<IUriPlacementResolver> PlacementResolvers
		{
			get
			{
				return this._uriPlacementResolvers.ExtractAlive();
			}
		}

		/// <summary>
		/// Добавляет в оболочку заданный <see cref="IUriPlacementResolver"/>.
		/// </summary>
		/// <param name="uriPlacementResolver"><see cref="IUriPlacementResolver"/>
		/// для добавления с использованием слабой ссылки.</param>
		public void AddUriPlacementResolver(IUriPlacementResolver uriPlacementResolver)
		{
			this._uriPlacementResolvers.Add(uriPlacementResolver);
		}

		/// <summary>
		/// Начинает процесс открытия заданного URI.
		/// </summary>
		/// <param name="uri">URI, который требуется открыть.</param>
		/// <param name="attachments">Список объектов, прикрепляемых к URI с помощью идентификаторов.</param>
		/// <returns>Сервис, позволяющий настроить и открыть объект, полученный через URI.</returns>
		public IShellResolve Resolve(Uri uri, params object[] attachments)
		{
			return this._shellResolveFactory(uri, attachments);
		}
		
		/// <summary>
		/// Проверяет, открыт ли заданный объект.
		/// </summary>
		/// <param name="resolved">Объект, для проверки, открыт ли он.</param>
		/// <returns>true, если объект открыт; иначе false.</returns>
		public bool IsResolvedOpen(object resolved)
		{
			return this._uriResolvedObjectHolder.Contains(resolved);
		}
		
		/// <summary>
		/// Возвращает идентификатор заданного объекта, открытого через URI.
		/// </summary>
		/// <param name="resolved">Объект, для которого запрашивается идентификатор.</param>
		/// <returns>Идентификатор заданного объекта.</returns>
		public int GetResolvedId(object resolved)
		{
			return this._uriResolvedObjectHolder.GetMetadata(resolved).ResolvedId;
		}

		/// <summary>
		/// Возвращает URI заданного открытого объекта.
		/// </summary>
		/// <param name="resolved">Объект, для которого запрашивается URI.</param>
		/// <returns>URI заданного объекта.</returns>
		public Uri GetResolvedUri(object resolved)
		{
			return this._uriResolvedObjectHolder.GetMetadata(resolved).Uri;
		}

		/// <summary>
		/// Закрывает указанный объект.
		/// </summary>
		/// <param name="resolved">Объект, который необходимо закрыть.</param>
		public void CloseResolved(object resolved)
		{
			this._uriResolvedObjectHolder.GetMetadata(resolved).Disposable.Dispose();
		}

		/// <summary>
		/// Пробует создать гиперссылку из текстового описания.
		/// </summary>
		/// <param name="hyperlink">Текстовое описание гиперссылки в формате гиперссылки HTML.</param>
		/// <param name="ownerId">Идентификатор объекта, к которому относится объект, получаемый
		/// при открытии гиперссылки.</param>
		/// <returns>Гиперссылку, если указанное текстовое описание гиперссылки верно; иначе null.</returns>
		public PhoenixHyperlink TryParseHyperlink(string hyperlink, int ownerId)
		{
			var matches = Shell._HyperLinkRegex.Matches(hyperlink);
			if (matches.Count == 0)
			{
				return null;
			}

			// Если текст в ячейке удовлетворяет шаблону гиперссылки,
			// то возвращаем описатель гиперссылки.

			var match = matches[0];
			var uri = new Uri(match.Groups[1].Value);
			var title = match.Groups[2].Value;

			if (uri.IsPhoenix())
			{
				// Добавляем OwnerId к URI представлений.
				var builder = new PhoenixUriBuilder(uri);
				builder.OwnerId = ownerId;
				uri = builder.Uri;
			}

			return new PhoenixHyperlink(uri, title, null);
		}

		/// <summary>
		/// Создает гиперссылку на основе указанного <see cref="Uri"/>. 
		/// </summary>
		/// <param name="uri">URI, для которого создается гиперссылка.</param>
		/// <returns>Гиперссылку, созданную на основе указанного <see cref="Uri"/>.</returns>
		public PhoenixHyperlink CreateHyperlink(Uri uri)
		{
			var builder = new PhoenixUriBuilder(uri);
			var title = builder.Parameters["title"];

			var iconParameter = builder.Parameters["icon"];
			Uri icon = null;
			
#warning Implement using default uri provider
            //if (!Uri.TryCreate(iconParameter, UriKind.Absolute, out icon)
            //    && this._securityService.ConnectedServer != null)
            //{
            //    Uri relativeIconUri;
            //    if (Uri.TryCreate(iconParameter, UriKind.Relative, out relativeIconUri))
            //    {
            //        var mainEndpointUri = this._securityService.ConnectedServer.MainEndpoint.Uri;
            //        icon = new Uri(mainEndpointUri, relativeIconUri);
            //    }
            //}

			return new PhoenixHyperlink(uri, title, icon);
		}
	}
}
