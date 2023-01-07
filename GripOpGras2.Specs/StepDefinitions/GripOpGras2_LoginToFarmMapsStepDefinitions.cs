using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GripOpGras2.Specs.StepDefinitions
{
	[Binding]
	public class GripOpGras2_LoginToFarmMapsStepDefinitions
	{
		private IWebDriver? _driver;

		private const string Url = "http://localhost:4200";

		[When(@"I open the Grip op Gras application")]
		public void WhenIOpenTheGripOpGrasApplication()
		{
			string projectPath = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
			_driver = new ChromeDriver(projectPath + @"\Drivers\");

			try
			{
				_driver.Navigate().GoToUrl(Url);
			}
			catch (WebDriverException exception)
			{
				if (exception.Message.Contains("ERR_CONNECTION_REFUSED"))
				{
					throw new Exception(
						$"The application is not running on {Url}. Please start the application and try again.");
				}

				throw;
			}

			_driver.Manage().Window.Maximize();
			_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
		}

		[Then(@"the home page is loaded")]
		public void ThenTheHomePageIsLoaded()
		{
			IWebElement? navMenu = _driver.FindElement(By.XPath("//div[contains(@class, 'nav-item')]"));
			navMenu.Should().NotBeNull();
		}

		[AfterScenario]
		public void AfterScenario()
		{
			_driver?.Quit();
		}
	}
}