using System;
using System.Reflection;

namespace UriShell.Samples.TabApp.Input
{
	public class WeakDelegateReference
	{
		private readonly MethodInfo _delegateMethod;

		private readonly Type _delegateType;

		private readonly WeakReference _delegateTarget;

		public WeakDelegateReference(Delegate handler)
		{
			this._delegateMethod = handler.Method;
			this._delegateType = handler.GetType();

			if (!handler.Method.IsStatic)
			{
				this._delegateTarget = new WeakReference(handler.Target);
			}
		}

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
