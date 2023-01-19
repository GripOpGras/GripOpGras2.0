using GripOpGras2.Specs.Data;
using GripOpGras2.Specs.Data.Exceptions.SeleniumExceptions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GripOpGras2.Specs.Utils
{
	internal class WebDriverUtils
	{
		private const string BaseUrl = "http://localhost:4200";

		public static void NavigateWebDriverToApplication(IWebDriver driver, string uri = "/")
		{
			try
			{
				driver.Navigate().GoToUrl(BaseUrl + uri);
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

		public static void LoginToApplication(IWebDriver driver, FarmMapsTestAccount farmMapsTestAccount,
			TimeSpan pageLoadDelay)
		{
			driver.FindElement(By.ClassName("login-page"));

			if (!driver.Url.Contains("farmmaps.eu"))
			{
				throw new UnexpectedPageUrlException(driver.Url, "login page");
			}

			driver.FindElement(By.Id("Username")).SendKeys(farmMapsTestAccount.Username);
			driver.FindElement(By.Id("Password")).SendKeys(farmMapsTestAccount.Password);
			driver.FindElement(By.ClassName("btn-primary")).Click();

			try
			{
				new WebDriverWait(driver, pageLoadDelay).Until(d => d.PageSource.Contains(farmMapsTestAccount.Username));
			}
			catch (WebDriverTimeoutException)
			{
				throw new SeleniumException("Could not verify that the user is logged in.");
			}
		}
	}
}