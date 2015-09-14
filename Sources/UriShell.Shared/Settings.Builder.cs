using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriShell
{
	partial class Settings
	{
		/// <summary>
		/// Builder of the <see cref="Settings"/>.
		/// </summary>
		public class Builder
		{
			/// <summary>
			/// Initializes a new object <see cref="Builder"/>.
			/// </summary>
			public Builder()
			{
				this.Scheme = "urishell";
			}

			/// <summary>
			/// Gets or sets the URI scheme.
			/// </summary>
			public string Scheme
			{
				get;
				set;
			}
		}
	}
}
