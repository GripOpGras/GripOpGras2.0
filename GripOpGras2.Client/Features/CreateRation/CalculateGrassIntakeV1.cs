using GripOpGras2.Domain;

namespace GripOpGras2.Client.Features.CreateRation
{
	//TODO the implementation of this class needs to be moved to a separate module.
	public class CalculateGrassIntakeV1 : ICalculateGrassIntake
	{
		private const float GrazingLosses = 0.85f;

		public async Task<float> CalculateGrassIntakeAsync(GrazingActivity grazingActivity)
		{
			float totalDryMatter = grazingActivity.Plot.NetDryMatter * grazingActivity.Plot.Area;
			totalDryMatter *= GrazingLosses;

			return await Task.FromResult(totalDryMatter);
		}
	}
}