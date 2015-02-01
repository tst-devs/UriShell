using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using TST.Onyx.Arm.Utility;

namespace TST.Onyx.Arm.Themes
{
	/// <summary>
	/// Определяет внешний вид приложения по умолчанию.
	/// </summary>
	public class DefaultTheme : OnyxTheme
	{
		/// <summary>
		/// Загружает словари ресурсов темы по умолчанию (DefaultTheme).
		/// </summary>
		/// <returns>Список <see cref="ResourceDictionary"/>, содержащих ресурсы темы.</returns>
		protected override IEnumerable<ResourceDictionary> LoadThemeDictionaries()
		{
			return new ResourceDictionary[]
			{ 
				new ResourceDictionary()
				{
					Source = this.ComputeResourceUri(typeof(ApplicationLook).Assembly, "Themes/DefaultTheme.xaml")
				},
			};
		}
	}
}
