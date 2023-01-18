using BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GripOpGras2.Specs.Hooks
{
	[Binding]
	public class WebDriverHooks
	{
		private readonly IObjectContainer _container;

		public WebDriverHooks(IObjectContainer container)
		{
			_container = container;
		}

		[BeforeScenario]
		public void CreateWebDriver()
		{
			ChromeOptions chromeOptions = new();
			chromeOptions.AddArgument("headless");
			string projectPath = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
			ChromeDriver driver = new(projectPath + @"\Drivers\", chromeOptions);
			driver.Manage().Window.Maximize();
			driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

			_container.RegisterInstanceAs<IWebDriver>(driver);
		}

		[AfterScenario]
		public void DestroyWebDriver()
		{
			IWebDriver? driver = _container.Resolve<IWebDriver>();

			if (driver != null)
			{
				driver.Quit();
				driver.Dispose();
			}
		}
	}
}