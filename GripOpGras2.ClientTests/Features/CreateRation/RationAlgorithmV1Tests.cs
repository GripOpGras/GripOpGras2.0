using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using GripOpGras2.Client.Features.CreateRation;
using System;
using System.Collections;
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
using static GripOpGras2.Client.Features.CreateRation.Tests.TestAssetsForRationAlgorithmV1Tests;

namespace GripOpGras2.Client.Features.CreateRation.Tests
{
	public class RationTests
	{
		[Test()]
		[TestCase(1000, 500, 500, 5000, 150, 200, 5500, 5000, 10.5f, 10, 500)]
		public void TestTotalCalculations(float vem, float vem_bijprod, float appliedVEM, float appliedVEMbijpord,
			float RE, float RE_bijprod, float expectedVEM, float expectedVEM_bijprod,
			float expectedDM, float expectedDM_bijprod, float expectedRediff)
		{
			Ration ration = new();
			ration.ApplyChangesToRationList(GetMappedFeedProduct("product 1", RE, vem, appliedVEM));
			ration.ApplyChangesToRationList(GetMappedFeedProduct("product 2", RE_bijprod, vem_bijprod,
				appliedVEMbijpord, false));

			Console.WriteLine(ration.RationList[0].AppliedKgdm);
			Console.WriteLine(ration.RationList[1].AppliedKgdm);
			Assert.AreEqual(expectedVEM, ration.totalVEM, "total vem");
			Assert.AreEqual(expectedVEM_bijprod, ration.totalVEM_Bijprod, "total vem bijprod");
			Assert.AreEqual(expectedDM, ration.totalDM, "total dm");
			Assert.AreEqual(expectedDM_bijprod, ration.totalDM_Bijprod, "total dm bijprod");
			Assert.AreEqual(expectedRediff, ration.totalREdiff, "total rediff");
		}

		[Test()]
		[TestCase(1000, 500, 500, 150, 200, 500, 250, 0.75f, 0.5f, 25)]
		public void TestAllCalculationsWithCombos(float vem, float vem_bijprod, float appliedVEM, float RE,
			float RE_bijprod, float expectedVEM, float expectedVEM_bijprod, float expectedDM, float expectedDM_bijprod,
			float expectedRediff)
		{
			Ration ration = new();
			MappedFeedProduct mappeditem1 = GetMappedFeedProduct("product 1", RE, vem, 5);
			MappedFeedProduct mappeditem2 = GetMappedFeedProduct("product 2", RE_bijprod, vem_bijprod, 5, false);
			MappedFeedProductGroup mappedcombo = new((mappeditem1, 0.5f), (mappeditem2, 0.5f));
			mappedcombo.SetAppliedVem(appliedVEM);
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
			Ration ration = new();
			AbstractMappedFoodItem item1 = GetMappedFeedProduct("product 1", 10f, 100f, 10f);
			AbstractMappedFoodItem item2 = GetMappedFeedProduct("product 2", 20f, 200f, 20f);
			ration.ApplyChangesToRationList(item1);
			ration.ApplyChangesToRationList(item2);
			AbstractMappedFoodItem item3 = GetMappedFeedProduct("product 3", 20f, 200f, 8f);
			List<AbstractMappedFoodItem> changes = new()
			{
				item1.Clone(),
				item2.Clone(),
				item3
			};
			changes[0].SetAppliedVem(5);
			changes[1].SetAppliedVem(10);

			// Act
			ration.ApplyChangesToRationList(changes);

			// Assert
			Assert.AreEqual(15f,
				ration.RationList.First(x => x.OriginalReference == item1.OriginalReference).AppliedVem);
			Assert.AreEqual(30f,
				ration.RationList.First(x => x.OriginalReference == item2.OriginalReference).AppliedVem);
			Assert.Contains(item3, ration.RationList.ToList());
		}

