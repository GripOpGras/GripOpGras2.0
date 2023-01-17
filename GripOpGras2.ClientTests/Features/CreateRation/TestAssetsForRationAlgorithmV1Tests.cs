using GripOpGras2.Client.Features.CreateRation;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;
using NUnit.Framework;

namespace GripOpGras2.ClientTests.Features.CreateRation
{
	public static class TestAssetsForRationAlgorithmV1Tests
	{
		public static FeedProduct GetFeedProduct(string name, float re, float vem, bool isRoughage = true)
		{
			bool isAvailable = true;
			float dryMatter = 400;
			FeedProduct feedProduct = (isRoughage ? new Roughage() : new SupplementaryFeedProduct());
			feedProduct.Name = name;
			feedProduct.Available = isAvailable;
			feedProduct.FeedAnalysis = new FeedAnalysis
			{
				Date = DateTime.Now,
				DryMatter = dryMatter,
				Re = re,
				Vem = vem
			};
			return feedProduct;
		}

		public static MappedFeedProduct GetMappedFeedProduct(string name, float re, float vem, float appliedVem,
			bool isRoughage = true)
		{
			MappedFeedProduct mappedFeedProduct = new(GetFeedProduct(name, re, vem, isRoughage));
			mappedFeedProduct.SetAppliedVem(appliedVem);
			return mappedFeedProduct;
		}

		public static RationAlgorithmV1 CreateRationAlgorithm(
			List<FeedProduct> feedProducts, float totalGrassIntake = 0,
			float lmilk = 3000, bool hasPlot = true, float plotRe = 210, float plotVem = 1000)
		{
			// Arrange
			Herd herd = new()
			{
				Name = "Herd1",
				NumberOfAnimals = 100,
				Type = "Koe"
			};
			MilkProductionAnalysis milkProductionAnalysis = new()
			{
				Date = DateTime.Now,
				Amount = lmilk
			};
			GrazingActivity grazingActivity = new()
			{
				Herd = herd
			};
			if (hasPlot)
			{
				grazingActivity.Plot = new Plot
				{
					FeedAnalysis = new FeedAnalysis
					{
						Date = DateTime.Now,
						DryMatter = 80,
						Re = plotRe,
						Vem = plotVem
					}
				};
			}


			RationAlgorithmV1 rationAlgorithm = new();
			// Act
			Assert.DoesNotThrow(() => rationAlgorithm.SetUp(feedProducts: feedProducts, herd: herd,
				totalGrassIntake: totalGrassIntake, milkProductionAnalysis: milkProductionAnalysis,
				grazingActivity: grazingActivity));

			return rationAlgorithm;
		}
	}
}