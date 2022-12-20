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
	[TestFixture]
	public class RationAlgorithmV2Tests
	{
		public FeedProduct GetFeedProduct(string name, float re, float vem, bool isRoughage = true)
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



		[SetUp]
		public RationAlgorithmV2 Setup(List<FeedProduct> feedProducts, float totalGrassIntake =  1062.5f, float lmilk = 3000)
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
			grazingActivity.Plot = new Plot();
			var rationAlgorithm = new RationAlgorithmV2();
			// Act
			rationAlgorithm.SetUp(feedProducts: feedProducts, herd: herd, totalGrassIntake: totalGrassIntake, milkProductionAnalysis: milkProductionAnalysis, grazingActivity: grazingActivity);
			return rationAlgorithm;
		}

		[Test()]
		public void CreateRationAsyncTest()
		{
			// Arrange
			
			Assert.Fail();
		}

		[Test()]
		public void GetTotalKGDMTest()
		{
			//Arrange
			var rationAlgorithm = new RationAlgorithmV2();
			var feedProducts = new List<FeedProduct>();
			
			Assert.Fail();
		}

		[Test()]
		public void GetTotalVEMTest()
		{
			Assert.Fail();
		}

		[Test()]
		public void GetGrassRENuturalizerFeedProductTest()
		{
			Assert.Fail();
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
    }
}