		[Test()]
		public void ErrorOnIllegalChangesToRationListTest()
		{
			// Arrange
			Ration ration = new();
			AbstractMappedFoodItem item1 = GetMappedFeedProduct("product 1", 10f, 100f, 10f);
			AbstractMappedFoodItem item2 = GetMappedFeedProduct("product 2", 20f, 200f, 20f);
			ration.ApplyChangesToRationList(item1);
			ration.ApplyChangesToRationList(item2);
			AbstractMappedFoodItem item3 = GetMappedFeedProduct("product 3", 20f, 200f, -2f);
			List<AbstractMappedFoodItem> changes = new()
			{
				item1.Clone(),
				item2.Clone(),
				item3
			};
			changes[0].SetAppliedVem(5);
			changes[1].SetAppliedVem(10);
			//Assert
			Assert.Throws<RationAlgorithmException>(() => ration.ApplyChangesToRationList(changes));
		}

		[Test()]
		public void CloneTest()
		{
			// Arrange
			Ration ration = new();
			AbstractMappedFoodItem item1 = GetMappedFeedProduct("product 1", 10f, 100f, 10f);
			ration.ApplyChangesToRationList(item1);
			//Act
			Ration clone = ration.Clone();
			List<AbstractMappedFoodItem> changes = new()
			{
				item1.Clone()
			};
			changes[0].SetAppliedVem(5);
			clone.ApplyChangesToRationList(changes);
			//Assert
			Assert.AreSame(ration.originalRefference, clone.originalRefference);
			Assert.AreNotSame(ration, clone);
			Assert.AreSame(item1.OriginalReference, clone.RationList[0].OriginalReference);

			Assert.AreNotEqual(ration.totalVEM, clone.totalVEM);
			Assert.AreNotEqual(ration.totalDM, clone.totalDM);
		}

		[Test()]
		[TestCase(1, 1800, 30, 1200, 1, 800, 70, 1000, 1800 + 1200 + 800 + 70000)]
		[TestCase(800, 0, 30, 700, 50, 0, 70, 1000, 700 + 70000)]
		public void GetTotalVEMTest(float VEMPerDMprod1, float VEMprod1, float VEMPerDMprod2, float VEMprod2,
			float VEMPerDMBijvProd, float VEMBijvoerProd, float KGgrass, float VEMgrass, float expected)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", 189f, VEMPerDMprod1);
			FeedProduct prod2 = GetFeedProduct("prod2", 88f, VEMPerDMprod2);
			FeedProduct bijvprod = GetFeedProduct("bijvprod", 189f, VEMPerDMBijvProd, false);
			MappedFeedProduct mappedProd1 = new(prod1);
			MappedFeedProduct mappedProd2 = new(prod2);
			MappedFeedProduct mappedBijvprod = new(bijvprod);
			mappedProd1.SetAppliedVem(VEMprod1);
			mappedProd2.SetAppliedVem(VEMprod2);
			mappedBijvprod.SetAppliedVem(VEMBijvoerProd);
			RationAlgorithmV1 rationAlgorithm =
				CreateRationAlgorithm(new List<FeedProduct>(), KGgrass, PlotVEM: VEMgrass);
			rationAlgorithm.CurrentRation.RationList =
				new List<AbstractMappedFoodItem> { mappedProd1, mappedProd2, mappedBijvprod };
			//Act
			float totalVEM = rationAlgorithm.CurrentRation.totalVEM;
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
			MappedFeedProduct mappedProd1 = new(prod1);
			MappedFeedProduct mappedProd2 = new(prod2);
			mappedProd1.SetAppliedVem(VEMprod1);
			mappedProd2.SetAppliedVem(VEMprod2);

