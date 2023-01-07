using GripOpGras2.Specs.Models;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace GripOpGras2.Specs.StepDefinitions
{
	[Binding]
	public class GripOpGras2_LoginToFarmMapsStepDefinitions
	{
		private readonly IWebDriver _driver;

		private const string Url = "http://localhost:4200";

		private readonly FarmMapsTestAccount _farmMapsTestAccount;

		public GripOpGras2_LoginToFarmMapsStepDefinitions(IWebDriver driver)
		{
			_driver = driver;

			IConfigurationRoot? config = new ConfigurationBuilder().AddUserSecrets<FarmMapsTestAccount>().Build();
			FarmMapsTestAccount? account = config.GetSection(nameof(FarmMapsTestAccount)).Get<FarmMapsTestAccount>();
			_farmMapsTestAccount = account ?? throw new Exception(
				"The FarmMapsTestAccount variable in the secrets.json file has not been configured. See the following link on how to configure this file: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows");

			// Initialize Selenium page object
			//this.loginPage = new LoginPage(driver);
		}

		[When(@"I open the Grip op Gras application")]
		public void WhenIOpenTheGripOpGrasApplication()
		{
			NavigateWebDriverToApplication();
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

		[Given(@"the user is on the login page of FarmMaps")]
		public void GivenTheUserIsOnTheLoginPageOfFarmMaps()
		{
			NavigateWebDriverToApplication();

			// Give the page time to load
			_driver!.FindElement(By.ClassName("login-page"));

			if (!_driver.Url.Contains("farmmaps.eu"))
			{
				throw new Exception("The application did not navigate to the FarmMaps login page.");
			}
		}

		[When(@"the user enters the username and password")]
		public void WhenTheUserEntersTheUsernameAndPassword()
		{
			IWebElement usernameInput = _driver.FindElement(By.Id("Username"));
			usernameInput.SendKeys(_farmMapsTestAccount.Username);
			IWebElement passwordInput = _driver.FindElement(By.Id("Password"));
			passwordInput.SendKeys(_farmMapsTestAccount.Password);
		}

		[When(@"the user clicks the login button")]
		public void WhenTheUserClicksTheLoginButton()
		{
			IWebElement loginButton = _driver!.FindElement(By.ClassName("btn-primary"));
			loginButton.Click();
		}

		[Then(@"the application should navigate to the FarmMaps dashboard")]
		public void ThenTheApplicationShouldNavigateToTheFarmMapsDashboard()
		{
			Thread.Sleep(3000);

			_driver.Url.Should().Be(Url + "/");
		}

		[Then(@"the application should show the users email address")]
		public void ThenTheApplicationShouldShowTheUsersEmailAddress()
		{
			_driver.PageSource.Should().Contain(_farmMapsTestAccount.Username);
		}

		private void NavigateWebDriverToApplication()
		{
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
		}
	}
}