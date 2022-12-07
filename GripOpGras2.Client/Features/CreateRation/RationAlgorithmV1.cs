using GripOpGras2.Domain;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class RationAlgorithmV1 : IRationAlgorithm
	{
		public readonly string GrassName = "Versgras";

		/// <summary>
		/// The standerd VEM needs of a cow
		/// </summary>
		private const float DefaultVEMNeeds = 5500;

		private const float VemNeedsPerLiterMilk = 450;

		private const float OptimalVEMCoverage = 1.05f;

		private const float OptimalRECoverage = 0.15f;

		private const float MaxRE = 1.70f;

		public async Task<FeedRation> CreateRationAsync(IReadOnlyList<Roughage> roughages, Herd herd,
			float totalGrassIntake, GrazingActivity grazingActivity, MilkProductionAnalysis milkProductionAnalysis)
		{
			float totalDryMatterIntake = totalGrassIntake;
			float vemIntake = (float)(grazingActivity.Plot.FeedAnalysis.VEM * totalGrassIntake);
			float reIntake = (float)(grazingActivity.Plot.FeedAnalysis.RE * totalGrassIntake);

			float vemNeeds = CalculateVemNeedsOfTheHerd(herd, milkProductionAnalysis);



			// TODO bereken hoeveel 

			throw new NotImplementedException();
		}

		public float CalculateVemNeedsOfTheHerd(Herd herd, MilkProductionAnalysis milkProductionAnalysis)
		{
			float milkProductionForEachCow = milkProductionAnalysis.Amount / herd.NumberOfAnimals;

			return herd.NumberOfAnimals *
				   ((milkProductionForEachCow * VemNeedsPerLiterMilk + DefaultVEMNeeds) * OptimalVEMCoverage);
		}

		public float CalculateProteinNeedsOfTheHerd(float totalDryMatterIntake)
		{
			return totalDryMatterIntake * OptimalRECoverage;
		}
	}
}