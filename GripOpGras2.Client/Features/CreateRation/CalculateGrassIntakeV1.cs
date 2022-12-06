using GripOpGras2.Domain;

namespace GripOpGras2.Client.Features.CreateRation
{
	//TODO the implementation of this class needs to be moved to a separate module.
	public class CalculateGrassIntakeV1 : ICalculateGrassIntake
	{
		public async Task<float> CalculateGrassIntakeAsync(Grazing grazingActivity)
		{
			float totalDryMatter = grazingActivity.Plot.NetDryMatter * grazingActivity.Plot.Area;
			totalDryMatter = (float)(totalDryMatter - totalDryMatter * 0.15);

			return await Task.FromResult(totalDryMatter);
		}
	}
}