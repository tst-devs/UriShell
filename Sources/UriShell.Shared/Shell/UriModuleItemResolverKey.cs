using System;
using System.Diagnostics.Contracts;

namespace UriShell.Shell
{
	/// <summary>
	/// Идентифицирует <see cref="IUriModuleItemResolver"/> для заданного сочетания компонентов
	/// module/item в URI представления.
	/// </summary>
	public sealed class UriModuleItemResolverKey
	{
		/// <summary>
		/// Инициализирует новый объект класса <see cref="UriModuleItemResolverKey"/>.
		/// </summary>
		/// <param name="module">Компонент module в URI представления.</param>
		/// <param name="item">Компонент item в URI представления.</param>
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
		/// Задает инвариант класса.
		/// </summary>
		[ContractInvariantMethod]
		private void ContractInvariant()
		{
			Contract.Invariant(!string.IsNullOrWhiteSpace(this.Module));
			Contract.Invariant(!string.IsNullOrWhiteSpace(this.Item));
		}

		/// <summary>
		/// Возвращает компонент module в URI представления.
		/// </summary>
		public string Module
		{
			get;
			private set;
		}

		/// <summary>
		/// Возвращает компонент item в URI представления.
		/// </summary>
		public string Item
		{
			get;
			private set;
		}

		/// <summary>
		/// Определяет, является ли заданный объект равным по значению текущему.
		/// </summary>
		/// <param name="obj">Объект для сравнения с текущим.</param>
		/// <returns>true, если заданный объект является равным по значению текущему;
		/// иначе false.</returns>
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
		/// Возвращает хэш-код данного <see cref="UriModuleItemResolverKey"/>.
		/// </summary>
		/// <returns>Хэш-код данного <see cref="UriModuleItemResolverKey"/>.</returns>
		public override int GetHashCode()
		{
			return this.Module.GetHashCode() ^ this.Item.GetHashCode();
		}

		/// <summary>
		/// Возвращает строковое представление объекта.
		/// </summary>
		/// <returns>Строковое представление объекта.</returns>
		public override string ToString()
		{
			return String.Format("{0}/{1}", this.Module, this.Item);
		}
	}
}
