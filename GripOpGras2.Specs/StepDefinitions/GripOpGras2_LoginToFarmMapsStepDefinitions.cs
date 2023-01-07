using GripOpGras2.Specs.Models;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace GripOpGras2.Specs.StepDefinitions
{
	[Binding]
	public class GripOpGras2_LoginToFarmMapsStepDefinitions
	{
		private readonly IWebDriver _driver;

		private const string BaseUrl = "http://localhost:4200";

		private readonly FarmMapsTestAccount _farmMapsTestAccount;

		public GripOpGras2_LoginToFarmMapsStepDefinitions(IWebDriver driver)
		{
			_driver = driver;

			IConfigurationRoot? config = new ConfigurationBuilder().AddUserSecrets<FarmMapsTestAccount>().Build();
			FarmMapsTestAccount? account = config.GetSection(nameof(FarmMapsTestAccount)).Get<FarmMapsTestAccount>();
			_farmMapsTestAccount = account ?? throw new Exception(
				"The FarmMapsTestAccount variable in the secrets.json file has not been configured. See the following link on how to configure this file: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows");
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
				throw new Exception("The application did not navigate to the FarmMaps login page.");
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
			Thread.Sleep(3000);

			_driver.Url.Should().Be(BaseUrl + "/");
		}

		[Then(@"the page should show the users email address")]
		public void ThenThePageShouldShowTheUsersEmailAddress()
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
					throw new Exception(
						$"The application is not running on {BaseUrl}. Please start the application and try again.");
				}

				throw;
			}
		}
	}
}