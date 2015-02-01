using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UriShell.Tests
{
	/// <summary>
	/// Позволяет контролировать то, что генерация исключения состоялась.
	/// </summary>
	public class ExceptionAssert
	{
		/// <summary>
		/// Проверяет, что указанное действие генерирует исключение указанного типа.
		/// </summary>
		/// <typeparam name="T">Тип исключения.</typeparam>
		/// <param name="testAction">Действие, которое генерирует исключение.</param>
		public static void Throws<T>(Action testAction)
			where T : Exception
		{
			ExceptionAssert.Throws<T>(default(Predicate<T>), testAction);
		}

		/// <summary>
		/// Проверяет, что указанное действие генерирует заданное исключение.
		/// </summary>
		/// <typeparam name="T">Тип исключения.</typeparam>
		/// <param name="testAction">Действие, которое генерирует исключение.</param>
		/// <param name="exception">Исключение, которое должно быть сгенерировано.</param>
		public static void Throws<T>(T expectedException, Action testAction)
			where T : Exception
		{
			ExceptionAssert.Throws<T>(e => e == expectedException, testAction);
		}

		/// <summary>
		/// Проверяет, что указанное действие генерирует исключение, соответствующее
		/// заданным условиям.
		/// </summary>
		/// <typeparam name="T">Тип исключения.</typeparam>
		/// <param name="expectedExceptionFilter">Предикат для проверки дополнительных параметров исключения. 
		/// Если возвращает false, метод считает, что сгенерировалось неверное исключение.</param>
		/// <param name="testAction">Действие, которое генерирует исключение.</param>
		public static void Throws<T>(Predicate<T> expectedExceptionFilter, Action testAction)
			where T : Exception
		{
			try
			{
				testAction();
			}
			catch (Exception ex)
			{
				if (typeof(T).IsAssignableFrom(ex.GetType()))
				{
					if (expectedExceptionFilter == null)
					{
						return;
					}

					if (expectedExceptionFilter((T)ex))
					{
						return;
					}
				}

				Assert.Fail("Вызов завершился с исключением, не соответствующим заданным условиям.");
			}

			Assert.Fail("Вызов завершился без исключения.");
		}
	}
}
