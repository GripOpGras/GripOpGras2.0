﻿using GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions;
using GripOpGras2.Client.Features.CreateRation;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using static GripOpGras2.ClientTests.Features.CreateRation.TestAssetsForRationAlgorithmV1Tests;
using Assert = NUnit.Framework.Assert;

namespace GripOpGras2.ClientTests.Features.CreateRation
{
	public class RationTests
	{
		[Test()]
		[TestCase(1000, 500, 500, 5000, 150, 200, 5500, 5000, 10.5f, 10, 500)]
		public void TestTotalCalculations(float vem, float vemSupplementaryFeedProduct, float appliedVem, float appliedVeMbijpord,
			float re, float reSupplementaryFeedProduct, float expectedVem, float expectedVemSupplementaryFeedProduct,
			float expectedDm, float expectedDmSupplementaryFeedProduct, float expectedRediff)
		{
			Ration ration = new();
			ration.ApplyChangesToRationList(GetMappedFeedProduct("product 1", re, vem, appliedVem));
			ration.ApplyChangesToRationList(GetMappedFeedProduct("product 2", reSupplementaryFeedProduct, vemSupplementaryFeedProduct,
				appliedVeMbijpord, false));

			Console.WriteLine(ration.RationList[0].AppliedKgdm);
			Console.WriteLine(ration.RationList[1].AppliedKgdm);
			Assert.AreEqual(expectedVem, ration.TotalVem, "total vem");
			Assert.AreEqual(expectedVemSupplementaryFeedProduct, ration.TotalVemSupplementaryFeedProduct, "total vem SupplementaryFeedProduct");
			Assert.AreEqual(expectedDm, ration.TotalDm, "total dm");
			Assert.AreEqual(expectedDmSupplementaryFeedProduct, ration.TotalDmSupplementaryFeedProduct, "total dm SupplementaryFeedProduct");
			Assert.AreEqual(expectedRediff, ration.TotalReDiff, "total rediff");
		}

		[Test()]
		[TestCase(1000, 500, 500, 150, 200, 500, 250, 0.75f, 0.5f, 25)]
		public void TestAllCalculationsWithCombos(float vem, float vemSupplementaryFeedProduct, float appliedVem, float re,
			float reSupplementaryFeedProduct, float expectedVem, float expectedVemSupplementaryFeedProduct, float expectedDm, float expectedDmSupplementaryFeedProduct,
			float expectedRediff)
		{
			Ration ration = new();
			MappedFeedProduct mappeditem1 = GetMappedFeedProduct("product 1", re, vem, 5);
			MappedFeedProduct mappeditem2 = GetMappedFeedProduct("product 2", reSupplementaryFeedProduct, vemSupplementaryFeedProduct, 5, false);
			MappedFeedProductGroup mappedcombo = new((mappeditem1, 0.5f), (mappeditem2, 0.5f));
			mappedcombo.SetAppliedVem(appliedVem);
			ration.ApplyChangesToRationList(mappedcombo);

			Assert.AreEqual(expectedVem, ration.TotalVem);
			Assert.AreEqual(expectedVemSupplementaryFeedProduct, ration.TotalVemSupplementaryFeedProduct);
			Assert.AreEqual(expectedDm, ration.TotalDm);
			Assert.AreEqual(expectedDmSupplementaryFeedProduct, ration.TotalDmSupplementaryFeedProduct);
			Assert.AreEqual(expectedRediff, ration.TotalReDiff);
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
			Assert.AreSame(ration.OriginalRefference, clone.OriginalRefference);
			Assert.AreNotSame(ration, clone);
			Assert.AreSame(item1.OriginalReference, clone.RationList[0].OriginalReference);

			Assert.AreNotEqual(ration.TotalVem, clone.TotalVem);
			Assert.AreNotEqual(ration.TotalDm, clone.TotalDm);
		}

