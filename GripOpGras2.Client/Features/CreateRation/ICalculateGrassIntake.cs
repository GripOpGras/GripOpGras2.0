using GripOpGras2.Domain;

namespace GripOpGras2.Client.Features.CreateRation
{
	public interface ICalculateGrassIntake
	{
		/// <summary>
		/// Calculates the total grass intake during the grazing activity.
		/// </summary>
		/// <returns>The total dry matter intake of the herd in kg</returns>
		public Task<float> CalculateGrassIntakeAsync(GrazingActivity grazingActivity);
	}
}