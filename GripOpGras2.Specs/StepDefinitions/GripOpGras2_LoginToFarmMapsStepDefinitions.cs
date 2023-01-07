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
			ChromeOptions chromeOptions = new();
			chromeOptions.AddArgument("headless");
			string projectPath = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
			_driver = new ChromeDriver(projectPath + @"\Drivers\", chromeOptions);

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

		[Then(@"the application should navigate to the FarmMaps login page")]
		public void ThenTheApplicationShouldNavigateToTheFarmMapsLoginPage()
		{
			IWebElement loginPage = _driver!.FindElement(By.ClassName("login-page"));
			loginPage.Should().NotBeNull();

			IWebElement loginForm = loginPage.FindElement(By.TagName("form"));
			loginForm.Should().NotBeNull();
			loginForm.GetAttribute("method").Should().Be("post");

			// Should be checked as last to allow the webdriver to load the page
			_driver.Url.Should().Contain("farmmaps.eu");
		}


		[AfterScenario]
		public void AfterScenario()
		{
			_driver?.Quit();
		}
	}
}