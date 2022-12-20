using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class RationAlgorithmV1 : IRationAlgorithm
	{
		private const float DefaultVEMNeedsOfCow = 5500;

		private const float VemNeedsPerLiterMilk = 450;

		private const float MaxAmountOfSupplementaryFeedProductInKGPerCow = 4.5f;

		private const float MaxKgDmIntakePerCow = 18;

		private const float OptimalVEMCoverage = 1.05f;

		private const float OptimalRECoverageInGramsPerKgDm = 150;

		// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		private const float MaxRECoverageInGramsPerKgDm = 170;

		// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		private const float MinRECoverageInGramsPerKgDm = 140;

		public async Task<FeedRation> CreateRationAsync(IReadOnlyList<FeedProduct> feedProducts, Herd herd,
			float totalGrassIntake, MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			Dictionary<FeedProduct, float> rationProducts = new();

			// TODO test wat er wordt gedaan met ruwvoer producten die wel of niet een VEM en/of RE waarde bevatten
			// TODO test de conditie wanneer een ruwvoerproduct niet beschikbaar is maar wel aan de functie wordt gegeven.
			List<FeedProduct> availableFeedProducts =
				feedProducts.Where(r => r is { Available: true, FeedAnalysis: { VEM: { }, RE: { } } }).ToList();

			IReadOnlyList<Roughage> roughages = availableFeedProducts.OfType<Roughage>().ToList();
			IReadOnlyList<SupplementaryFeedProduct> supplementaryFeedProducts =
				availableFeedProducts.OfType<SupplementaryFeedProduct>().ToList();

			if (herd.NumberOfAnimals == 0)
			{
				return await Task.FromResult(new FeedRation
				{
					Herd = herd,
					Date = DateTime.Now,
					Plot = null,
					FeedProducts = rationProducts,
					GrassIntake = 0
				});
			}

			if (availableFeedProducts.Count == 0)
			{
				return await Task.FromResult(new FeedRation
				{
					Herd = herd,
					Date = DateTime.Now,
					Plot = grazingActivity?.Plot,
					FeedProducts = rationProducts,
					GrassIntake = totalGrassIntake
				});
			}

			float totalDryMatterIntakeInKg = totalGrassIntake;

			float dryMatterIntakeCow = totalDryMatterIntakeInKg / herd.NumberOfAnimals;
			// Positive value is too much RE and a negative value is too little RE.
			float deviationFromDesiredReCoverageForEachVemInTheGrass =
				(float)((OptimalRECoverageInGramsPerKgDm - grazingActivity.Plot.FeedAnalysis.RE) /
					grazingActivity.Plot.FeedAnalysis.VEM * -1);
			float amountOfKgGrassForEachVem = (float)(1 / grazingActivity.Plot.FeedAnalysis.VEM);
			float amountOfProteinInTheGrassToBeCompensated = (float)(grazingActivity.Plot.FeedAnalysis.VEM *
			                                                         deviationFromDesiredReCoverageForEachVemInTheGrass *
			                                                         totalGrassIntake);


			Dictionary<FeedProduct, float> deviationFromDesiredReCoverageForEachProduct = new();

			foreach (SupplementaryFeedProduct supplementaryFeedProduct in supplementaryFeedProducts)
			{
				float deviationFromDesiredRe =
					(float)((OptimalRECoverageInGramsPerKgDm - supplementaryFeedProduct.FeedAnalysis.RE) /
						supplementaryFeedProduct.FeedAnalysis.VEM * -1);
				deviationFromDesiredReCoverageForEachProduct.Add(supplementaryFeedProduct, deviationFromDesiredRe);
			}


			//pick the Roughage with the biggest negative RE of grass deviation

			Roughage roughageWithBiggestReDeviation;

			if (deviationFromDesiredReCoverageForEachVemInTheGrass > 0)
			{
				roughageWithBiggestReDeviation =
					roughages.OrderBy(r => deviationFromDesiredReCoverageForEachProduct[r]).First();
			}
			else if (deviationFromDesiredReCoverageForEachVemInTheGrass < 0)
			{
				roughageWithBiggestReDeviation =
					roughages.OrderBy(r => deviationFromDesiredReCoverageForEachProduct[r]).Last();
			}
			else
			{
				throw new NotImplementedException("add support for grass with RE of 0");
			}

			float vemIntake = 0;
			float proteinIntakeInG = 0;

			if (grazingActivity != null)
			{
				vemIntake = (float)(grazingActivity.Plot.FeedAnalysis.VEM * totalGrassIntake);
				proteinIntakeInG = (float)(grazingActivity.Plot.FeedAnalysis.RE * totalGrassIntake);
			}

			//pick the amount VEM of roughageWithBiggestNegativeReDeviation to compensate the REdeviation of grass
			float aanvullenVEMMetHoogstTegenpooLvanEiwittekortCverschot = 1;
			throw new NotImplementedException("Not Implemented");
			return null;
		}

		//////////--------------------------//////////
			//////////--------------------------//////////
			//////////--------------------------//////////

		//	float vemNeeds = CalculateVemNeedsOfTheHerd(herd, milkProductionAnalysis);

		//	if (vemIntake < vemNeeds)
		//	{
		//		Roughage roughageWithHighestVEM =
		//			availableRoughages.OrderByDescending(r => r.FeedAnalysis.VEM).First();

		//		// TODO mogelijk een hogere limiet hebben dan 0
		//		if (roughageWithHighestVEM.FeedAnalysis.VEM > 0)
		//		{
		//			float amountOfVemToAdd = vemNeeds - vemIntake;
		//			float amountOfRoughageToAdd = (float)(amountOfVemToAdd / roughageWithHighestVEM.FeedAnalysis.VEM);

		//			rationProducts.Add(roughageWithHighestVEM, amountOfRoughageToAdd);

		//			totalDryMatterIntakeInKg += amountOfRoughageToAdd;

		//			vemIntake += amountOfVemToAdd;
		//			proteinIntakeInKg += (float)(amountOfRoughageToAdd * roughageWithHighestVEM.FeedAnalysis.RE / 1000);
		//		}
		//	}

		//	if (proteinIntakeInKg > CalculateMaxAllowedProteinIntake(totalDryMatterIntakeInKg))
		//	{
		//		Roughage roughageWithLowestRE = availableRoughages.OrderBy(r => r.FeedAnalysis.RE).First();
		//		// TODO bespreek met de studenten wat de beste aanpak is
		//		if (roughageWithLowestRE.FeedAnalysis.RE <= MaxRECoverage * 1000 * 0.90)
		//		{
		//			// The roughageWithLowestRE can be used in a realistic situation to reduce the RE percentage

		//			float amountOfProductToAdd = 0;
		//			float feedAnalysisRe = (float)(roughageWithLowestRE.FeedAnalysis.RE / 1000);

		//			// Add roughage product until the ration has reached the optimal RE coverage
		//			while (proteinIntakeInKg / OptimalRECoverage > totalDryMatterIntakeInKg)
		//			{
		//				amountOfProductToAdd++;
		//				totalDryMatterIntakeInKg++;
		//				proteinIntakeInKg += feedAnalysisRe;
		//			}

		//			rationProducts.Add(roughageWithLowestRE, amountOfProductToAdd);
		//		}
		//	}
		//	else if (proteinIntakeInKg < CalculateProteinNeedsOfTheHerd(totalDryMatterIntakeInKg))
		//	{
		//		Roughage roughageWithHighestRe =
		//			availableRoughages.OrderByDescending(r => r.FeedAnalysis.RE).First();

		//		// The protein contant needs to be at least 10% more then the optimal coverage. 
		//		// TODO bespreek met de studenten wat de beste aanpak is
		//		if (roughageWithHighestRe.FeedAnalysis.RE >= OptimalRECoverage * 1000 * 1.10)
		//		{
		//			// The roughageWithHighestRE can be used in a realistic situation

		//			float amountOfProductToAdd = 0;
		//			float feedAnalysisRe = (float)(roughageWithHighestRe.FeedAnalysis.RE / 1000);

		//			// Add roughage product until the ration has reached the optimal RE coverage
		//			while (proteinIntakeInKg / OptimalRECoverage < totalDryMatterIntakeInKg)
		//			{
		//				amountOfProductToAdd++;
		//				totalDryMatterIntakeInKg++;
		//				proteinIntakeInKg += feedAnalysisRe;
		//			}

		//			rationProducts.Add(roughageWithHighestRe, amountOfProductToAdd);
		//		}
		//	}

		//	return await Task.FromResult(new FeedRation
		//	{
		//		Herd = herd,
		//		Date = DateTime.Now,
		//		Plot = grazingActivity?.Plot,
		//		FeedProducts = rationProducts,
		//		GrassIntake = totalGrassIntake
		//	});
		//}

		////TODO hier tests voor opstellen
		//public float CalculateVemNeedsOfTheHerd(Herd herd, MilkProductionAnalysis milkProductionAnalysis)
		//{
		//	float milkProductionForEachCow = milkProductionAnalysis.Amount / herd.NumberOfAnimals;
		//	float vemNeedsCow = (milkProductionForEachCow * VemNeedsPerLiterMilk + DefaultVEMNeedsOfCow) *
		//						OptimalVEMCoverage;

		//	return herd.NumberOfAnimals * vemNeedsCow;
		//}

		///// <param name="totalDryMatterIntake">In kg.</param>
		///// <returns>Total protein needs in kg.</returns>
		//public float CalculateProteinNeedsOfTheHerd(float totalDryMatterIntake)
		//{
		//	return totalDryMatterIntake * OptimalRECoverage;
		//}

		///// <param name="totalDryMatterIntake">In kg.</param>
		///// <returns>The maximum amount of protein that the herd is allowed to take in, in kg.</returns>
		//public float CalculateMaxAllowedProteinIntake(float totalDryMatterIntake)
		//{
		//	return totalDryMatterIntake * MaxRECoverage;
		//}
	}
}