using GripOpGras2.Domain;

namespace GripOpGras2.Client.Features.CreateRation
{
	public interface IRationAlgorithm
	{
		/// <summary>
		/// Calculates the ration for the herd.
		/// </summary>
		/// <param name="totalGrassIntake">The total grass intake of the herd in kg dm.</param>
		public Task<FeedRation> CreateRationAsync(IReadOnlyList<Roughage> roughages, Herd herd, float totalGrassIntake,
			GrazingActivity grazingActivity, MilkProductionAnalysis milkProductionAnalysis);
	}
}