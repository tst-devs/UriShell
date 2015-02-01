using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using UriShell.Shell.Registration;

namespace UriShell.Shell
{
	[ContractClassFor(typeof(IShell))]
	internal abstract class IShellContract : IShell
	{
		public void AddUriPlacementResolver(IUriPlacementResolver uriPlacementResolver)
		{
			Contract.Requires<ArgumentNullException>(uriPlacementResolver != null);
		}

		public IShellResolve Resolve(Uri uri, params object[] attachments)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentNullException>(attachments != null);

			Contract.Ensures(Contract.Result<IShellResolve>() != null);

			return default(IShellResolve);
		}

		public bool IsResolvedOpen(object resolved)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);

			return default(bool);
		}

		public int GetResolvedId(object resolved)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);
			Contract.Requires<ArgumentException>(this.IsResolvedOpen(resolved));
			
			Contract.Ensures(Contract.Result<int>() >= PhoenixUriBuilder.MinResolvedId);
            Contract.Ensures(Contract.Result<int>() <= PhoenixUriBuilder.MaxResolvedId);

			return default(int);
		}

		public Uri GetResolvedUri(object resolved)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);
			Contract.Requires<ArgumentException>(this.IsResolvedOpen(resolved));

			return default(Uri);
		}

		public void CloseResolved(object resolved)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);

			Contract.Ensures(!this.IsResolvedOpen(resolved));
		}

		public PhoenixHyperlink TryParseHyperlink(string hyperlink, int ownerId)
		{
			Contract.Requires<ArgumentNullException>(hyperlink != null);

			Contract.Requires<ArgumentOutOfRangeException>(ownerId >= PhoenixUriBuilder.MinResolvedId);
			Contract.Requires<ArgumentOutOfRangeException>(ownerId <= PhoenixUriBuilder.MaxResolvedId);

			return default(PhoenixHyperlink);
		}

		public PhoenixHyperlink CreateHyperlink(Uri uri)
		{
			Contract.Requires<ArgumentNullException>(uri != null);

			Contract.Ensures(Contract.Result<PhoenixHyperlink>() != null);
			return default(PhoenixHyperlink);
		}
	}
}
