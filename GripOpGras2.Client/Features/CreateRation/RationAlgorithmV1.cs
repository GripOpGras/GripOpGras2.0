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

		private const float OptimalRECoverage = 0.15f;

		private const float MaxRE = 1.70f;

		public async Task<FeedRation> CreateRationAsync(IReadOnlyList<Roughage> roughages, Herd herd,
			float totalGrassIntake, GrazingActivity grazingActivity, MilkProductionAnalysis milkProductionAnalysis)
		{
			float totalDryMatterIntakeInKg = totalGrassIntake;
			float vemIntake = (float)(grazingActivity.Plot.FeedAnalysis.VEM * totalGrassIntake);
			float proteinIntakeInKg = (float)(grazingActivity.Plot.FeedAnalysis.RE * totalGrassIntake / 1000);
			float vemNeeds = CalculateVemNeedsOfTheHerd(herd, milkProductionAnalysis);
			float proteinNeedsInKg = CalculateProteinNeedsOfTheHerd(totalDryMatterIntakeInKg);

			Dictionary<Roughage, float> rationRoughages = new();

			if (vemIntake > vemNeeds)
			{
				if (proteinIntakeInKg > proteinNeedsInKg)
				{
					return await Task.FromResult(new FeedRation
					{
						Herd = herd,
						Date = DateTime.Now,
						Plot = grazingActivity.Plot,
						Roughages = rationRoughages,
						GrassIntake = totalGrassIntake
					});
				}

				//TODO add RE rich products
				throw new NotImplementedException();
			}


			//TODO add VEM rich products
			throw new NotImplementedException();




			if (proteinIntakeInKg > proteinNeedsInKg)
			{
				return new FeedRation
				{
					Roughages = rationRoughages,
					Date = DateTime.Now,
					GrassIntake = totalGrassIntake,
					Herd = herd,
					Plot = grazingActivity.Plot
				};
			}

			//TODO add RE rich products
			throw new NotImplementedException();

		}

		//TODO hier tests voor opstellen
		public float CalculateVemNeedsOfTheHerd(Herd herd, MilkProductionAnalysis milkProductionAnalysis)
		{
			float milkProductionForEachCow = milkProductionAnalysis.Amount / herd.NumberOfAnimals;
			float vemNeedsCow = (milkProductionForEachCow * VemNeedsPerLiterMilk + DefaultVEMNeeds) *
								OptimalVEMCoverage;

			return herd.NumberOfAnimals * vemNeedsCow;
		}

		/// <param name="totalDryMatterIntake">In kg.</param>
		/// <returns>Total protein needs in kg.</returns>
		public float CalculateProteinNeedsOfTheHerd(float totalDryMatterIntake)
		{
			return totalDryMatterIntake * OptimalRECoverage;
		}
	}
}