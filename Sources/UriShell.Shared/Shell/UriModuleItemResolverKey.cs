using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Identifies the <see cref="IUriModuleItemResolver"/> for the given combination of 
	/// "module/item" components in a URI of a view.
	/// </summary>
	public sealed class UriModuleItemResolverKey
	{
		/// <summary>
		/// Initializes a new instance of the class <see cref="UriModuleItemResolverKey"/>.
		/// </summary>
		/// <param name="module">The component "module" in the URI of a view.</param>
		/// <param name="item">The component "item" in the URI of a view.</param>
		public UriModuleItemResolverKey(string module, string item)
		{
			if (module != null)
			{
				this.Module = module.ToLower();
			}
			if (item != null)
			{
				this.Item = item.ToLower();
			}
		}

		/// <summary>
		/// Describes the invariant of the class.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(!string.IsNullOrWhiteSpace(this.Module));
			Contract.Invariant(!string.IsNullOrWhiteSpace(this.Item));
		}

		/// <summary>
		/// Gets the component "module" in the URI of the view
		/// </summary>
		public string Module
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the component "item" in the URI of the view
		/// </summary>
		public string Item
		{
			get;
			private set;
		}

		/// <summary>
		/// Determines if the given object is equal to this.
		/// </summary>
		/// <param name="obj">The object for comparison with this.</param>
		/// <returns>true, if the given object is equal to this;
		/// otherwise false.</returns>
		public override bool Equals(object obj)
		{
			if (obj != null && obj.GetType() == this.GetType())
			{
				var other = (UriModuleItemResolverKey)obj;

				if (this.Module != other.Module)
				{
					return false;
				}

				if (this.Item != other.Item)
				{
					return false;
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the hash-code of the <see cref="UriModuleItemResolverKey"/>.
		/// </summary>
		/// <returns>The hash-code of the <see cref="UriModuleItemResolverKey"/>.</returns>
		public override int GetHashCode()
		{
			return this.Module.GetHashCode() ^ this.Item.GetHashCode();
		}

		/// <summary>
		/// Gets the string representation of the object.
		/// </summary>
		/// <returns>The string representation of the object</returns>
		public override string ToString()
		{
			return String.Format("{0}/{1}", this.Module, this.Item);
		}
	}
}
