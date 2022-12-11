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

		private const float MaxRECoverage = 0.17f;

		public async Task<FeedRation> CreateRationAsync(IReadOnlyList<Roughage> roughages, Herd herd,
			float totalGrassIntake, GrazingActivity grazingActivity, MilkProductionAnalysis milkProductionAnalysis)
		{
			// TODO test de konditie wanneer een ruwvoerproduct niet beschikbaar is maar wel aan de functie wordt gegeven.
			List<Roughage> availableRoughages = roughages.Where(r => r.Available).ToList();
			float totalDryMatterIntakeInKg = totalGrassIntake;
			float vemIntake = (float)(grazingActivity.Plot.FeedAnalysis.VEM * totalGrassIntake);
			float proteinIntakeInKg = (float)(grazingActivity.Plot.FeedAnalysis.RE * totalGrassIntake / 1000);
			float vemNeeds = CalculateVemNeedsOfTheHerd(herd, milkProductionAnalysis);

			Dictionary<Roughage, float> rationRoughages = new();

			if (vemIntake < vemNeeds)
			{
				Roughage roughageWithHighestVEM = availableRoughages.OrderByDescending(r => r.FeedAnalysis.VEM).First();

				float amountOfVemToAdd = vemNeeds - vemIntake;
				float amountOfRoughageToAdd = (float)(amountOfVemToAdd / roughageWithHighestVEM.FeedAnalysis.VEM);

				rationRoughages.Add(roughageWithHighestVEM, amountOfRoughageToAdd);

				totalDryMatterIntakeInKg += amountOfRoughageToAdd;

				vemIntake += amountOfVemToAdd;
				proteinIntakeInKg += (float)(amountOfRoughageToAdd * roughageWithHighestVEM.FeedAnalysis.RE / 1000);
			}

			if (proteinIntakeInKg > totalDryMatterIntakeInKg * MaxRECoverage)
			{
				Roughage roaghageWithLowestRE = availableRoughages.OrderBy(r => r.FeedAnalysis.RE).First();
				// TODO check of het RE gehalte van het ruwvoer product laag genoeg is, zodat dit gebruikt kan worden om te verdunnen.
				// TODO Het rantsoen bevat te veel eiwit en moet dus worden verdund
				throw new NotImplementedException();
			}
			else if (proteinIntakeInKg < CalculateProteinNeedsOfTheHerd(totalDryMatterIntakeInKg))
			{
				Roughage roughageWithHighestRE = availableRoughages.OrderByDescending(r => r.FeedAnalysis.RE).First();

				// The protein contant needs to be at least 10% more then the optimal coverage. 
				// TODO bespreek met de studenten wat de beste aanpak is
				if (roughageWithHighestRE.FeedAnalysis.RE > OptimalRECoverage * 100 * 1.10)
				{
					// The roughageWithHighestRE can be used in a realistic situation

					float amountOfProductToAdd = 0;

					while (proteinIntakeInKg / OptimalRECoverage < totalDryMatterIntakeInKg)
					{
						//TODO
					}

					//TODO add RE rich products

					throw new NotImplementedException();
				}
			}

			return await Task.FromResult(new FeedRation
			{
				Herd = herd,
				Date = DateTime.Now,
				Plot = grazingActivity.Plot,
				Roughages = rationRoughages,
				GrassIntake = totalGrassIntake
			});
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