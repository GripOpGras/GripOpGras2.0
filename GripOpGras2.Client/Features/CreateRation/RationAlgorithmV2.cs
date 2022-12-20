using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class RationAlgorithmV2 : IRationAlgorithm
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

		private List<AbstractMappedFoodItem> availableFeedProducts = new();
		private List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups = new();

		//private List<AbstractMappedFoodItem> GrassRENuturalizerFeedProducts = new();
		private List<AbstractMappedFoodItem> REFoodItens = new();
		private List<AbstractMappedFoodItem> roughages = new();


		public Task<FeedRation> CreateRationAsync(IReadOnlyList<FeedProduct> feedProducts, Herd herd, float totalGrassIntake,
			MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			throw new NotImplementedException();
		}

		protected float GetTotalKGDM()
		{
			throw new NotImplementedException();
		}

		protected float GetTotalVEM()
		{
			throw new NotImplementedException();}

		private void ApplyGrassRENuturalizerFeedProduct()
		{
			throw new NotImplementedException();
		}

		private void MakeRENaturalFeedProductGroups()
		{
			throw new NotImplementedException();
		}

		private void ApplyRENaturalFeedProductGroups()
		{
			throw new NotImplementedException();
		}

		private void MakeImprovementWithBijprod()
		{
			throw new NotImplementedException();
		}
		
		private List<AbstractMappedFoodItem> ImprovementRationMethodGrassRENuterilizer()
		{
			throw new NotImplementedException();
		}

		private List<AbstractMappedFoodItem> ImprovementRationMethodNaturalREGroups()
		{
			throw new NotImplementedException();
		}

		private List<AbstractMappedFoodItem> ImprovementRationMethodChangeoptimalcoverages()
		{
			throw new NotImplementedException();
		}







	}
}