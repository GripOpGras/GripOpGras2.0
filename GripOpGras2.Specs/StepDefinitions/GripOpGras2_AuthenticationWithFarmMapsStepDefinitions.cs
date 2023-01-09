using GripOpGras2.Specs.Data;
using GripOpGras2.Specs.Data.Exceptions.SeleniumExceptions;
using GripOpGras2.Specs.Data.Exceptions.SpecFlowTestExceptions;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace GripOpGras2.Specs.StepDefinitions
{
	[Binding]
	public class GripOpGras2_AuthenticationWithFarmMapsStepDefinitions
	{
		private readonly IWebDriver _driver;

		private const string BaseUrl = "http://localhost:4200";

		private readonly FarmMapsTestAccount _farmMapsTestAccount;

		private readonly TimeSpan _pageLoadDelay = TimeSpan.FromSeconds(4);

		public GripOpGras2_AuthenticationWithFarmMapsStepDefinitions(IWebDriver driver)
		{
			_driver = driver;

			IConfigurationRoot? config = new ConfigurationBuilder().AddUserSecrets<FarmMapsTestAccount>().Build();
			FarmMapsTestAccount? account = config.GetSection(nameof(FarmMapsTestAccount)).Get<FarmMapsTestAccount>();
			_farmMapsTestAccount = account ?? throw new MissingUserSecretsException(nameof(FarmMapsTestAccount));
		}

		[When(@"I open the Grip op Gras application")]
		public void WhenIOpenTheGripOpGrasApplication()
		{
			NavigateWebDriverToApplication();
		}

		[Then(@"the application should navigate to the FarmMaps login page")]
		public void ThenTheApplicationShouldNavigateToTheFarmMapsLoginPage()
		{
			IWebElement loginPage = _driver.FindElement(By.ClassName("login-page"));
			loginPage.Should().NotBeNull();

			IWebElement loginForm = loginPage.FindElement(By.TagName("form"));
			loginForm.Should().NotBeNull();
			loginForm.GetAttribute("method").Should().Be("post");

			// Should be checked as last to allow the webdriver to load the page
			_driver.Url.Should().Contain("farmmaps.eu");
		}

		[Given(@"that I am on the FarmMaps login page")]
		public void GivenThatIAmOnTheFarmMapsLoginPage()
		{
			NavigateWebDriverToApplication();

			// Give the page time to load
			_driver.FindElement(By.ClassName("login-page"));

			if (!_driver.Url.Contains("farmmaps.eu"))
			{
				throw new UnexpectedPageUrlException(_driver.Url, "login page");
			}
		}

		[When(@"I enter my username and password")]
		public void WhenIEnterMyUsernameAndPassword()
		{
			IWebElement usernameInput = _driver.FindElement(By.Id("Username"));
			usernameInput.SendKeys(_farmMapsTestAccount.Username);
			IWebElement passwordInput = _driver.FindElement(By.Id("Password"));
			passwordInput.SendKeys(_farmMapsTestAccount.Password);
		}

		[When(@"I click the login button")]
		public void WhenIClickTheLoginButton()
		{
			IWebElement loginButton = _driver.FindElement(By.ClassName("btn-primary"));
			loginButton.Click();
		}

		[Then(@"I will have to be redirected to the home page of the application")]
		public void ThenIWillHaveToBeRedirectedToTheHomePageOfTheApplication()
		{
			Thread.Sleep(_pageLoadDelay);

			_driver.Url.Should().Be(BaseUrl + "/");
		}

		[Then(@"the page should show my email address")]
		public void ThenThePageShouldShowMyEmailAddress()
		{
			_driver.PageSource.Should().Contain(_farmMapsTestAccount.Username);
		}

		[When(@"I enter an incorrect username and password")]
		public void WhenIEnterAnIncorrectUsernameAndPassword()
		{
			string guid = Guid.NewGuid().ToString();
			IWebElement usernameInput = _driver.FindElement(By.Id("Username"));
			usernameInput.SendKeys(guid);
			IWebElement passwordInput = _driver.FindElement(By.Id("Password"));
			passwordInput.SendKeys(guid);
		}

		[Then(@"the login page should show me an error message")]
		public void ThenTheLoginPageShouldShowMeAnErrorMessage()
		{
			_driver.PageSource.Should().Contain("Invalid username or password");
			_driver.Url.Should().Contain("farmmaps.eu");
		}

		[Given(@"that I am logged into the application")]
		public void GivenThatIAmLoggedIntoTheApplication()
		{
			GivenThatIAmOnTheFarmMapsLoginPage();
			WhenIEnterMyUsernameAndPassword();
			WhenIClickTheLoginButton();
			ThenIWillHaveToBeRedirectedToTheHomePageOfTheApplication();
			ThenThePageShouldShowMyEmailAddress();
		}

		[Given(@"that I am currently on the home page")]
		public void GivenThatIAmCurrentlyOnTheHomePage()
		{
			NavigateWebDriverToApplication();
			Thread.Sleep(_pageLoadDelay);
			if (_driver.Url != BaseUrl + "/")
			{
				throw new UnexpectedPageUrlException(_driver.Url, "home page");
			}
		}

		[When(@"I click the logout button")]
		public void WhenIClickTheLogoutButton()
		{
			IWebElement logoutButton = _driver.FindElement(By.ClassName("oi-account-logout"));
			logoutButton.Click();

			// Button from the FarmMaps environment
			IWebElement logoutConfirmButton = _driver.FindElement(By.ClassName("btn-primary"));
			logoutConfirmButton.Click();
		}

		[Then(@"I should be logged out of the application")]
		public void ThenIShouldBeLoggedOutOfTheApplication()
		{
			NavigateWebDriverToApplication();

			IWebElement loginPage = _driver.FindElement(By.ClassName("login-page"));
			loginPage.Should().NotBeNull();

			// Should be checked as last to allow the webdriver to load the page
			_driver.Url.Should().Contain("farmmaps.eu");
		}

		[When(@"I visit the test page")]
		public void WhenIVisitTheTestPage()
		{
			NavigateWebDriverToApplication("/testpage");
			Thread.Sleep(_pageLoadDelay);
		}

		[Then(@"the page should show my farms")]
		public void ThenThePageShouldShowMyFarms()
		{
			IWebElement farmOverview = _driver.FindElement(By.Id("farmoverview"));
			farmOverview.Should().NotBeNull();
		}

		[When(@"I visit a page that doesn't exist")]
		public void WhenIVisitAPageThatDoesntExist()
		{
			NavigateWebDriverToApplication("/this-is-a-page-that-doesnt-exist");
			Thread.Sleep(_pageLoadDelay);
		}


		private void NavigateWebDriverToApplication(string uri = "/")
		{
			try
			{
				_driver.Navigate().GoToUrl(BaseUrl + uri);
			}
			catch (WebDriverException exception)
			{
				if (exception.Message.Contains("ERR_CONNECTION_REFUSED"))
				{
					throw new SeleniumException(
						$"The application is not running on {BaseUrl}. Start the application and then try again.");
				}

				throw;
			}
		}
	}
}