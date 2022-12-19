using GripOpGras2.Specs.Drivers;

namespace GripOpGras2.Specs.StepDefinitions
{
	/// <summary>
	/// Generic "Then" steps used to verify if an exception has been thrown. 
	/// </summary>
	[Binding]
	internal class ErrorSteps
	{
		private readonly ExceptionDriver _exceptionDriver;

		public ErrorSteps(ExceptionDriver exceptionDriver)
		{
			_exceptionDriver = exceptionDriver;
		}

		[Then(@"an exception with the message '(.*)' should be thrown")]
		public void ThenTheExceptionWithMessageShouldBeThrown(string expectedMessage)
		{
			_exceptionDriver.AssertExceptionWasThrownWithMessage(expectedMessage);
		}

		[AfterScenario]
		public void CheckForUnexpectedExceptionsAfterEachScenario()
		{
			_exceptionDriver.AssertNoUnexpectedExceptionsRaised();
		}
	}
}
