using GripOpGras2.Client.Features.CreateRation;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;
using GripOpGras2.Specs.Drivers;

namespace GripOpGras2.Specs.StepDefinitions
{
	[Binding]
	public class GripOpGras2_RationAlgorithmV1StepDefinitions
	{
		private readonly RationAlgorithmV1 _rationAlgorithmV1 = new();

		private readonly Herd _herd = new();

		private readonly MilkProductionAnalysis _milkProductionAnalysis = new();

		private GrazingActivity? _grazingActivity;

		private readonly List<FeedProduct> _feedProducts = new();

		private float _totalGrassIntake;

		private FeedRation? _result;

		private readonly ExceptionDriver _exceptionDriver;

		public GripOpGras2_RationAlgorithmV1StepDefinitions(ExceptionDriver exceptionDriver)
		{
			_exceptionDriver = exceptionDriver;
		}

		[Given(@"I have the roughage product (.*) that contains (.*) kg dm, (.*) g protein, and (.*) VEM")]
		public void GivenIHaveTheRoughageProductThatContainsDmProteinAndVEM(string roughageName, float dm,
			float protein, float vem)
		{
			_feedProducts.Add(new Roughage
			{
				Name = roughageName,
				FeedAnalysis = new FeedAnalysis
				{
					DryMatter = dm,
					RE = protein,
					VEM = vem
				},
				Available = true
			});
		}

		[Given(@"I have the supplementary product (.*) that contains (.*) kg dm, (.*) g protein, and (.*) VEM")]
		public void GivenIHaveTheSupplementaryProductThatContainsDmProteinAndVEM(string supplementaryName, float dm,
			float protein, float vem)
		{
			_feedProducts.Add(new SupplementaryFeedProduct()
			{
				Name = supplementaryName,
				FeedAnalysis = new FeedAnalysis
				{
					DryMatter = dm,
					RE = protein,
					VEM = vem
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
		public void GivenEachKgDmGrassContainsVEMAndProtein(float vem, float protein)
		{
			_grazingActivity.Plot = new Plot
			{
				Name = "Test plot",
				FeedAnalysis = new FeedAnalysis
				{
					VEM = vem,
					RE = protein
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
				_result = _rationAlgorithmV1.CreateRationAsync(_feedProducts, _herd, _totalGrassIntake,
					_milkProductionAnalysis, _grazingActivity).Result
			);
		}

		[Then(@"the ration should contain (.*) kg dm of (.*)")]
		public void ThenTheRationShouldContainKgDmOfProduct(float amount, string productName)
		{
			_result.Should().NotBeNull();

			FeedProduct? roughage = _feedProducts.FirstOrDefault(r => r.Name == productName);

			if (roughage == null)
			{
				throw new Exception($"FeedProduct {productName} could not be found.");
			}

			//TODO mogelijk dit opdelen in twee checks!
			_result!.FeedProducts.Should().Contain(roughage, amount);
		}

		[Then(@"the ration must contain (.*) kg of grass")]
		public void ThenTheRationMustContainKgOfGrass(int amount)
		{
			_result.Should().NotBeNull();
			_result!.GrassIntake.Should().Be(amount);
		}
	}
}