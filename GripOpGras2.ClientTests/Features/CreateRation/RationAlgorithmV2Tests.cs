using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using GripOpGras2.Client.Features.CreateRation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GripOpGras2.Client.Features.CreateRation.Tests
{
	public class RationTests
	{

		[Test()]
		public void RationTestWithTwoProducts(float REprod1, float REprod2, float VEMprod1, float VEMprod2)
		{
			
		}
		[Test()]
		[TestCase(1000, 500, 500, 0, 0, 1000, 500, 500, 0)]
		[TestCase(1000, 500, 0, 500, 0, 1000, 500, 500, 0)]
		[TestCase(1000, 0, 500, 0, 500, 1000, 500, 500, 0)]
		[TestCase(1000, 500, 500, 500, 500, 1500, 1000, 1000, 1000)]
		public void TestTotalCalculations(float vem, float vem_bijprod, float dm, float dm_bijprod, float rediff, float expectedVEM, float expectedVEM_bijprod, float expectedDM, float expectedDM_bijprod)
		{
			Ration ration = new Ration();
			ration.RationList.Add(TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 1", rediff, vem, vem));
			ration.RationList.Add(TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 2", 0, vem_bijprod, vem_bijprod));
			ration.RationList.Add(TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 3", 0, 0, dm, false));
			ration.RationList.Add(TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 4", 0, 0, dm_bijprod, false));
    
			Assert.AreEqual(expectedVEM, ration.totalVEM);
			Assert.AreEqual(expectedVEM_bijprod, ration.totalVEM_Bijprod);
			Assert.AreEqual(expectedDM, ration.totalDM);
			Assert.AreEqual(expectedDM_bijprod, ration.totalDM_Bijprod);
			Assert.AreEqual(rediff, ration.totalREdiff);
		}

		[Test()]
		public void ApplyChangesToRationListTest()
		{
			Assert.Fail();
		}

		[Test()]
		public void CloneTest()
		{
			Assert.Fail();
		}
	}

	public static class TestassetsForRationAlgorithmV2Tests
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
				RE = re,
				VEM = vem
			};
			return feedProduct;
		}
		public static MappedFeedProduct GetMappedFeedProduct(string name, float re, float vem,float appliedVEM, bool isRoughage = true)
		{
			MappedFeedProduct mappedFeedProduct = new MappedFeedProduct(GetFeedProduct(name, re, vem, isRoughage));
			mappedFeedProduct.setAppliedVEM(appliedVEM);
			return mappedFeedProduct;
		}
	}

	[TestClass()]
	[TestFixture]
	public class RationAlgorithmV2Tests
	{
		[SetUp]
		public RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods Setup(List<FeedProduct> feedProducts, float totalGrassIntake = 1062.5f,
			float lmilk = 3000, bool hasPlot = true, float PlotRE = 210, float PlotVEM = 1000)
		{
			// Arrange
			var herd = new Herd()
			{
				Name = "Herd1",
				NumberOfAnimals = 100,
				Type = "Koe"
			};
			var milkProductionAnalysis = new MilkProductionAnalysis
			{
				Date = DateTime.Now,
				Amount = lmilk
			};
			GrazingActivity grazingActivity = new GrazingActivity();
			grazingActivity.Herd = herd;
			if (hasPlot)
			{
				grazingActivity.Plot = new Plot();
				grazingActivity.Plot.FeedAnalysis = new FeedAnalysis
				{
					Date = DateTime.Now,
					DryMatter = 80,
					RE = PlotRE,
					VEM = PlotVEM

				};
			}


			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm = new RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods();
			// Act
			Assert.DoesNotThrowAsync(rationAlgorithm.SetUp(feedProducts: feedProducts, herd: herd,
				totalGrassIntake: totalGrassIntake, milkProductionAnalysis: milkProductionAnalysis,
				grazingActivity: grazingActivity));

			return rationAlgorithm;
		}


		[Test()]
		public void CreateRationAsyncTest()
		{
			// Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod1", 189f, 884.2f);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod2", 88f, 1238f, false);
			var herd = new Herd()
			{
				Name = "Herd1",
				NumberOfAnimals = 100,
				Type = "Koe"
			};
			var milkProductionAnalysis = new MilkProductionAnalysis
			{
				Date = DateTime.Now,
				Amount = 3200
			};
			GrazingActivity grazingActivity = new GrazingActivity();
			grazingActivity.Herd = herd;
			grazingActivity.Plot = new Plot();
			var rationAlgorithm = new RationAlgorithmV2();
			// Act
			var feedRotation = rationAlgorithm.CreateRationAsync(
				feedProducts: new List<FeedProduct>()
				{
					prod1, prod2
				},
				herd: new Herd()
				{
					Name = "Herd1",
					NumberOfAnimals = 100,
					Type = "Koe"
				},
				totalGrassIntake: 1063.5f,
				milkProductionAnalysis: milkProductionAnalysis,
				grazingActivity: grazingActivity);
			// Assert
			Assert.NotNull(feedRotation);
			Assert.NotNull(feedRotation.FeedProducts);
			Assert.Contains(prod1, feedRotation.FeedProducts!.Keys);
			Assert.Contains(prod2, feedRotation.FeedProducts.Keys);
			Assert.AreEqual(1063.5f, feedRotation.GrassIntake);
			Assert.AreEqual(herd, feedRotation.Herd);
			//Assert.(feedRotation.Plot); plot nog een plekje gevene
		}



		[Test()]
		[TestCase(30, 30, 1800.5f, 1860.5)]
		[TestCase(0, 0, 1800.5f, 1800.5f)]
		[TestCase(0, 0, 0, 0)]
		[TestCase(30.5, 15, 0, 45.5)]
		public void GetTotalKGDMTest(float KGprod1, float KGprod2, float KGgrass, float expected)
		{
			//Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod1", 189f, 884.2f);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod2", 88f, 1238f);
			MappedFeedProduct mappedProd1 = new MappedFeedProduct(prod1);
			MappedFeedProduct mappedProd2 = new MappedFeedProduct(prod2);
			mappedProd1.setAppliedVEM(KGprod1 * 884.2f);
			mappedProd2.setAppliedVEM(KGprod2 * 1238f);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm = Setup(new List<FeedProduct>(), KGgrass);

			//Act
			var totalKGDM = rationAlgorithm.GetTotalKGDM();
			//Assert
			Assert.AreEqual(expected, totalKGDM);
		}

		[Test()]
		[TestCase(1, 1800, 30, 1200, 1, 800, 70, 1000, 1800 + 1200 + 800 + 70000)]
		[TestCase(800, 0, 30, 700, 50, 0, 70, 1000, 700 + 70000)]
		public void GetTotalVEMTest(float VEMPerDMprod1, float VEMprod1, float VEMPerDMprod2, float VEMprod2, float VEMPerDMBijvProd, float VEMBijvoerProd, float KGgrass, float VEMgrass, float expected)
		{
			//Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod1", 189f, VEMPerDMprod1);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod2", 88f, VEMPerDMprod2);
			FeedProduct bijvprod = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("bijvprod", 189f, VEMPerDMBijvProd, false);
			MappedFeedProduct mappedProd1 = new MappedFeedProduct(prod1);
			MappedFeedProduct mappedProd2 = new MappedFeedProduct(prod2);
			MappedFeedProduct mappedBijvprod = new MappedFeedProduct(bijvprod);
			mappedProd1.setAppliedVEM(VEMprod1);
			mappedProd2.setAppliedVEM(VEMprod2);
			mappedBijvprod.setAppliedVEM(VEMBijvoerProd);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm = Setup(new List<FeedProduct>(), KGgrass, PlotVEM: VEMgrass);
			rationAlgorithm.getsetroughages =
				new List<AbstractMappedFoodItem>{mappedProd1, mappedProd2, mappedBijvprod};
			//Act
			float totalVEM = rationAlgorithm.getsetTargetedVEMCoverage;
			//Assert
			Assert.AreEqual(expected, totalVEM);
		}

		[Test]
		[TestCase(100, 200, 300, 400, 30, 10, 500, 600, 200 + 400 + 500 * 600 + 10 * 0.3 * 100 + 10 * 0.7 * 300)]
		[TestCase(100, 200, 300, 400, 30, 0, 500, 600, 200 + 400 + 500 * 600)]
		[TestCase(300, 800, 500, 0, 0, 100, 0, 80, 800 + 500 * 100)]
		public void GetTotalVEMTestWithCombos(float VEMPerDMprod1, float VEMprod1, float VEMPerDMprod2, float VEMprod2,
			float percentProd1, float KGofGroup, float KGgrass, float VEMgrass, float expectd)
		{
			//Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod1", 189f, VEMPerDMprod1);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod2", 88f, VEMPerDMprod2);
			MappedFeedProduct mappedProd1 = new MappedFeedProduct(prod1);
			MappedFeedProduct mappedProd2 = new MappedFeedProduct(prod2);
			mappedProd1.setAppliedVEM(VEMprod1);
			mappedProd2.setAppliedVEM(VEMprod2);

			MappedFeedProductGroup mappedgroup = new MappedFeedProductGroup(
				new List<Tuple<AbstractMappedFoodItem, float>>{new Tuple<AbstractMappedFoodItem, float>(mappedProd1, percentProd1),
					new Tuple<AbstractMappedFoodItem, float>(mappedProd2, 100 - percentProd1)});
			mappedgroup.setAppliedVEM(KGofGroup);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm = Setup(new List<FeedProduct>(), KGgrass, PlotVEM: VEMgrass);
			rationAlgorithm.getsetroughages = new List<AbstractMappedFoodItem>{mappedProd1, mappedProd2, mappedgroup};
			//Act
			float totalVEM = rationAlgorithm.getsetTargetedVEMCoverage;
			//Assert
			Assert.AreEqual(expectd, totalVEM);
		}

		[Test()]
		public void GetGrassRENuturalizerFeedProductTest(float REperDMprod1, float VEMPerDMprod1, float REperDMprod2, float VEMPerDMprod2, float REperDMBijprod, float VEMPerDMbijprod, float KGgrass, float VEMgrass, float REgrass, string expectedProd, float expectedKGDM, Exception? expectedException)
		{
			//Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod1", REperDMprod1, VEMPerDMprod1);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod2", REperDMprod2, VEMPerDMprod2);
			FeedProduct bijprod = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("bijprod", REperDMBijprod, VEMPerDMbijprod, false);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm =
				Setup(new List<FeedProduct>{prod1, prod2, bijprod}, KGgrass, PlotVEM: VEMgrass, PlotRE: REgrass);
			//Act
			List<AbstractMappedFoodItem> foodlist = rationAlgorithm.GetGrassRENuturalizerFeedProduct();
			//Assert
			Assert.AreEqual(1, foodlist.Count);
			Assert.AreEqual(expectedProd, foodlist[0].GetProducts()[0].Item1.Name);
			Assert.AreEqual(expectedKGDM, foodlist[0].appliedKGDM);
			Assert.AreEqual(expectedException, Assert.Throws<Exception>(() => rationAlgorithm.GetGrassRENuturalizerFeedProduct()));
			//af maken (testcases, expectedKGDM, exceptions
		}

		[Test()]
		public void GenerateRENaturalFeedProductGroupsTest()
		{
			Assert.Fail();
		}

		[Test()]
		public void FindBestRENaturalFeedProductGroupTest()
		{
			Assert.Fail();
		}

		[Test()]
		public void DetermineImprovemendRationsWithBijprodTest()
		{
			Assert.Fail();
		}

		[Test()]
		public void FindImprovementRationMethodGrassRENuterilizerTest()
		{
			Assert.Fail();
		}

		[Test()]
		public void FindImprovementRationMethodNaturalREGroupsTest()
		{
			Assert.Fail();
		}

		[Test()]
		public void ImprovementRationMethodChangeTargetedCoveragesTest()
		{
			Assert.Fail();
		}

		//RationAlgorithmV2 Class with public getters and setters for testing purposes
		public class RationAlgorithmV2WithTestMethods : RationAlgorithmV2
		{
			public float getsetTargetedREcoveragePerKgDm
			{
				get => TargetedREcoveragePerKgDm;
				set => TargetedREcoveragePerKgDm = value;
			}

			public float getsetTargetedMaxAmountOfSupplementeryFeedProductInKGPerCow
			{
				get => TargetedMaxAmountOfSupplementeryFeedProductInKGPerCow;
				set => TargetedMaxAmountOfSupplementeryFeedProductInKGPerCow = value;
			}

			public float getsetTargetedMaxKgDmIntakePerCow
			{
				get => TargetedMaxKgDmIntakePerCow;
				set => TargetedMaxKgDmIntakePerCow = value;
			}
			public float getsetTargetedVEMCoverage { get => TargetedVEMCoverage; set => TargetedVEMCoverage = value; }
			public List<AbstractMappedFoodItem> getsetavailableFeedProducts { get => availableFeedProducts; set => availableFeedProducts = value; }
			public List<AbstractMappedFoodItem> getsetavailableRENaturalFeedProductGroups { get => availableRENaturalFeedProductGroups; set => availableRENaturalFeedProductGroups = value; }
			public List<AbstractMappedFoodItem> getsetREFoodItems { get => REFoodItems; set => REFoodItems = value; }
			public List<AbstractMappedFoodItem> getsetroughages { get => roughages; set => roughages = value; }
		}
	}

}