using NUnit.Framework;
using System.Diagnostics;

namespace GripOpGras2.Specs.Drivers
{
	/// <summary>
	/// Used to catch and assert exceptions.
	/// Based on https://ronaldbosma.github.io/blog/2021/05/31/handling-exceptions-in-specflow/
	/// </summary>
	public class ExceptionDriver
	{
		private readonly Queue<Exception> _exceptions = new();

		public void TryExecute(Action act)
		{
			try
			{
				act();
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"The following exception was caught while executing {act.Method.Name}: {ex}");
				_exceptions.Enqueue(ex);
			}
		}

		public void AssertExceptionWasThrownWithMessage(string expectedExceptionMessage)
		{
			Assert.IsTrue(_exceptions.Any(), $"No exception was raised but expected exception with message: {expectedExceptionMessage}");

			Exception actualException = _exceptions.Dequeue();
			Assert.That(actualException.Message, Does.Contain(expectedExceptionMessage));
		}

		public void AssertNoUnexpectedExceptionsRaised()
		{
			if (_exceptions.Any())
			{
				Exception unexpectedException = _exceptions.Dequeue();
				Assert.Fail($"No exception was expected to be raised but found exception: {unexpectedException}");
			}
		}
	}
}
