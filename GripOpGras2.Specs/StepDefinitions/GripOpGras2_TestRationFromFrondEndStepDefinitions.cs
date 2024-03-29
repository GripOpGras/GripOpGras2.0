using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;
using GripOpGras2.Specs.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace GripOpGras2.Specs.StepDefinitions
{
	[Binding]
	public class GripOpGras2_TestRationFromFrondEndStepDefinitions
	{
		private readonly IWebDriver _driver;

		public GripOpGras2_TestRationFromFrondEndStepDefinitions(IWebDriver driver)
		{
			_driver = driver;
		}

		[Given(@"I am on the home page")]
		public void GivenIAmOnTheHomePage()
		{
			WebDriverUtils.NavigateWebDriverToApplication(_driver);
		}

		[Given(
			@"I have (.*) Products and (.*) Supplementary Feedproducts that should be able to make a correct Ration")]
		public void GivenIHaveProductsAndSupplementaryFeedproductsThatShouldBeAbleToMakeACorrectRation(int p0, int p1)
		{
			List<FeedProduct> products = new()
			{
				new Roughage { Available = true, Name = "Prod1", FeedAnalysis = new FeedAnalysis { Vem = 861, Re = 161 } },
				new Roughage { Available = true, Name = "Prod2", FeedAnalysis = new FeedAnalysis { Vem = 960, Re = 82 } },
				new Roughage { Available = true, Name = "Prod3", FeedAnalysis = new FeedAnalysis { Vem = 835, Re = 140 } },
				new Roughage { Available = true, Name = "Prod4", FeedAnalysis = new FeedAnalysis { Vem = 928, Re = 120 } },
				new Roughage { Available = true, Name = "Prod5", FeedAnalysis = new FeedAnalysis { Vem = 874, Re = 156 } }
			};

			List<FeedProduct> supplementaryFeedProducts = new()
			{
				new SupplementaryFeedProduct
				{
					Available = true, Name = "Bijproduct1", FeedAnalysis = new FeedAnalysis { Vem = 1037, Re = 95 }
				},
				new SupplementaryFeedProduct
				{
					Available = true, Name = "Bijprod2",
					FeedAnalysis = new FeedAnalysis { Vem = 1219.29f, Re = 117.14f }
				},
				new SupplementaryFeedProduct { Available = true, Name = "Bijprod3", FeedAnalysis = new FeedAnalysis { Vem = 1240, Re = 121 } },
				new SupplementaryFeedProduct { Available = true, Name = "Bijprod4", FeedAnalysis = new FeedAnalysis { Vem = 1130, Re = 105 } },
				new SupplementaryFeedProduct { Available = true, Name = "Bijprod5", FeedAnalysis = new FeedAnalysis { Vem = 1320, Re = 130 } }
			};

			List<FeedProduct> chosenFeedProducts = new();

			chosenFeedProducts.AddRange(products.GetRange(0, p0));

			chosenFeedProducts.AddRange(supplementaryFeedProducts.GetRange(0, p1));
			foreach (FeedProduct product in chosenFeedProducts)
			{
				IWebElement nameInput = _driver.FindElement(By.Id("Name"));
				nameInput.SendKeys(product.Name);
				IWebElement vemInput = _driver.FindElement(By.Id("VEM"));
				vemInput.SendKeys(product.FeedAnalysis.Vem.ToString());
				IWebElement reInput = _driver.FindElement(By.Id("RE"));
				reInput.SendKeys(product.FeedAnalysis.Re.ToString());
				IWebElement rationInputSpan = _driver.FindElement(By.Id($"{product.GetType().Name}"));
				IWebElement rationInput = rationInputSpan.FindElement(By.ClassName("valid"));
				rationInput.Click();
				IWebElement addButton = _driver.FindElement(By.Id("submit_feedproduct"));
				addButton.Click();
			}
		}

		[Given(@"I have a herd with (.*) cows which have produced a total of (.*) L milk")]
		public void GivenIHaveAHerdWithCowsWhichHaveProducedATotalOfLMilk(float p0, float p1)
		{
			IWebElement numberOfCowsInput = _driver.FindElement(By.Id("NumberOfCows"));
			numberOfCowsInput.SendKeys(p0.ToString());
			IWebElement lmilkInput = _driver.FindElement(By.Id("milkProduced"));
			lmilkInput.SendKeys(p1.ToString());
		}

		[Given(@"the herd has grazed (.*) KG Dm of grass with (.*) VEM and (.*) Re per Kg Dm")]
		public void GivenTheHerdHasGrazedKgDmOfGrass(float p0, float p1, float p2)
		{
			IWebElement grassIntakeInput = _driver.FindElement(By.Id("grassInputInKgDm"));
			grassIntakeInput.SendKeys(p0.ToString());
			IWebElement grassVEMInput = _driver.FindElement(By.Id("grassVem"));
			grassVEMInput.SendKeys(p1.ToString());
			IWebElement grassREInput = _driver.FindElement(By.Id("grassRe"));
			grassREInput.SendKeys(p2.ToString());
		}

		[When(@"I request the ration")]
		public void WhenIRequestTheRation()
		{
			IWebElement createRationButton = _driver.FindElement(By.Id("CreateRationButton"));
			Thread.Sleep(TimeSpan.FromSeconds(1));

			createRationButton.Click();
		}

		[Then(@"the page should contain a Ration within (.*) seconds")]
		public void ThenThePageShouldContainARationWithinSeconds(float p0)
		{
			WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(p0));
			wait.Until(drv => drv.FindElement(By.Id("RationResult")));
		}

		[Then(@"The ration should contain (.*) products")]
		public void ThenTheRationShouldContainProducts(int p0)
		{
			ReadOnlyCollection<IWebElement>? productResults = _driver.FindElements(By.Id("RationProductName"));
			Assert.AreEqual(p0, productResults.Count);
		}

		[Then(@"The ration should contain (.*) KG grass")]
		public void ThenTheRationShouldContainKGGrass(int p0)
		{
			IWebElement KgGrassResult = _driver.FindElement(By.Id("KGDmGrassResult"));
			Assert.AreEqual(p0.ToString(), KgGrassResult.Text);
		}

		[Then(@"the page should contain an error message within (.*) seconds")]
		public void ThenThePageShouldContainANErrorMessage(float p0)
		{
			WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(p0));
			IWebElement errorElement = wait.Until(drv => drv.FindElement(By.Id("errorCard")));
		}
	}
}