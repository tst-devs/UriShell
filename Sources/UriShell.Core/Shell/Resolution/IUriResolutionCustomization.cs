using System.Collections.Generic;
using System.Diagnostics.Contracts;

using Autofac.Features.Indexed;

namespace UriShell.Shell.Resolution
{
	/// <summary>
	/// Предоставляет настраиваемые компоненты, участвующие в процессе открытия URI.
	/// </summary>
	[ContractClass(typeof(IUriResolutionCustomizationContract))]
	internal interface IUriResolutionCustomization
	{
		/// <summary>
		/// Возвращает список сервисов, умеющих создавать объекты по заданному URI,
		/// зарегистрированные через <see cref="UriModuleItemResolverKey"/>.
		/// </summary>
		IIndex<UriModuleItemResolverKey, IUriModuleItemResolver> ModuleItemResolvers
		{
			get;
		}

		/// <summary>
		/// Возвращает список сервисов, определяющих размещение объектов по заданному URI.
		/// </summary>
		IEnumerable<IUriPlacementResolver> PlacementResolvers
		{
			get;
		}
	}
}
