using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class RationAlgorithmV1 : IRationAlgorithm
	{
		public async Task<FeedRation> CreateRationAsync(IReadOnlyList<FeedProduct> feedProducts, Herd herd,
			float totalGrassIntake, MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			throw new NotImplementedException();
		}
	}
}