using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Describes a hyperlink for opening the URI from UI.
	/// </summary>
	public class PhoenixHyperlink
	{
		/// <summary>
		/// The URI which is opened by the hyperlink.
		/// </summary>
		private readonly Uri _uri; 

		/// <summary>
		/// The text of the hyperlink.
		/// </summary>
		private readonly string _text;

		/// <summary>
		/// The URI of an icon displayed in the hyperlink.
		/// </summary>
		private readonly Uri _icon;

		/// <summary>
		/// Initializes a new instance of the  class <see cref="PhoenixHyperlink"/>.
		/// </summary>
		/// <param name="uri">The URI which is opened by the hyperlink.</param>
		public PhoenixHyperlink(Uri uri)
			: this(uri, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the  class <see cref="PhoenixHyperlink"/>.
		/// </summary>
		/// <param name="uri">The URI which is opened by the hyperlink.</param>
		/// <param name="text">The text of the hyperlink.</param>
		public PhoenixHyperlink(Uri uri, string text)
			: this(uri, text, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the  class <see cref="PhoenixHyperlink"/>.
		/// </summary>
		/// <param name="uri">The URI which is opened by the hyperlink.</param>
		/// <param name="text">The text of the hyperlink.</param>
		/// <param name="icon">The URI of an icon displayed in the hyperlink.</param>
		public PhoenixHyperlink(Uri uri, string text, Uri icon)
		{
			this._uri = uri;
			this._text = text;
			this._icon = icon;
		}

		/// <summary>
		/// Describes the invariant of the class.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(this.Uri != null);
			Contract.Invariant(!string.IsNullOrWhiteSpace(this.Text));
		}

		/// <summary>
		/// Gets the URI which is opened by the hyperlink.
		/// </summary>
		public Uri Uri
		{
			get
			{
				return this._uri;
			}
		}

		/// <summary>
		/// Gets the text of the hyperlink.
		/// </summary>
		public string Text
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this._text))
				{
					return this._uri.ToString();
				}

				return this._text;
			}
		}

		/// <summary>
		/// Gets the URI of an icon displayed in the hyperlink.
		/// </summary>
		public Uri Icon
		{
			get
			{
				return this._icon;
			}
		}
	}
}
