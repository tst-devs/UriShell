using System;
using System.Diagnostics.Contracts;

namespace UriShell
{
	/// <summary>
	/// Holds global settings for UriShell.
	/// </summary>
	public partial class Settings
	{
		/// <summary>
		/// A singletone of <see cref="Settings"/>.
		/// </summary>
		private static Settings _Instance;

		/// <summary>
		/// Gets a singletone of <see cref="Settings"/>.
		/// </summary>
		public static Settings Instance
		{
			get
			{
				return Settings._Instance;
			}
		}

		/// <summary>
		/// Initializes a new object <see cref="Settings"/> from the builder.
		/// </summary>
		/// <param name="builder"></param>
		private Settings(Builder builder)
		{
			this.Scheme = builder.Scheme;
		}

		/// <summary>
		/// Initializes the singletone <see cref="Instance"/>.
		/// </summary>
		/// <param name="initialize">Initialization action that accepts a builder.</param>
		public static void Initialize(Action<Settings.Builder> initialize)
		{
			Contract.Requires<InvalidOperationException>(Settings.Instance == null);

			var builder = new Builder();
			initialize(builder);
			Settings._Instance = new Settings(builder);
		}

		/// <summary>
		/// Gets the scheme for UriShell.
		/// </summary>
		public string Scheme
		{
			get;
			private set;
		}
	}
}
