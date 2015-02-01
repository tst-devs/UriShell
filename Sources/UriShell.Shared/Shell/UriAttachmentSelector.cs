using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell.Shell
{
	/// <summary>
	/// Представляет метод, позволяющий получить объект, прикрепеленный к URI через параметры.
	/// </summary>
	/// <param name="id">Идентификатор прикрепленного объекта, переданный в параметре URI.</param>
	/// <returns>Объект, прикрепленный к URI, если его идентификатор был найден; иначе null.</returns>
	public delegate object UriAttachmentSelector(string id);
}
