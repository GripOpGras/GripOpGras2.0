using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using GripOpGras2.Client.Features.CreateRation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions;
using static GripOpGras2.Client.Features.CreateRation.Tests.TestAssetsForRationAlgorithmV2Tests;

namespace GripOpGras2.Client.Features.CreateRation.Tests
{
	public class RationTests
	{

		[Test()]
		public void RationTestWithTwoProducts(float REprod1, float REprod2, float VEMprod1, float VEMprod2)
		{

		}
		[Test()]
		[TestCase(1000, 500, 500, 5000, 150, 200, 500, 5500, 5000, 10.5f, 10, 500)]
		public void TestTotalCalculations(float vem, float vem_bijprod, float appliedVEM, float appliedVEMbijpord, float RE, float RE_bijprod, float dm_bijprod, float expectedVEM, float expectedVEM_bijprod, float expectedDM, float expectedDM_bijprod, float expectedRediff)
		{
			Ration ration = new Ration();
			ration.ApplyChangesToRationList(GetMappedFeedProduct("product 1", RE, vem, appliedVEM));
			ration.ApplyChangesToRationList(GetMappedFeedProduct("product 2", RE_bijprod, vem_bijprod, appliedVEMbijpord, false));

			Console.WriteLine(ration.RationList[0].appliedKGDM);
			Console.WriteLine(ration.RationList[1].appliedKGDM);
			Assert.AreEqual(expectedVEM, ration.totalVEM, "total vem");
			Assert.AreEqual(expectedVEM_bijprod, ration.totalVEM_Bijprod, "total vem bijprod");
			Assert.AreEqual(expectedDM, ration.totalDM, "total dm");
			Assert.AreEqual(expectedDM_bijprod, ration.totalDM_Bijprod, "total dm bijprod");
			Assert.AreEqual(expectedRediff, ration.totalREdiff, "total rediff");
		}

		[Test()]
		[TestCase(1000, 500, 500, 150, 200, 500, 250, 0.75, 0.5, 25)]
		public void TestAllCalculationsWithCombos(float vem, float vem_bijprod, float appliedVEM, float RE, float RE_bijprod, float expectedVEM, float expectedVEM_bijprod, float expectedDM, float expectedDM_bijprod, float expectedRediff)
		{
			Ration ration = new Ration();
			var mappeditem1 = GetMappedFeedProduct("product 1", RE, vem, 5);
			var mappeditem2 = GetMappedFeedProduct("product 2", RE_bijprod, vem_bijprod, 5, false);
			var mappedcombo = new MappedFeedProductGroup((mappeditem1, 0.5f), (mappeditem2, 0.5f));
			mappedcombo.setAppliedVEM(appliedVEM);
			ration.ApplyChangesToRationList(mappedcombo);

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
			AbstractMappedFoodItem item1 = GetMappedFeedProduct("product 1", 10f, 100f, 10f);
			AbstractMappedFoodItem item2 = GetMappedFeedProduct("product 2", 20f, 200f, 20f);
			ration.ApplyChangesToRationList(item1);
			ration.ApplyChangesToRationList(item2);
			AbstractMappedFoodItem item3 = GetMappedFeedProduct("product 3", 20f, 200f, 8f);
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
			Assert.AreEqual(15f, ration.RationList.FirstOrDefault(x => x.originalRefference == item1.originalRefference).appliedVEM);
			Assert.AreEqual(30f, ration.RationList.FirstOrDefault(x => x.originalRefference == item2.originalRefference).appliedVEM);
			Assert.Contains(item3, ration.RationList.ToList());
		}

		[Test()]
		public void ErrorOnIllegalChangesToRationListTest()
		{
			// Arrange
			Ration ration = new Ration();
			AbstractMappedFoodItem item1 = GetMappedFeedProduct("product 1", 10f, 100f, 10f);
			AbstractMappedFoodItem item2 = GetMappedFeedProduct("product 2", 20f, 200f, 20f);
			ration.ApplyChangesToRationList(item1);
			ration.ApplyChangesToRationList(item2);
			AbstractMappedFoodItem item3 = GetMappedFeedProduct("product 3", 20f, 200f, -2f);
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
			AbstractMappedFoodItem item1 = GetMappedFeedProduct("product 1", 10f, 100f, 10f);
			ration.ApplyChangesToRationList(item1);
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
				[Test()]
		[TestCase(1, 1800, 30, 1200, 1, 800, 70, 1000, 1800 + 1200 + 800 + 70000)]
		[TestCase(800, 0, 30, 700, 50, 0, 70, 1000, 700 + 70000)]
		public void GetTotalVEMTest(float VEMPerDMprod1, float VEMprod1, float VEMPerDMprod2, float VEMprod2, float VEMPerDMBijvProd, float VEMBijvoerProd, float KGgrass, float VEMgrass, float expected)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", 189f, VEMPerDMprod1);
			FeedProduct prod2 = GetFeedProduct("prod2", 88f, VEMPerDMprod2);
			FeedProduct bijvprod = GetFeedProduct("bijvprod", 189f, VEMPerDMBijvProd, false);
			MappedFeedProduct mappedProd1 = new MappedFeedProduct(prod1);
			MappedFeedProduct mappedProd2 = new MappedFeedProduct(prod2);
			MappedFeedProduct mappedBijvprod = new MappedFeedProduct(bijvprod);
			mappedProd1.setAppliedVEM(VEMprod1);
			mappedProd2.setAppliedVEM(VEMprod2);
			mappedBijvprod.setAppliedVEM(VEMBijvoerProd);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm = CreateRationAlgorithm(new List<FeedProduct>(), KGgrass, PlotVEM: VEMgrass);
			rationAlgorithm.getsetcurrentRation.RationList =
				new List<AbstractMappedFoodItem> { mappedProd1, mappedProd2, mappedBijvprod };
			//Act
			float totalVEM = rationAlgorithm.currentRation.totalVEM;
			//Assert
			Assert.AreEqual(expected, totalVEM);
		}