		[Test()]
		[TestCase(1, 1800, 30, 1200, 1, 800, 70, 1000, 1800 + 1200 + 800 + 70000)]
		[TestCase(800, 0, 30, 700, 50, 0, 70, 1000, 700 + 70000)]
		public void GetTotalVemTest(float vemPerDMprod1, float veMprod1, float vemPerDMprod2, float veMprod2,
			float vemPerDmBijvProd, float vemBijvoerProd, float kGgrass, float veMgrass, float expected)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", 189f, vemPerDMprod1);
			FeedProduct prod2 = GetFeedProduct("prod2", 88f, vemPerDMprod2);
			FeedProduct bijvprod = GetFeedProduct("bijvprod", 189f, vemPerDmBijvProd, false);
			MappedFeedProduct mappedProd1 = new(prod1);
			MappedFeedProduct mappedProd2 = new(prod2);
			MappedFeedProduct mappedBijvprod = new(bijvprod);
			mappedProd1.SetAppliedVem(veMprod1);
			mappedProd2.SetAppliedVem(veMprod2);
			mappedBijvprod.SetAppliedVem(vemBijvoerProd);
			RationAlgorithmV1 rationAlgorithm =
				CreateRationAlgorithm(new List<FeedProduct>(), kGgrass, plotVem: veMgrass);
			rationAlgorithm.CurrentRation.RationList =
				new List<AbstractMappedFoodItem> { mappedProd1, mappedProd2, mappedBijvprod };
			//Act
			float totalVem = rationAlgorithm.CurrentRation.TotalVem;
			//Assert
			Assert.AreEqual(expected, totalVem);
		}

		[Test]
		[TestCase(100, 200, 300, 400, 30, 1000, 500, 600, 301600)]
		[TestCase(100, 200, 300, 400, 30, 0, 500, 600, 300600f)]
		[TestCase(300, 800, 500, 0, 0, 1000, 0, 80, 1800)]
		public void GetTotalVemTestWithCombos(float vemPerDMprod1, float veMprod1, float vemPerDMprod2, float veMprod2,
			float percentProd1, float veMofGroup, float kGgrass, float veMgrass, float expectedVem)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", 189f, vemPerDMprod1);
			FeedProduct prod2 = GetFeedProduct("prod2", 88f, vemPerDMprod2);
			MappedFeedProduct mappedProd1 = new(prod1);
			MappedFeedProduct mappedProd2 = new(prod2);
			mappedProd1.SetAppliedVem(veMprod1);
			mappedProd2.SetAppliedVem(veMprod2);

