using System;
using System.Reflection;

namespace UriShell
{
	/// <summary>
	/// Holds a method reference with a weak target object's reference 
	/// in case of an instance method.
	/// </summary>
	public class WeakDelegateReference
	{
		/// <summary>
		/// The reference to a method invoked by the delegate.
		/// </summary>
		private readonly MethodInfo _delegateMethod;

		/// <summary>
		/// The type of a method invoked by the delegate.
		/// </summary>
		private readonly Type _delegateType;

		/// <summary>
		/// The reference to an object, whose method is invoked in case of an instance method.
		/// </summary>
		private readonly WeakReference _delegateTarget;

		/// <summary>
		/// Initializes a new object of the class <see cref="WeakDelegateReference"/>.
		/// </summary>
		/// <param name="handler">The delegate to be held with a weak reference.</param>
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
		/// Gets the delegate held by the <see cref="WeakDelegateReference"/>, if it could be invoked.
		/// </summary>
		/// <param name="handler">The delegate held by the <see cref="WeakDelegateReference"/>, 
		/// if it could be invoked.</param>
		/// <returns>true, if the delegate could be invoked; otherwise false.</returns>
		public bool TryGetDelegate(out Delegate handler)
		{
			object target = null;

			// If the method is non-static, try to get a strong reference.
 			// Return false on failure.
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
