using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class RationAlgorithmV2 : IRationAlgorithm
	{
		public const float DefaultVEMNeedsOfCow = 5500;

		public const float VemNeedsPerLiterMilk = 450;

		public const float MaxAmountOfSupplementaryFeedProductInKGPerCow = 4.5f;

		private const float MaxKgDmIntakePerCow = 18;

		private const float OptimalVEMCoverage = 1.05f;

		private const float OptimalRECoverageInGramsPerKgDm = 150;
		
		// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		private const float MaxRECoverageInGramsPerKgDm = 170;

		// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		private const float MinRECoverageInGramsPerKgDm = 140;

		protected float TargetedREcoveragePerKgDm { get; set; } = OptimalRECoverageInGramsPerKgDm;
		protected float TargetedMaxAmountOfSupplementeryFeedProductInKGPerCow { get; set; } = MaxAmountOfSupplementaryFeedProductInKGPerCow;
		protected float TargetedMaxKgDmIntakePerCow { get; set; } = MaxKgDmIntakePerCow;
		protected float TargetedVEMCoverage { get; set; } = OptimalVEMCoverage;




		protected List<AbstractMappedFoodItem> availableFeedProducts = new();
		protected List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups = new();

		//private List<AbstractMappedFoodItem> GrassRENuturalizerFeedProducts = new();
		protected List<AbstractMappedFoodItem> REFoodItens = new();
		protected List<AbstractMappedFoodItem> roughages = new();


		public Task<FeedRation> CreateRationAsync(IReadOnlyList<FeedProduct> feedProducts, Herd herd, float totalGrassIntake,
			MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			throw new NotImplementedException();
		}

		public float GetTotalKGDM()
		{
			throw new NotImplementedException();
		}

		public float GetTotalVEM()
		{
			throw new NotImplementedException();}

		public List<AbstractMappedFoodItem> GetGrassRENuturalizerFeedProduct()
		{
			throw new NotImplementedException();
		}

		public List<AbstractMappedFoodItem> GenerateRENaturalFeedProductGroups()
		{
			throw new NotImplementedException();
		}

		public AbstractMappedFoodItem GetBestRENaturalFeedProductGroup()
		{
			throw new NotImplementedException();
		}

		public List<AbstractMappedFoodItem> DetermineImprovemendRationsWithBijprod()
		{
			throw new NotImplementedException();
		}
		
		public List<AbstractMappedFoodItem> FindImprovementRationMethodGrassRENuterilizer()
		{
			throw new NotImplementedException();
		}

		public List<AbstractMappedFoodItem> FindImprovementRationMethodNaturalREGroups()
		{
			throw new NotImplementedException();
		}

		public List<AbstractMappedFoodItem> ImprovementRationMethodChangeTargetedCoverages()
		{
			throw new NotImplementedException();
		}







	}
}