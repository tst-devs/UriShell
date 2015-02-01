using System;
using System.Reflection;

namespace UriShell
{
	/// <summary>
	/// Хранит ссылку на метод со слабой ссылкой на вызываемый объект,
	/// в случае экземплярного метода.
	/// </summary>
	public class WeakDelegateReference
	{
		/// <summary>
		/// Ссылка на метод, вызываемый делегатом.
		/// </summary>
		private readonly MethodInfo _delegateMethod;

		/// <summary>
		/// Тип метода, вызываемого делегатом.
		/// </summary>
		private readonly Type _delegateType;

		/// <summary>
		/// Ссылка на объект, метод которого вызывается делегатом в случае экземлярного метода.
		/// </summary>
		private readonly WeakReference _delegateTarget;

		/// <summary>
		/// Инициализирует новый объект класса <see cref="WeakDelegateReference"/>.
		/// </summary>
		/// <param name="handler">Делегат, для которого создается ссылка.</param>
		public WeakDelegateReference(Delegate handler)
		{
			this._delegateMethod = handler.Method;
			this._delegateType = handler.GetType();

			if (!handler.Method.IsStatic)
			{
				this._delegateTarget = new WeakReference(handler.Target);
			}
		}

		/// <summary>
		/// Получает делегат, на который ссылается <see cref="WeakDelegateReference"/>, если возможен его вызов.
		/// </summary>
		/// <returns><langword>true</langword>, если вызов делагата возможен; иначе <langword>false</langword>.</returns>
		public bool TryGetDelegate(out Delegate handler)
		{
			object target = null;

			// Если метод нестатический, пробуем получить строгую ссылку.
			// В случае, когда это не удалось, возвращаем false.
			if (!this._delegateMethod.IsStatic)
			{
				target = this._delegateTarget.Target;
				if (target == null)
				{
					handler = null;
					return false;
				}
			}

			handler = Delegate.CreateDelegate(this._delegateType, target, this._delegateMethod);
			return true;
		}
	}
}
