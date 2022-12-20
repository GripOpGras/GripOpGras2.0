using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;

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

		private const float OptimalRECoverage = 150;

		private const float MaxRECoverage = 170;

		private const float MinRECoverage = 140;

		public async Task<FeedRation> CreateRationAsync(IReadOnlyList<FeedProduct> feedProducts, Herd herd,
			float totalGrassIntake, MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			throw new NotImplementedException();

			/*return await Task.FromResult(new FeedRation
			{
				Herd = herd,
				Date = DateTime.Now,
				Plot = grazingActivity?.Plot,
				FeedProducts = rationProducts,
				GrassIntake = totalGrassIntake
			});*/
		}

		public float CalculateVemNeedsOfTheHerd(Herd herd, MilkProductionAnalysis milkProductionAnalysis)
		{
			float milkProductionForEachCow = milkProductionAnalysis.Amount / herd.NumberOfAnimals;
			float vemNeedsCow = (milkProductionForEachCow * VemNeedsPerLiterMilk + DefaultVEMNeeds) *
								OptimalVEMCoverage;

			return herd.NumberOfAnimals * vemNeedsCow;
		}
	}
}