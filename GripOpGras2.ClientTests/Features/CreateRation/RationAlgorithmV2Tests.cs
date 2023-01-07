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
using GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions;

namespace GripOpGras2.Client.Features.CreateRation.Tests
{
	public class RationTests
	{

		[Test()]
		public void RationTestWithTwoProducts(float REprod1, float REprod2, float VEMprod1, float VEMprod2)
		{

		}
		[Test()]
		[TestCase(1000, 500, 500, 5000, 150, 200, 500, 5500, 5000, 500 / 1000 + 5000 / 500, 5000 / 500, 50 / 500 * 5000)]
		public void TestTotalCalculations(float vem, float vem_bijprod, float appliedVEM, float appliedVEMbijpord, float RE, float RE_bijprod, float dm_bijprod, float expectedVEM, float expectedVEM_bijprod, float expectedDM, float expectedDM_bijprod, float expectedRediff)
		{
			Ration ration = new Ration();
			ration.RationList.Add(TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 1", RE, vem, appliedVEM));
			ration.RationList.Add(TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 2", RE_bijprod, vem_bijprod, appliedVEMbijpord, false));

			Assert.AreEqual(expectedVEM, ration.totalVEM);
			Assert.AreEqual(expectedVEM_bijprod, ration.totalVEM_Bijprod);
			Assert.AreEqual(expectedDM, ration.totalDM);
			Assert.AreEqual(expectedDM_bijprod, ration.totalDM_Bijprod);
			Assert.AreEqual(expectedRediff, ration.totalREdiff);
		}

		[Test()]
		[TestCase(1000, 500, 500, 150, 200, 500, 500, 250 / 1000 + 250 / 500, 250 / 500, 50 / 500 * 250)]
		public void TestAllCalculationsWithCombos(float vem, float vem_bijprod, float appliedVEM, float RE, float RE_bijprod, float expectedVEM, float expectedVEM_bijprod, float expectedDM, float expectedDM_bijprod, float expectedRediff)
		{
			Ration ration = new Ration();
			var mappeditem1 = TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 1", RE, vem, 5);
			var mappeditem2 = TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 2", RE_bijprod, vem_bijprod, 5, false);
			var mappedcombo = new MappedFeedProductGroup((mappeditem1, 0.5f), (mappeditem2, 0.5f));
			mappedcombo.setAppliedVEM(appliedVEM);
			ration.RationList.Add(mappedcombo);

			Assert.AreEqual(expectedVEM, ration.totalVEM);
			Assert.AreEqual(expectedVEM_bijprod, ration.totalVEM_Bijprod);
			Assert.AreEqual(expectedDM, ration.totalDM);
			Assert.AreEqual(expectedDM_bijprod, ration.totalDM_Bijprod);
			Assert.AreEqual(expectedRediff, ration.totalREdiff);
		}


		[Test()]
		public void ApplyChangesToRationListTest()
		{
			// Arrange
			Ration ration = new Ration();
			AbstractMappedFoodItem item1 = TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 1", 10f, 100f, 10f);
			AbstractMappedFoodItem item2 = TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 2", 20f, 200f, 20f);
			ration.RationList.Add(item1);
			ration.RationList.Add(item2);
			AbstractMappedFoodItem item3 = TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 3", 20f, 200f, 8f);
			List<AbstractMappedFoodItem> changes = new List<AbstractMappedFoodItem>
			{
				item1.Clone(),
				item2.Clone(),
				item3
			};
			changes[0].setAppliedVEM(5);
			changes[1].setAppliedVEM(10);

			// Act
			ration.ApplyChangesToRationList(changes);

			// Assert
			Assert.AreEqual(15f, ration.RationList.Find(x => x.originalRefference == item1.originalRefference).appliedVEM);
			Assert.AreEqual(30f, ration.RationList.Find(x => x.originalRefference == item2.originalRefference).appliedVEM);
			Assert.Contains(item3, ration.RationList);
		}

		[Test()]
		public void ErrorOnIllegalChangesToRationListTest()
		{
			// Arrange
			Ration ration = new Ration();
			AbstractMappedFoodItem item1 = TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 1", 10f, 100f, 10f);
			AbstractMappedFoodItem item2 = TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 2", 20f, 200f, 20f);
			ration.RationList.Add(item1);
			ration.RationList.Add(item2);
			AbstractMappedFoodItem item3 = TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 3", 20f, 200f, -2f);
			List<AbstractMappedFoodItem> changes = new List<AbstractMappedFoodItem>
			{
				item1.Clone(),
				item2.Clone(),
				item3
			};
			changes[0].setAppliedVEM(5);
			changes[1].setAppliedVEM(10);
			//Assert
			Assert.Throws<RationAlgorithmException>(() => ration.ApplyChangesToRationList(changes));
		}

		[Test()]
		public void CloneTest()
		{
			// Arrange
			Ration ration = new Ration();
			AbstractMappedFoodItem item1 = TestassetsForRationAlgorithmV2Tests.GetMappedFeedProduct("product 1", 10f, 100f, 10f);
			ration.RationList.Add(item1);
			//Act
			Ration clone = ration.Clone();
			List<AbstractMappedFoodItem> changes = new List<AbstractMappedFoodItem>
			{
				item1.Clone()
			};
			changes[0].setAppliedVEM(5);
			clone.ApplyChangesToRationList(changes);
			//Assert
			Assert.AreSame(ration.originalRefference, clone.originalRefference);
			Assert.AreNotSame(ration, clone);
			Assert.AreSame(item1.originalRefference, clone.RationList[0].originalRefference);

			Assert.AreNotEqual(ration.totalVEM, clone.totalVEM);
			Assert.AreNotEqual(ration.totalDM, clone.totalDM);
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
		public static MappedFeedProduct GetMappedFeedProduct(string name, float re, float vem, float appliedVEM, bool isRoughage = true)
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
		public RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods CreateRationAlgorithm(List<FeedProduct> feedProducts, float totalGrassIntake = 1062.5f,
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
		public async void CreateRationAsyncTest()
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
			var feedRotation = await rationAlgorithm.CreateRationAsync(
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
		public void GetTotalKgdmTest(float KGprod1, float KGprod2, float KGgrass, float expected)
		{
			//Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod1", 189f, 884.2f);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod2", 88f, 1238f);
			MappedFeedProduct mappedProd1 = new MappedFeedProduct(prod1);
			MappedFeedProduct mappedProd2 = new MappedFeedProduct(prod2);
			mappedProd1.setAppliedVEM(KGprod1 * 884.2f);
			mappedProd2.setAppliedVEM(KGprod2 * 1238f);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm = CreateRationAlgorithm(new List<FeedProduct>(), KGgrass);

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
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm = CreateRationAlgorithm(new List<FeedProduct>(), KGgrass, PlotVEM: VEMgrass);
			rationAlgorithm.getsetroughages =
				new List<AbstractMappedFoodItem> { mappedProd1, mappedProd2, mappedBijvprod };
			//Act
			float totalVEM = rationAlgorithm.GetTotalVEM();
			//Assert
			Assert.AreEqual(expected, totalVEM);
		}

		[Test]
		[TestCase(100, 200, 300, 400, 30, 10, 500, 600, 200 + 400 + 500 * 600 + 10 * 0.3 * 100 + 10 * 0.7 * 300)]
		[TestCase(100, 200, 300, 400, 30, 0, 500, 600, 300600f)]
		[TestCase(300, 800, 500, 0, 0, 100, 0, 80, 800 + 500 * 100)]
		public void GetTotalVEMTestWithCombos(float VEMPerDMprod1, float VEMprod1, float VEMPerDMprod2, float VEMprod2,
			float percentProd1, float KGofGroup, float KGgrass, float VEMgrass, float expectedVEM)
		{
			//Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod1", 189f, VEMPerDMprod1);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod2", 88f, VEMPerDMprod2);
			MappedFeedProduct mappedProd1 = new MappedFeedProduct(prod1);
			MappedFeedProduct mappedProd2 = new MappedFeedProduct(prod2);
			mappedProd1.setAppliedVEM(VEMprod1);
			mappedProd2.setAppliedVEM(VEMprod2);

			MappedFeedProductGroup mappedgroup = new MappedFeedProductGroup(
				(mappedProd1, percentProd1),
					(mappedProd2, 100 - percentProd1));
			mappedgroup.setAppliedVEM(KGofGroup);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm = CreateRationAlgorithm(new List<FeedProduct>(), KGgrass, PlotVEM: VEMgrass);
			rationAlgorithm.getsetroughages = new List<AbstractMappedFoodItem> { mappedProd1, mappedProd2, mappedgroup };
			//Act
			float totalVEM = rationAlgorithm.GetTotalVEM();
			//Assert
			Assert.AreEqual(expectedVEM, totalVEM);
		}

		[Test()]
		[TestCase(1, 1800, 30, 1200, 1, 800, 70, 1000, 1)]
		public void GetGrassRENuturalizerFeedProductTest(float REperDMprod1, float VEMPerDMprod1, float REperDMprod2, float VEMPerDMprod2, float REperDMBijprod, float VEMPerDMbijprod, float KGgrass, float VEMgrass, float REgrass, string expectedProd, float expectedKGDM)
		{
			//Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod1", REperDMprod1, VEMPerDMprod1);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod2", REperDMprod2, VEMPerDMprod2);
			FeedProduct bijprod = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("bijprod", REperDMBijprod, VEMPerDMbijprod, false);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm =
				CreateRationAlgorithm(new List<FeedProduct> { prod1, prod2, bijprod }, KGgrass, PlotVEM: VEMgrass, PlotRE: REgrass);
			//Act
			List<AbstractMappedFoodItem> foodlist = rationAlgorithm.GetGrassRENuturalizerFeedProduct();
			var ration = new Ration();
			ration.RationList = foodlist;
			//Assert
			Assert.AreEqual(1, foodlist.Count);
			Assert.AreEqual(expectedProd, foodlist[0].GetProducts()[0].Item1.Name);
			Assert.AreEqual(expectedKGDM, foodlist[0].appliedKGDM);
			Assert.AreEqual(150, ration.totalREdiff, 1);
		}

		[Test()]
		public void ExceptionByGetGrassRENuturalizerFeedProductTest()
		{
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 1", 160, 800);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 2", 190, 11000);
			RationAlgorithmV2WithTestMethods ration = CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2 });
			Assert.Throws<NoProductsWithPossibleREException>(() => ration.GetGrassRENuturalizerFeedProduct());
		}

		[Test()]
		public void GenerateRENaturalFeedProductGroupsTest()
		{
			//Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 1", 90, 800);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 2", 150, 11000);
			FeedProduct prod3 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 3", 180, 11000);
			FeedProduct prod4 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 4", 140, 11000, false);
			//make a rationAlgorithm
			RationAlgorithmV2WithTestMethods ration = CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2, prod3, prod4 });
			//Act
			List<AbstractMappedFoodItem> groups = ration.GenerateRENaturalFeedProductGroups();
			//Assert
			Assert.AreEqual(3, groups.Count); //this can be more when more supplementery datapoints are added.
			Assert.AreEqual(0, groups[0]);
			Assert.AreEqual(0, groups[1]);
			Assert.AreEqual(0, groups[2]);
		}
		public void GroupGenerateRENaturalFeedProductGroupsTest()
		{
			//Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 1", 90, 100);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 4", 160, 200, false);
			//make a rationAlgorithm
			RationAlgorithmV2WithTestMethods ration = CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2 });
			//Act
			List<AbstractMappedFoodItem> groups = ration.GenerateRENaturalFeedProductGroups();
			//Assert
			Assert.AreEqual(1, groups.Count);
			Assert.Contains(prod1, groups[0].GetProducts());
			Assert.Contains(prod2, groups[0].GetProducts());
			Assert.AreEqual((0.917), groups[0].partOfTotalVEMbijprod);
		}

		[Test()]
		public void FindBestRENaturalFeedProductGroupTest()
		{
			//Arrange
			FeedProduct prod1 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 1", 90, 100);
			FeedProduct prod2 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 2", 160, 200);
			FeedProduct prod3 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 3", 180, 200);
			FeedProduct prod4 = TestassetsForRationAlgorithmV2Tests.GetFeedProduct("prod 4", 140, 400, false);
			//make a rationAlgorithm
			RationAlgorithmV2WithTestMethods ration = CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2, prod3, prod4 });
			var groups = ration.GenerateRENaturalFeedProductGroups();
			ration.getsetavailableRENaturalFeedProductGroups = groups;
			//Act
			AbstractMappedFoodItem group = ration.FindBestRENaturalFeedProductGroup(false);
			AbstractMappedFoodItem group2 = ration.FindBestRENaturalFeedProductGroup(true);
			//Assert
			Assert.Contains(prod1, group.GetProducts());
			Assert.Contains(prod3, group.GetProducts());
			Assert.Contains(prod4, group2.GetProducts());
			Assert.Contains(prod2, group2.GetProducts());
		}

		//TODO: test for determine improvemend rations with bijprod test
		[Test()]
		public void DetermineImprovemendRationsWithBijprodTest()
		{
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
			public List<AbstractMappedFoodItem> getsetavailableFeedProducts { get => availableFeedProducts; set => availableFeedProducts = value; }
			public List<AbstractMappedFoodItem> getsetavailableRENaturalFeedProductGroups { get => availableRENaturalFeedProductGroups; set => availableRENaturalFeedProductGroups = value; }
			public List<AbstractMappedFoodItem> getsetREFoodItems { get => REFoodItems; set => REFoodItems = value; }
			public List<AbstractMappedFoodItem> getsetroughages { get => roughages; set => roughages = value; }
		}
	}

}