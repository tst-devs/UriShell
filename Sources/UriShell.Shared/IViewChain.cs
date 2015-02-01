using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace TST.Phoenix.Arm
{
	/// <summary>
	/// Интерфейс цепочки, позволяющей представлению 
	/// указывать свои внутренние представления. 
	/// </summary>
	[ContractClass(typeof(IViewChainContract))]
	public interface IViewChain
	{
		/// <summary>
		/// Выполняет добавление представления в цепочку представлений.
		/// </summary>
		/// <param name="view">Представление, которое добавляется в цепочку.</param>
		void Add(object view);
	}
}