			MappedFeedProductGroup mappedgroup = new(
				(mappedProd1, percentProd1),
				(mappedProd2, 100 - percentProd1));
			mappedgroup.SetAppliedVem(veMofGroup);
			RationAlgorithmV1 rationAlgorithm =
				CreateRationAlgorithm(new List<FeedProduct>(), kGgrass, plotVem: veMgrass);
			rationAlgorithm.CurrentRation.RationList = new List<AbstractMappedFoodItem>
				{ mappedProd1, mappedProd2, mappedgroup };
			//Act
			float totalVem = rationAlgorithm.CurrentRation.TotalVem;
			//Assert
			Assert.AreEqual(expectedVem, totalVem);
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
		public async Task CreateRationAsyncTest(float prod1Re, float prod1Vem, float prod2Re, float prod2Vem,
			float supplementaryFeedProductRe, float supplementaryFeedProductVem, float totalgrassIntake, float grassVem, float grassRe)
		{
			// Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", prod1Re, prod1Vem);
			FeedProduct prod2 = GetFeedProduct("prod2", prod2Re, prod2Vem);
			FeedProduct prod3 = GetFeedProduct("bijprod", supplementaryFeedProductRe, supplementaryFeedProductVem, false);
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
						Vem = grassVem,
						Re = grassRe,
					}
				}
			};
			Client.Features.CreateRation.RationAlgorithmV1 rationAlgorithm = new();
			List<FeedProduct> feedProducts = new();
			if (prod1.FeedAnalysis.Vem != 0) feedProducts.Add(prod1);
			if (prod2.FeedAnalysis.Vem != 0) feedProducts.Add(prod2);
			if (prod3.FeedAnalysis.Vem != 0) feedProducts.Add(prod3);
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
		public void ProofOfConceptDataTest()
		{
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
					Area = 1000,
					Name = "TestcaseGrass",
					NetDryMatter = 1062.5f * 1.1f,
					FeedAnalysis = new FeedAnalysis()
					{
						Vem = 1000,
						Re = 210,
					}
				}
			};
			FeedProduct product1 = GetFeedProduct("prod1", 160, 920);
			FeedProduct product2 = GetFeedProduct("prod2", 60, 960);
			FeedProduct product3 = GetFeedProduct("prod3", 84, 900);
			FeedProduct productvem = GetFeedProduct("bijprod(vem)", 88,1264 ,false);
			FeedProduct productRe = GetFeedProduct("bijprod(RE)", 242,942,false);
			RationAlgorithmV1 rationAlgorithm = new RationAlgorithmV1();
			//act
			FeedRation ration = rationAlgorithm.CreateRationAsync(
				new[] { product1, product2, product3, productvem, productRe }, herd, 1062.5f, milkProductionAnalysis,
				grazingActivity).Result;
			//assert
			Assert.LessOrEqual(20, ration.FeedProducts.Sum(x => x.Value));
			Assert.True(true);
		}


		[Test()]
		[TestCase(30, 30, 1800.5f, 1860.5f)]
		[TestCase(0, 0, 1800.5f, 1800.5f)]
		[TestCase(0f, 0f, 0f, 0f)]
		[TestCase(30.5f, 15f, 0f, 45.5f)]
		public void GetTotalKgdmTest(float kGprod1, float kGprod2, float kGgrass, float expected)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", 189f, 884.2f);
			FeedProduct prod2 = GetFeedProduct("prod2", 88f, 1238f);
			MappedFeedProduct mappedProd1 = new(prod1);
			MappedFeedProduct mappedProd2 = new(prod2);
			mappedProd1.SetAppliedVem(kGprod1 * 884.2f);
			mappedProd2.SetAppliedVem(kGprod2 * 1238f);
			RationAlgorithmV1 rationAlgorithm =
				CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2 }, kGgrass);
			rationAlgorithm.CurrentRation.ApplyChangesToRationList(new List<AbstractMappedFoodItem>()
				{ mappedProd1, mappedProd2 });

			//Act
			float totalKgdm = rationAlgorithm.CurrentRation.TotalDm;
			//Assert
			Assert.AreEqual(expected, totalKgdm);
		}


		[Test()]
		[TestCase(140, 1800, 140, 1200, 150, 1200, 200, "prod1")] //prod 1 heeft meer VEM
		[TestCase(170, 1800, 160, 1200, 140, 1200, 50, "prod1")] // prod 1 heeft een groter RE difference
		[TestCase(170, 1800, 160, 1200, 140, 1200, 180,
			"bijprod")] //prod 1 en 2 hebben allebei net als het gras een overschot. #Taiga issue: #199
		[TestCase(170, 1800, 149, 1200, 100, 1200, 200, "prod2")] //ruwvoer gaat altijd voor bijvoer producten.
		[TestCase(170, 1800, 150, 1200, 100, 1200, 150, "prod1")]
		[TestCase(160, 1800, 170, 1800, 100, 1200, 130, "prod2")]
		public void GetGrassReNuturalizerFeedProductTest(float rEperDMprod1, float vemPerDMprod1, float rEperDMprod2,
			float vemPerDMprod2, float rEperDmSupplementaryFeedProduct, float vemPerDmSupplementaryFeedProduct, float rEgrass, string expectedProd)
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod1", rEperDMprod1, vemPerDMprod1);
			FeedProduct prod2 = GetFeedProduct("prod2", rEperDMprod2, vemPerDMprod2);
			FeedProduct supplementaryFeedProduct = GetFeedProduct("bijprod", rEperDmSupplementaryFeedProduct, vemPerDmSupplementaryFeedProduct, false);
			RationAlgorithmV1 rationAlgorithm =
				CreateRationAlgorithm(new List<FeedProduct> { prod1, prod2, supplementaryFeedProduct }, totalGrassIntake: 700,
					plotVem: 1000, plotRe: rEgrass);
			//Act
			List<AbstractMappedFoodItem> foodlist = rationAlgorithm.GetGrassReNeutralizerFeedProduct();
			rationAlgorithm.CurrentRation.ApplyChangesToRationList(foodlist);
			//Assert
			Assert.AreEqual(1, foodlist.Count);
			Assert.AreEqual(expectedProd, foodlist[0].GetProducts().First().Key.Name);
			Assert.AreEqual(150,
				rationAlgorithm.CurrentRation.TotalRe / rationAlgorithm.CurrentRation.TotalDm, 1);
		}

		[Test()]
		public void ExceptionByGetGrassReNuturalizerFeedProductTest()
		{
			FeedProduct prod1 = GetFeedProduct("prod 1", 160, 800);
			FeedProduct prod2 = GetFeedProduct("prod 2", 190, 11000);
			RationAlgorithmV1 ration = CreateRationAlgorithm(new List<FeedProduct>() { prod1, prod2 },
				plotRe: 200, plotVem: 1000, totalGrassIntake: 500);
			Assert.Throws<NoProductsWithPossibleReException>(() => ration.GetGrassReNeutralizerFeedProduct());
		}

		[Test()]
		public void GenerateReNaturalFeedProductGroupsTest()
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
			List<AbstractMappedFoodItem> groups = ration.GenerateReNaturalFeedProductGroups();
			//Assert
			Assert.AreEqual(3, groups.Count,
				"amount of expected groups"); //this can be more when more Supplementary datapoints are added.
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
			List<AbstractMappedFoodItem> groups = ration.GenerateReNaturalFeedProductGroups();
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
		public void FindBestReNaturalFeedProductGroupTest(bool findREnaturalFeedproductGroup, string expectedproduct1,
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
			List<AbstractMappedFoodItem> groups = ration.GenerateReNaturalFeedProductGroups();
			ration.AvailableReNaturalFeedProductGroups = groups;
			//Act
			AbstractMappedFoodItem group = ration.FindBestReNaturalFeedProductGroup(findREnaturalFeedproductGroup);
			//Assert
			Assert.Contains(expectedproduct1, group.GetProducts().Select(x => x.Key.Name).ToList(),
				$"naturalFeedproductgroup: {findREnaturalFeedproductGroup}; prod: {expectedproduct1}");
			Assert.Contains(expectedproduct2, group.GetProducts().Select(x => x.Key.Name).ToList(),
				$"naturalFeedproductgroup: {findREnaturalFeedproductGroup}; prod: {expectedproduct1}");
		}

		//TODO: test for determine improvemend rations with bijprod test
		[Test()]
		public void DetermineImprovemendRationsWithSupplementaryFeedProductTest()
		{
			//Arrange
			FeedProduct prod1 = GetFeedProduct("prod 1", 90, 50);
			FeedProduct prod2 = GetFeedProduct("prod 2", 160, 200);
			FeedProduct prod3 = GetFeedProduct("prod 3", 180, 200);
			FeedProduct prod4 = GetFeedProduct("prod 4", 140, 400, false);
			//make a rationAlgorithm
			RationAlgorithmV1 ration = CreateRationAlgorithm(new List<FeedProduct>()
				{ prod1, prod2, prod3, prod4 });
			List<AbstractMappedFoodItem> groups = ration.GenerateReNaturalFeedProductGroups();
			ration.AvailableReNaturalFeedProductGroups = groups;
			//Act
			AbstractMappedFoodItem group = ration.FindBestReNaturalFeedProductGroup(false);
			AbstractMappedFoodItem group2 = ration.FindBestReNaturalFeedProductGroup(true);
			//Assert
			Assert.Contains(prod1.Name, group.GetProducts().Select(x => x.Key.Name).ToList());
			Assert.Contains(prod2.Name, group.GetProducts().Select(x => x.Key.Name).ToList());
			Assert.Contains(prod4.Name, group2.GetProducts().Select(x => x.Key.Name).ToList());
			Assert.Contains(prod2.Name, group2.GetProducts().Select(x => x.Key.Name).ToList());
		}

		[Test()]
		public void FindImprovementRationMethodGrassReNuterilizerTest()
		{
			Assert.Fail();
		}

		[Test()]
		public void FindImprovementRationMethodNaturalReGroupsTest()
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