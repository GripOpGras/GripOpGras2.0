using GripOpGras2.Client.Features.CreateRation;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;
using GripOpGras2.Specs.Drivers;

namespace GripOpGras2.Specs.StepDefinitions
{
	[Binding]
	public class GripOpGras2RationAlgorithmStepDefinitions
	{
		private readonly IRationAlgorithm _rationAlgorithm = new RationAlgorithmV1();

		private readonly Herd _herd = new();

		private readonly MilkProductionAnalysis _milkProductionAnalysis = new();

		private GrazingActivity? _grazingActivity;

		private readonly List<FeedProduct> _feedProducts = new();

		private float _totalGrassIntake;

		private FeedRation? _result;

		private readonly ExceptionDriver _exceptionDriver;

		public GripOpGras2RationAlgorithmStepDefinitions(ExceptionDriver exceptionDriver)
		{
			_exceptionDriver = exceptionDriver;
		}

		[Given(@"I have the roughage product (.*) that contains (.*) g protein, and (.*) VEM")]
		public void GivenIHaveTheRoughageProductThatContainsProteinAndVem(string roughageName, float protein, float vem)
		{
			_feedProducts.Add(new Roughage
			{
				Name = roughageName,
				FeedAnalysis = new FeedAnalysis
				{
					Re = protein,
					Vem = vem
				},
				Available = true
			});
		}

		[Given(@"I have the supplementary product (.*) that contains (.*) g protein, and (.*) VEM")]
		public void GivenIHaveTheSupplementaryProductThatContainsDmProteinAndVem(string supplementaryName,
			float protein, float vem)
		{
			_feedProducts.Add(new SupplementaryFeedProduct()
			{
				Name = supplementaryName,
				FeedAnalysis = new FeedAnalysis
				{
					Re = protein,
					Vem = vem
				},
				Available = true
			});
		}

		[Given(@"I have a herd with (.*) cows in it")]
		public void GivenIHaveAHerdWithHerd_SizeCowsInIt(int herdSize)
		{
			_herd.NumberOfAnimals = herdSize;
		}

		[Given(@"I have a herd with (.*) cows in it, which have taken in (.*) kg dm grass")]
		public void GivenIHaveAHerdWithCowsInItWhichHaveTakenInKgDmGrass(int herdSize, float grassIntake)
		{
			GivenIHaveAHerdWithHerd_SizeCowsInIt(herdSize);
			_totalGrassIntake = grassIntake;
			_grazingActivity = new GrazingActivity
			{
				Herd = _herd
			};
		}

		[Given(@"each kg dm grass contains (.*) VEM and (.*) g protein")]
		public void GivenEachKgDmGrassContainsVemAndProtein(float vem, float protein)
		{
			_grazingActivity.Plot = new Plot
			{
				Name = "Test plot",
				FeedAnalysis = new FeedAnalysis
				{
					Vem = vem,
					Re = protein
				}
			};
		}

		[Given(@"my herd has produced (.*) liters of milk")]
		public void GivenMyHerdHasProducedLitersOfMilk(float milkProduction)
		{
			_milkProductionAnalysis.Amount = milkProduction;
		}

		[When(@"I let Grip op Gras 2 create a ration")]
		public void WhenILetGripOpGrasCreateARation()
		{
			_exceptionDriver.TryExecute(() =>
				_result = _rationAlgorithm.CreateRationAsync(_feedProducts, _herd, _totalGrassIntake,
					_milkProductionAnalysis, _grazingActivity).Result
			);
		}

		[Then(@"the ration should contain between (.*) and (.*) kg dm of roughage products")]
		public void ThenTheRationShouldContainBetweenMinAndMaxKgDmOfRoughageProducts(float minAmount, float maxAmount)
		{
			_result.Should().NotBeNull();
			_result!.FeedProducts.Should().NotBeNull();

			float totalAmountOfKgRoughageDryMatter = _result.FeedProducts!
				.Where(feedProduct => feedProduct.Key is Roughage).Sum(feedProduct => feedProduct.Value);
			totalAmountOfKgRoughageDryMatter.Should().BeInRange(minAmount, maxAmount);
		}

		[Then(@"the ration should contain between (.*) and (.*) kg dm of supplementary products")]
		public void ThenTheRationShouldContainBetweenMinAndMaxKgDmOfSupplementaryProducts(float minAmount,
			float maxAmount)
		{
			_result.Should().NotBeNull();
			_result!.FeedProducts.Should().NotBeNull();

			float totalAmountOfKgSupplementaryDryMatter = _result.FeedProducts!
				.Where(feedProduct => feedProduct.Key is SupplementaryFeedProduct)
				.Sum(feedProduct => feedProduct.Value);
			totalAmountOfKgSupplementaryDryMatter.Should().BeInRange(minAmount, maxAmount);
		}

		[Then(@"the ration should contain between (.*) and (.*) g protein")]
		public void ThenTheRationShouldContainBetweenProtein_Ration_MinAndProtein_Ration_MaxGProtein(float min,
			float max)
		{
			_result.Should().NotBeNull();
			_result!.FeedProducts.Should().NotBeNull();

			float totalAmountOfProtein =
				(float)_result.FeedProducts!.Sum(feedProduct => feedProduct.Key.FeedAnalysis!.Re * feedProduct.Value)!;
			totalAmountOfProtein.Should().BeInRange(min, max);
		}

		[Then(@"the ration should contain between (.*) and (.*) VEM")]
		public void ThenTheRationShouldContainBetweenVem_Ration_MinAndVem_Ration_MaxVEM(float min, float max)
		{
			_result.Should().NotBeNull();
			_result!.FeedProducts.Should().NotBeNull();

			float totalAmountOfVem =
				(float)_result.FeedProducts!.Sum(feedProduct => feedProduct.Key.FeedAnalysis!.Vem * feedProduct.Value)!;
			totalAmountOfVem.Should().BeInRange(min, max);
		}

		[Then(@"the ration must contain (.*) kg of grass")]
		public void ThenTheRationMustContainKgOfGrass(int amount)
		{
			_result.Should().NotBeNull();
			_result!.GrassIntake.Should().Be(amount);
		}
	}
}