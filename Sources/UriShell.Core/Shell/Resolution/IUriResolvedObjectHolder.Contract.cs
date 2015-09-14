using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace UriShell.Shell.Resolution
{
	[ContractClassFor(typeof(IUriResolvedObjectHolder))]
	internal abstract class IUriResolvedObjectHolderContract : IUriResolvedObjectHolder
	{
		public void Add(object resolved, UriResolvedMetadata metadata)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);
			Contract.Requires<ArgumentNullException>(metadata.Uri != null);
			Contract.Requires<ArgumentNullException>(metadata.Disposable != null);
		}

		public void Remove(object resolved)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);
		}

		public abstract bool Contains(object resolved);

		public object Get(int id)
		{
			Contract.Requires<ArgumentOutOfRangeException>(id >= ShellUriBuilder.MinResolvedId);
			Contract.Requires<ArgumentOutOfRangeException>(id <= ShellUriBuilder.MaxResolvedId);

			return default(object);
		}

		public UriResolvedMetadata GetMetadata(object resolved)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);

			Contract.Ensures(Contract.Result<UriResolvedMetadata>().ResolvedId >= ShellUriBuilder.MinResolvedId);
			Contract.Ensures(Contract.Result<UriResolvedMetadata>().ResolvedId <= ShellUriBuilder.MaxResolvedId);

			return default(UriResolvedMetadata);
		}

		public void ModifyMetadata(object resolved, Uri overrideUri)
		{
			Contract.Requires<ArgumentNullException>(resolved != null);
			Contract.Requires<ArgumentNullException>(overrideUri != null);
		}

		#region IEnumerable Contracts

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
