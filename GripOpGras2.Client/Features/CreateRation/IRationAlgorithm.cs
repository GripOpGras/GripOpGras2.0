using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public interface IRationAlgorithm
	{
		/// <summary>
		/// Calculates the ration for the herd.
		/// </summary>
		/// <param name="totalGrassIntake">The total grass intake of the herd in kg dm.</param>
		public Task<FeedRation> CreateRationAsync(IReadOnlyList<FeedProduct> feedProducts, Herd herd, float totalGrassIntake,
			MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity);
	}
}