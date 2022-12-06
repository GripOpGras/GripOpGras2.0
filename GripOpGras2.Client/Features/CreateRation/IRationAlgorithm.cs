using GripOpGras2.Domain;

namespace GripOpGras2.Client.Features.CreateRation
{
	public interface IRationAlgorithm
	{
		public Task<FeedRation> CreateRationAsync(IReadOnlyList<Roughage> roughages, Herd herd, float totalGrassIntake,
			Grazing grazingActivity, MilkProductionAnalysis milkProductionAnalysis);
	}
}