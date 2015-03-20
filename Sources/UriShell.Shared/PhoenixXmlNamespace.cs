using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell
{
    /// <summary>
	/// Describes strings for the Phoenix XML namespace.
    /// </summary>
	[Obsolete("UriShell should be completely separated from the Phoenix application.")]
    public static class PhoenixXmlNamespace
    {
        /// <summary>
        /// The Phoenix XML namespace.
        /// </summary>
        public const string Value = "http://www.transsys.ru/phoenix";

        /// <summary>
		/// The recommended prefix of the Phoenix XML namespace.
        /// </summary>
        public const string Prefix = "phoenix";
    }
}