		[Test]
		[TestCase(100, 200, 300, 400, 30, 1000, 500, 600, 301600)]
		[TestCase(100, 200, 300, 400, 30, 0, 500, 600, 300600f)]
		[TestCase(300, 800, 500, 0, 0, 1000, 0, 80, 1800)]
		public void GetTotalVEMTestWithCombos(float VEMPerDMprod1, float VEMprod1, float VEMPerDMprod2, float VEMprod2,
			float percentProd1, float VEMofGroup, float KGgrass, float VEMgrass, float expectedVEM)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", 189f, VEMPerDMprod1);
			FeedProduct prod2 = GetFeedProduct("prod2", 88f, VEMPerDMprod2);
			MappedFeedProduct mappedProd1 = new MappedFeedProduct(prod1);
			MappedFeedProduct mappedProd2 = new MappedFeedProduct(prod2);
			mappedProd1.setAppliedVEM(VEMprod1);
			mappedProd2.setAppliedVEM(VEMprod2);

			MappedFeedProductGroup mappedgroup = new MappedFeedProductGroup(
				(mappedProd1, percentProd1),
					(mappedProd2, 100 - percentProd1));
			mappedgroup.setAppliedVEM(VEMofGroup);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm = CreateRationAlgorithm(new List<FeedProduct>(), KGgrass, PlotVEM: VEMgrass);
			rationAlgorithm.getsetcurrentRation.RationList = new List<AbstractMappedFoodItem> { mappedProd1, mappedProd2, mappedgroup };
			//Act
			float totalVEM = rationAlgorithm.currentRation.totalVEM;
			//Assert
			Assert.AreEqual(expectedVEM, totalVEM);
		}
	}

	[TestClass()]
	[TestFixture]
	public class RationAlgorithmV2Tests
	{


		[Test()]
		public async Task CreateRationAsyncTest()
		{
			// Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", 189f, 884.2f);
			FeedProduct prod2 = GetFeedProduct("prod2", 88f, 1238f, false);
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
		[TestCase(30, 30, 1800.5f, 1860.5f)]
		[TestCase(0, 0, 1800.5f, 1800.5f)]
		[TestCase(0f, 0f, 0f, 0f)]
		[TestCase(30.5f, 15f, 0f, 45.5f)]
		public void GetTotalKgdmTest(float KGprod1, float KGprod2, float KGgrass, float expected)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", 189f, 884.2f);
			FeedProduct prod2 = GetFeedProduct("prod2", 88f, 1238f);
			MappedFeedProduct mappedProd1 = new MappedFeedProduct(prod1);
			MappedFeedProduct mappedProd2 = new MappedFeedProduct(prod2);
			mappedProd1.setAppliedVEM(KGprod1 * 884.2f);
			mappedProd2.setAppliedVEM(KGprod2 * 1238f);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm = CreateRationAlgorithm(new List<FeedProduct>() {prod1, prod2}, KGgrass);
			rationAlgorithm.getsetcurrentRation.ApplyChangesToRationList(new List<AbstractMappedFoodItem>() { mappedProd1, mappedProd2 });

			//Act
			var totalKGDM = rationAlgorithm.currentRation.totalDM;
			//Assert
			Assert.AreEqual(expected, totalKGDM);
		}


		[Test()]
		[TestCase(140, 1800, 140, 1200, 150, 1200, 200, "prod1" )] //prod 1 heeft meer VEM
		[TestCase(170,1800,160,1200,140,1200,50,"prod1")] // prod 1 heeft een groter RE difference
		[TestCase(170, 1800, 160, 1200, 140, 1200, 180, "bijprod")] //prod 1 en 2 hebben allebei net als het gras een overschot
		[TestCase(170, 1800, 149, 1200, 100, 1200, 200, "prod2")] //ruwvoer gaat altijd voor bijvoer producten.
		[TestCase(170, 1800, 160, 1200, 100, 1200, 130, "prod1")] 
		[TestCase(160, 1800, 170, 1800, 100, 1200, 130, "prod2")] 
		public void GetGrassRENuturalizerFeedProductTest(float REperDMprod1, float VEMPerDMprod1, float REperDMprod2, float VEMPerDMprod2, float REperDMBijprod, float VEMPerDMbijprod, float REgrass, string expectedProd)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", REperDMprod1, VEMPerDMprod1);
			FeedProduct prod2 = GetFeedProduct("prod2", REperDMprod2, VEMPerDMprod2);
			FeedProduct bijprod = GetFeedProduct("bijprod", REperDMBijprod, VEMPerDMbijprod, false);
			RationAlgorithmV2Tests.RationAlgorithmV2WithTestMethods rationAlgorithm =
				CreateRationAlgorithm(new List<FeedProduct> { prod1, prod2, bijprod }, totalGrassIntake: 700, PlotVEM: 1000, PlotRE: REgrass);
			//Act
			List<AbstractMappedFoodItem> foodlist = rationAlgorithm.GetGrassRENuturalizerFeedProduct();
			rationAlgorithm.getsetcurrentRation.ApplyChangesToRationList(foodlist);
			//Assert
			Assert.AreEqual(1, foodlist.Count);
			Assert.AreEqual(expectedProd, foodlist[0].GetProducts()[0].Item1.Name);
			Assert.AreEqual(150, rationAlgorithm.getsetcurrentRation.totalRE/rationAlgorithm.getsetcurrentRation.totalDM, 1);
		}

		[Test()]
		public void ExceptionByGetGrassRENuturalizerFeedProductTest()
		{
			FeedProduct prod1 = GetFeedProduct("prod 1", 160, 800);
			FeedProduct prod2 = GetFeedProduct("prod 2", 190, 11000);
			RationAlgorithmV2WithTestMethods ration = CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2 }, PlotRE: 200, PlotVEM: 1000, totalGrassIntake:500);
			Assert.Throws<NoProductsWithPossibleREException>(() => ration.GetGrassRENuturalizerFeedProduct());
		}

		[Test()]
		public void GenerateRENaturalFeedProductGroupsTest()
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod 1", 90, 800);
			FeedProduct prod2 = GetFeedProduct("prod 2", 150, 11000);
			FeedProduct prod3 = GetFeedProduct("prod 3", 180, 11000);
			FeedProduct prod4 = GetFeedProduct("prod 4", 140, 11000, false);
			//make a rationAlgorithm
			RationAlgorithmV2WithTestMethods ration = CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2, prod3, prod4 });
			//Act
			List<AbstractMappedFoodItem> groups = ration.GenerateRENaturalFeedProductGroups();
			//Assert
			Assert.AreEqual(3, groups.Count, "amount of expected groups"); //this can be more when more supplementery datapoints are added.
			Assert.AreEqual(0, groups[0].REdiffPerVEM, 0.01, "group 0 RE");
			Assert.AreEqual(0, groups[1].REdiffPerVEM, 0.01, "group 1 RE");
			Assert.AreEqual(0, groups[2].REdiffPerVEM, 0.01, "group 2 RE");
		}
		public void GroupGenerateRENaturalFeedProductGroupsTest()
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod 1", 90, 100);
			FeedProduct prod2 = GetFeedProduct("prod 4", 160, 200, false);
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
			FeedProduct prod1 = GetFeedProduct("prod 1", 90, 100);
			FeedProduct prod2 = GetFeedProduct("prod 2", 160, 200);
			FeedProduct prod3 = GetFeedProduct("prod 3", 180, 200);
			FeedProduct prod4 = GetFeedProduct("prod 4", 140, 400, false);
			//make a rationAlgorithm
			RationAlgorithmV2WithTestMethods ration = CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2, prod3, prod4 });
			var groups = ration.GenerateRENaturalFeedProductGroups();
			ration.getsetavailableRENaturalFeedProductGroups = groups;
			//Act
			AbstractMappedFoodItem group = ration.FindBestRENaturalFeedProductGroup(false);
			AbstractMappedFoodItem group2 = ration.FindBestRENaturalFeedProductGroup(true);
			//Assert
			Assert.Contains(prod1, group.GetProducts().Select(x => x.Item1).ToList(), "group1; prod1");
			Assert.Contains(prod3, group.GetProducts().Select(x => x.Item1).ToList(), "group1, prod3");
			Assert.Contains(prod4, group2.GetProducts().Select(x => x.Item1).ToList(), "group2, prod4");
			Assert.Contains(prod2, group2.GetProducts().Select(x => x.Item1).ToList(),"group2, prod2");
		}

		//TODO: test for determine improvemend rations with bijprod test
		[Test()]
		public void DetermineImprovemendRationsWithBijprodTest()
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod 1", 90, 100);
			FeedProduct prod2 = GetFeedProduct("prod 2", 160, 200);
			FeedProduct prod3 = GetFeedProduct("prod 3", 180, 200);
			FeedProduct prod4 = GetFeedProduct("prod 4", 140, 400, false);
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
			public List<AbstractMappedFoodItem> getsetavailableFeedProducts { get => availableFeedProducts.ToList(); set => availableFeedProducts = value.ToList(); }
			public List<AbstractMappedFoodItem> getsetavailableRENaturalFeedProductGroups { get => availableRENaturalFeedProductGroups; set => availableRENaturalFeedProductGroups = value; }
			public Ration getsetcurrentRation { get => _currentRation; set => _currentRation = value; }
		}
	}

}