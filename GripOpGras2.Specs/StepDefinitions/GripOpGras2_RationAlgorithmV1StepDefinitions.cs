using FluentAssertions;
using GripOpGras2.Client.Features.CreateRation;
using GripOpGras2.Domain;

namespace GripOpGras2.Specs.StepDefinitions
{
	[Binding]
	public class GripOpGras2_RationAlgorithmV1StepDefinitions
	{
		private readonly RationAlgorithmV1 _rationAlgorithmV1 = new();
		private readonly Herd _herd = new();
		private readonly MilkProductionAnalysis _milkProductionAnalysis = new();
		private readonly GrazingActivity _grazingActivity = new();
		private readonly List<Roughage> _roughages = new();
		private float _totalGrassIntake;
		private FeedRation? _result;

		[Given(@"I have (.*) that contains (.*) dm, (.*) protein, and (.*) VEM")]
		public void GivenIHavProductThatContainsDmProteinAndVEM(string product, float dm, float protein, float vem)
		{
			_roughages.Add(new Roughage
			{
				Name = product,
				FeedAnalysis = new FeedAnalysis
				{
					DryMatter = dm,
					RE = protein,
					VEM = vem
				},
				Available = true
			});
		}

		[Given(@"I have a herd with (.*) cows in it, which have taken in (.*) kg dm grass")]
		public void GivenIHaveAHerdWithCowsInItWhichHaveTakenInKgDmGrass(int herdSize, float grassIntake)
		{
			_herd.NumberOfAnimals = herdSize;
			_totalGrassIntake = grassIntake;
			_grazingActivity.Herd = _herd;
		}

		[Given(@"each kg dm grass contains (.*) VEM and (.*) protein")]
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
			_result = _rationAlgorithmV1.CreateRationAsync(_roughages, _herd, _totalGrassIntake, _grazingActivity, _milkProductionAnalysis).Result;
		}

		[Then(@"the ration should contain (.*) kg dm of (.*)")]
		public void ThenTheRationShouldContainKgDmOfProduct(float amount, string product)
		{
			_result.Should().NotBeNull();
			_result!.Roughages.Should().ContainSingle(r => r.Key.Name == product);
			_result.Roughages.Should().Contain(r => r.Value == amount);
		}

		[Then(@"the ration should contain (.*) kg dm of grass")]
		public void ThenTheRationShouldContainKgDmOfGrass(float amount)
		{
			_result.Should().NotBeNull();
			_result!.Roughages.Should().ContainSingle(r => r.Key.Name == _rationAlgorithmV1.GrassName);
			_result.Roughages.Should().Contain(r => r.Value == amount);
		}
	}
}