			MappedFeedProductGroup mappedgroup = new(
				(mappedProd1, percentProd1),
				(mappedProd2, 100 - percentProd1));
			mappedgroup.SetAppliedVem(VEMofGroup);
			RationAlgorithmV1 rationAlgorithm =
				CreateRationAlgorithm(new List<FeedProduct>(), KGgrass, PlotVEM: VEMgrass);
			rationAlgorithm.CurrentRation.RationList = new List<AbstractMappedFoodItem>
				{ mappedProd1, mappedProd2, mappedgroup };
			//Act
			float totalVEM = rationAlgorithm.CurrentRation.totalVEM;
			//Assert
			Assert.AreEqual(expectedVEM, totalVEM);
		}
	}

	[TestClass()]
	[TestFixture]
	public class RationAlgorithmV1Tests
	{
		[Test()]
		[TestCase(165f, 884.2f, 70f, 1000.2f, 88f, 1338f, 1063.5f, 1210f,
			210f)] //realistic input. but it gives an error. Taiga #197
		[TestCase(165f, 2000.2f, 50f, 1800.2f, 88f, 1338f, 500.5f, 1600f,
			210f)] //not realistic, but should give an output.
		[TestCase(160f, 920f, 60f, 960f, 0, 0, 1062.5f, 1000, 210)] // HF5.4.5
		[TestCase(160f, 920f, 60f, 960f, 60, 1300, 1062.5f, 1000, 210)] // HF5.4.5 aangepast
		public async Task CreateRationAsyncTest(float prod1RE, float prod1VEM, float prod2RE, float prod2VEM,
			float bijprodRE, float bijprodVEM, float totalgrassIntake, float grassVEM, float grassRE)
		{
			// Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", prod1RE, prod1VEM);
			FeedProduct prod2 = GetFeedProduct("prod2", prod2RE, prod2VEM);
			FeedProduct prod3 = GetFeedProduct("bijprod", bijprodRE, bijprodVEM, false);
			Herd herd = new()
			{
				Name = "Herd1",
				NumberOfAnimals = 100,
				Type = "Koe"
			};
			MilkProductionAnalysis milkProductionAnalysis = new()
			{
				Date = DateTime.Now,
				Amount = 3000f
			};
			GrazingActivity grazingActivity = new()
			{
				Herd = herd,
				Plot = new Plot()
				{
					Area = 100,
					Name = "TestcaseGrass",
					NetDryMatter = totalgrassIntake * 1.1f,
					FeedAnalysis = new FeedAnalysis()
					{
						VEM = grassVEM,
						RE = grassRE,
					}
				}
			};
			CreateRation.RationAlgorithmV1 rationAlgorithm = new();
			List<FeedProduct> feedProducts = new();
			if (prod1.FeedAnalysis.VEM != 0) feedProducts.Add(prod1);
			if (prod2.FeedAnalysis.VEM != 0) feedProducts.Add(prod2);
			if (prod3.FeedAnalysis.VEM != 0) feedProducts.Add(prod3);
			// Act
			FeedRation feedRotation = await rationAlgorithm.CreateRationAsync(
				feedProducts: feedProducts,
				herd: herd,
				totalGrassIntake: totalgrassIntake,
				milkProductionAnalysis: milkProductionAnalysis,
				grazingActivity: grazingActivity);

			// Assert
			Assert.NotNull(feedRotation);
			Assert.NotNull(feedRotation.FeedProducts);
			Assert.Contains(prod1, feedRotation.FeedProducts!.Keys);
			Assert.Contains(prod2, feedRotation.FeedProducts.Keys);
			Assert.AreEqual(totalgrassIntake, feedRotation.GrassIntake);
			Assert.AreSame(herd, feedRotation.Herd);
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
			MappedFeedProduct mappedProd1 = new(prod1);
			MappedFeedProduct mappedProd2 = new(prod2);
			mappedProd1.SetAppliedVem(KGprod1 * 884.2f);
			mappedProd2.SetAppliedVem(KGprod2 * 1238f);
			RationAlgorithmV1 rationAlgorithm =
				CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2 }, KGgrass);
			rationAlgorithm.CurrentRation.ApplyChangesToRationList(new List<AbstractMappedFoodItem>()
				{ mappedProd1, mappedProd2 });

			//Act
			float totalKGDM = rationAlgorithm.CurrentRation.totalDM;
			//Assert
			Assert.AreEqual(expected, totalKGDM);
		}


		[Test()]
		[TestCase(140, 1800, 140, 1200, 150, 1200, 200, "prod1")] //prod 1 heeft meer VEM
		[TestCase(170, 1800, 160, 1200, 140, 1200, 50, "prod1")] // prod 1 heeft een groter RE difference
		[TestCase(170, 1800, 160, 1200, 140, 1200, 180,
			"bijprod")] //prod 1 en 2 hebben allebei net als het gras een overschot
		[TestCase(170, 1800, 149, 1200, 100, 1200, 200, "prod2")] //ruwvoer gaat altijd voor bijvoer producten.
		[TestCase(170, 1800, 160, 1200, 100, 1200, 130, "prod1")]
		[TestCase(160, 1800, 170, 1800, 100, 1200, 130, "prod2")]
		public void GetGrassRENuturalizerFeedProductTest(float REperDMprod1, float VEMPerDMprod1, float REperDMprod2,
			float VEMPerDMprod2, float REperDMBijprod, float VEMPerDMbijprod, float REgrass, string expectedProd)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", REperDMprod1, VEMPerDMprod1);
			FeedProduct prod2 = GetFeedProduct("prod2", REperDMprod2, VEMPerDMprod2);
			FeedProduct bijprod = GetFeedProduct("bijprod", REperDMBijprod, VEMPerDMbijprod, false);
			RationAlgorithmV1 rationAlgorithm =
				CreateRationAlgorithm(new List<FeedProduct> { prod1, prod2, bijprod }, totalGrassIntake: 700,
					PlotVEM: 1000, PlotRE: REgrass);
			//Act
			List<AbstractMappedFoodItem> foodlist = rationAlgorithm.GetGrassRENuturalizerFeedProduct();
			rationAlgorithm.CurrentRation.ApplyChangesToRationList(foodlist);
			//Assert
			Assert.AreEqual(1, foodlist.Count);
			Assert.AreEqual(expectedProd, foodlist[0].GetProducts().First().Key.Name);
			Assert.AreEqual(150,
				rationAlgorithm.CurrentRation.totalRE / rationAlgorithm.CurrentRation.totalDM, 1);
		}

		[Test()]
		public void ExceptionByGetGrassRENuturalizerFeedProductTest()
		{
			FeedProduct prod1 = GetFeedProduct("prod 1", 160, 800);
			FeedProduct prod2 = GetFeedProduct("prod 2", 190, 11000);
			RationAlgorithmV1 ration = CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2 },
				PlotRE: 200, PlotVEM: 1000, totalGrassIntake: 500);
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
			RationAlgorithmV1 ration = CreateRationAlgorithm(new List<FeedProduct>()
				{ prod1, prod2, prod3, prod4 });
			//Act
			List<AbstractMappedFoodItem> groups = ration.GenerateRENaturalFeedProductGroups();
			//Assert
			Assert.AreEqual(3, groups.Count,
				"amount of expected groups"); //this can be more when more supplementery datapoints are added.
			Assert.AreEqual(0, groups[0].REdiffPerVem, 0.01, "group 0 RE");
			Assert.AreEqual(0, groups[1].REdiffPerVem, 0.01, "group 1 RE");
			Assert.AreEqual(0, groups[2].REdiffPerVem, 0.01, "group 2 RE");
		}

		[Test()]
		public void GroupGenerateReNaturalFeedProductGroupsTest()
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod 1", 90, 100);
			FeedProduct prod2 = GetFeedProduct("prod 4", 160, 200, false);
			//make a rationAlgorithm
			RationAlgorithmV1 ration = CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2 });
			//Act
			List<AbstractMappedFoodItem> groups = ration.GenerateRENaturalFeedProductGroups();
			//Assert
			Assert.AreEqual(1, groups.Count);
			Assert.Contains(prod1.Name, groups[0].GetProducts().Select(x => x.Key.Name).ToList());
			Assert.Contains(prod2.Name, groups[0].GetProducts().Select(x => x.Key.Name).ToList());
			Assert.AreEqual((0.917), groups[0].SupplmenteryPartOfTotalVem, 0.01);
		}

		[Test()]
		[TestCase(false, "prod1",
			"prod2")] //prod 1 is de enige met een negatieve RE. maar omdat de VEM daar lager is dan product 2 en 3 die erbij kunnen, ligt de voorkeur voor in verhouding het meest prod 2&3. Daarom wordt er gekozen voor prod 3.
		[TestCase(true, "prod4",
			"prod3")] //Wanneer het bijprod gekozen kan worden geeft deze het minste KG DM per VEM. Daarom wordt er zoveel mogenlijk van het bijprod gekozen in verhouding. Daarom wordt er niet voor prod 3 maar voor 2 gekozen. In de toekomst zou er beter voorkeur komen liggen op zo min mogenlijk bijporduct, zodat deze altijd nog in de verbeterrondes gekozen kunnen worden wanneer nodig.
		[TestCase(true, "prod4",
			"prod2")] //#TODO Wanneer er gekozen wordt om wel bijproducten te gebruiken, probeer dan eerst van de bijproducten een verhouding te gebruiken waarvoor weinig bijrpoduct gebruikt wordt, zodat deze later nog verbeterd kan worden in de improvemet rondes.; Taiga: #196
		public void FindBestRENaturalFeedProductGroupTest(bool findREnaturalFeedproductGroup, string expectedproduct1,
			string expectedproduct2)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", 90, 100);
			FeedProduct prod2 = GetFeedProduct("prod2", 160, 200);
			FeedProduct prod3 = GetFeedProduct("prod3", 180, 200);
			FeedProduct prod4 = GetFeedProduct("prod4", 140, 400, false);
			//make a rationAlgorithm
			RationAlgorithmV1 ration = CreateRationAlgorithm(new List<FeedProduct>()
				{ prod1, prod2, prod3, prod4 });
			List<AbstractMappedFoodItem> groups = ration.GenerateRENaturalFeedProductGroups();
			ration.availableRENaturalFeedProductGroups = groups;
			//Act
			AbstractMappedFoodItem group = ration.FindBestRENaturalFeedProductGroup(findREnaturalFeedproductGroup);
			//Assert
			Assert.Contains(expectedproduct1, group.GetProducts().Select(x => x.Key.Name).ToList(),
				$"naturalFeedproductgroup: {findREnaturalFeedproductGroup}; prod: {expectedproduct1}");
			Assert.Contains(expectedproduct2, group.GetProducts().Select(x => x.Key.Name).ToList(),
				$"naturalFeedproductgroup: {findREnaturalFeedproductGroup}; prod: {expectedproduct1}");
		}

		//TODO: test for determine improvemend rations with bijprod test
		[Test()]
		public void DetermineImprovemendRationsWithBijprodTest()
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod 1", 90, 50);
			FeedProduct prod2 = GetFeedProduct("prod 2", 160, 200);
			FeedProduct prod3 = GetFeedProduct("prod 3", 180, 200);
			FeedProduct prod4 = GetFeedProduct("prod 4", 140, 400, false);
			//make a rationAlgorithm
			RationAlgorithmV1 ration = CreateRationAlgorithm(new List<FeedProduct>()
				{ prod1, prod2, prod3, prod4 });
			List<AbstractMappedFoodItem> groups = ration.GenerateRENaturalFeedProductGroups();
			ration.availableRENaturalFeedProductGroups = groups;
			//Act
			AbstractMappedFoodItem group = ration.FindBestRENaturalFeedProductGroup(false);
			AbstractMappedFoodItem group2 = ration.FindBestRENaturalFeedProductGroup(true);
			//Assert
			Assert.Contains(prod1.Name, group.GetProducts().Select(x => x.Key.Name).ToList());
			Assert.Contains(prod2.Name, group.GetProducts().Select(x => x.Key.Name).ToList());
			Assert.Contains(prod4.Name, group2.GetProducts().Select(x => x.Key.Name).ToList());
			Assert.Contains(prod2.Name, group2.GetProducts().Select(x => x.Key.Name).ToList());
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
		
	}
}