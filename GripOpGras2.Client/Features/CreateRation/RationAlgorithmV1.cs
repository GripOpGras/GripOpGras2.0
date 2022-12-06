using GripOpGras2.Domain;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class RationAlgorithmV1 : IRationAlgorithm
	{
		/// <summary>
		/// The standerd VEM needs of a cow
		/// </summary>
		private const float DefaultVEMNeeds = 5500;

		private const float VemNeedsPerLiterMilk = 450;

		private const float OptimalVEMCoverage = 1.05f;

		public async Task<FeedRation> CreateRationAsync(IReadOnlyList<Roughage> roughages, Herd herd,
			float totalGrassIntake, Grazing grazingActivity, MilkProductionAnalysis milkProductionAnalysis)
		{
			float vemNeeds = CalculateVemNeedsOfTheHerd(herd, milkProductionAnalysis);


			throw new NotImplementedException();
		}

		public float CalculateVemNeedsOfTheHerd(Herd herd, MilkProductionAnalysis milkProductionAnalysis)
		{
			float milkProductionForEachCow = milkProductionAnalysis.Amount / herd.NumberOfAnimals;

			return herd.NumberOfAnimals *
				   ((milkProductionForEachCow * VemNeedsPerLiterMilk + DefaultVEMNeeds) * OptimalVEMCoverage);
		}
	}
}