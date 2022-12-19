using GripOpGras2.Client.Features.CreateRation;
using GripOpGras2.Domain;

namespace GripOpGras2.Specs.StepDefinitions
{
	[Binding]
	public class GripOpGras2_CalculateGrassIntakeV1StepDefinitions
	{
		private readonly CalculateGrassIntakeV1 _calculateGrassIntakeV1 = new();

		private readonly Plot _plot = new();

		private float _result;

		[Given(@"my plot has a size of (.*) ha")]
		public void GivenMyPlotHasASizeOfHa(float size)
		{
			_plot.Area = size;
		}

		[Given(@"my plot has (.*) net kg dm/ha")]
		public void GivenMyPlotHasNetDryMatterKgDmHa(float dryMatter)
		{
			_plot.NetDryMatter = dryMatter;
		}

		[When(@"I calculate the total grass intake")]
		public void WhenICalculateTheTotalGrassIntake()
		{
			GrazingActivity grazingActivity = new()
			{
				From = default,
				To = default,
				Herd = null,
				Plot = _plot
			};
			_result = _calculateGrassIntakeV1.CalculateGrassIntakeAsync(grazingActivity).Result;
		}

		[Then(@"the total grass intake of the herd should be (.*) kg dm")]
		public void ThenTheTotalGrassIntakeOfTheHerdShouldBeKgDm(float result)
		{
			_result.Should().Be(result);
		}
	}
}