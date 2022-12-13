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
			Dictionary<Roughage, float> rationRoughages = new();

			if (herd.NumberOfAnimals == 0)
			{
				return await Task.FromResult(new FeedRation
				{
					Herd = herd,
					Date = DateTime.Now,
					Plot = null,
					Roughages = rationRoughages,
					GrassIntake = 0
				});
			}

			// TODO test wat er wordt gedaan met ruwvoer producten die wel of niet een VEM en/of RE waarde bevatten
			// TODO test de conditie wanneer een ruwvoerproduct niet beschikbaar is maar wel aan de functie wordt gegeven.
			List<Roughage> availableRoughages =
				roughages.Where(r => r is { Available: true, FeedAnalysis: { VEM: { }, RE: { } } }).ToList();

			if (availableRoughages.Count == 0)
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

			float totalDryMatterIntakeInKg = totalGrassIntake;
			float vemIntake = (float)(grazingActivity.Plot.FeedAnalysis.VEM * totalGrassIntake);
			float proteinIntakeInKg = (float)(grazingActivity.Plot.FeedAnalysis.RE * totalGrassIntake / 1000);
			float vemNeeds = CalculateVemNeedsOfTheHerd(herd, milkProductionAnalysis);

			if (vemIntake < vemNeeds)
			{
				Roughage roughageWithHighestVEM =
					availableRoughages.OrderByDescending(r => r.FeedAnalysis.VEM).First();

				// TODO mogelijk een hogere limiet hebben dan 0
				if (roughageWithHighestVEM.FeedAnalysis.VEM > 0)
				{
					float amountOfVemToAdd = vemNeeds - vemIntake;
					float amountOfRoughageToAdd = (float)(amountOfVemToAdd / roughageWithHighestVEM.FeedAnalysis.VEM);

					rationRoughages.Add(roughageWithHighestVEM, amountOfRoughageToAdd);

					totalDryMatterIntakeInKg += amountOfRoughageToAdd;

					vemIntake += amountOfVemToAdd;
					proteinIntakeInKg += (float)(amountOfRoughageToAdd * roughageWithHighestVEM.FeedAnalysis.RE / 1000);
				}
			}

			if (proteinIntakeInKg > CalculateMaxAllowedProteinIntake(totalDryMatterIntakeInKg))
			{
				Roughage roughageWithLowestRE = availableRoughages.OrderBy(r => r.FeedAnalysis.RE).First();
				// TODO bespreek met de studenten wat de beste aanpak is
				if (roughageWithLowestRE.FeedAnalysis.RE <= MaxRECoverage * 1000 * 0.90)
				{
					// The roughageWithLowestRE can be used in a realistic situation to reduce the RE percentage

					float amountOfProductToAdd = 0;
					float feedAnalysisRe = (float)(roughageWithLowestRE.FeedAnalysis.RE / 1000);

					// Add roughage product until the ration has reached the optimal RE coverage
					while (proteinIntakeInKg / OptimalRECoverage > totalDryMatterIntakeInKg)
					{
						amountOfProductToAdd++;
						totalDryMatterIntakeInKg++;
						proteinIntakeInKg += feedAnalysisRe;
					}

					rationRoughages.Add(roughageWithLowestRE, amountOfProductToAdd);
				}
			}
			else if (proteinIntakeInKg < CalculateProteinNeedsOfTheHerd(totalDryMatterIntakeInKg))
			{
				Roughage roughageWithHighestRe =
					availableRoughages.OrderByDescending(r => r.FeedAnalysis.RE).First();

				// The protein contant needs to be at least 10% more then the optimal coverage. 
				// TODO bespreek met de studenten wat de beste aanpak is
				if (roughageWithHighestRe.FeedAnalysis.RE >= OptimalRECoverage * 1000 * 1.10)
				{
					// The roughageWithHighestRE can be used in a realistic situation

					float amountOfProductToAdd = 0;
					float feedAnalysisRe = (float)(roughageWithHighestRe.FeedAnalysis.RE / 1000);

					// Add roughage product until the ration has reached the optimal RE coverage
					while (proteinIntakeInKg / OptimalRECoverage < totalDryMatterIntakeInKg)
					{
						amountOfProductToAdd++;
						totalDryMatterIntakeInKg++;
						proteinIntakeInKg += feedAnalysisRe;
					}

					rationRoughages.Add(roughageWithHighestRe, amountOfProductToAdd);
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

		/// <param name="totalDryMatterIntake">In kg.</param>
		/// <returns>The maximum amount of protein that the herd is allowed to take in, in kg.</returns>
		public float CalculateMaxAllowedProteinIntake(float totalDryMatterIntake)
		{
			return totalDryMatterIntake * MaxRECoverage;
		}
	}
}