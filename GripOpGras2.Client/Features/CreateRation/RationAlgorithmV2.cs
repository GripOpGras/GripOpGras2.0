using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;
using NUnit.Framework;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class TargetValues
	{

		public const float DefaultVEMNeedsOfCow = 5500;

		public const float VemNeedsPerLiterMilk = 450;

		public const float MaxAmountOfSupplementaryFeedProductInKGPerCow = 4.5f;

		public const float MaxKgDmIntakePerCow = 18;

		public const float OptimalVEMCoverage = 1.05f;

		public const float OptimalRECoverageInGramsPerKgDm = 150;

		// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		public const float MaxRECoverageInGramsPerKgDm = 170;

		// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		public const float MinRECoverageInGramsPerKgDm = 140;

		public float TargetedREcoveragePerKgDm { get; set; } = OptimalRECoverageInGramsPerKgDm;

		public float TargetedMaxAmountOfSupplementeryFeedProductInKGPerCow { get; set; } =
			MaxAmountOfSupplementaryFeedProductInKGPerCow;

		public float TargetedMaxKgDmIntakePerCow { get; set; } = MaxKgDmIntakePerCow;

		public float TargetedVEMCoverage { get; set; } = OptimalVEMCoverage;
	}

	public class RationAlgorithmV2 : IRationAlgorithm
	{
		public TargetValues targetValues = new();
		
		protected List<AbstractMappedFoodItem> availableFeedProducts = new();

		protected List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups = new();

		/// <summary>
		/// This property will be used to save and fill the ration with possible products. It has a .Clone get, to make sure the ration won't be changed by accident.
		/// </summary>
		public Ration currentRation
		{
			get => _currentRation.Clone();
			protected set => _currentRation = value;
		}

		/// <summary>
		/// The improvementSelector will be used to combine various ImprovementRapports to improve the ration based on various 
		/// </summary>
		private readonly IImprovementSelector _improvementSelector = new ImprovementSelectorV1();

		private Ration _currentRation = new();

		public Task<FeedRation> CreateRationAsync(IReadOnlyList<FeedProduct> feedProducts, Herd herd,
			float totalGrassIntake,
			MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			SetUp(feedProducts, herd, totalGrassIntake, milkProductionAnalysis, grazingActivity);
			RunAlgorithm();
			throw new NotImplementedException();
		}

		public AsyncTestDelegate SetUp(IReadOnlyList<FeedProduct> feedProducts, Herd herd, float totalGrassIntake,
			MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			throw new NotImplementedException();
		}

		public void RunAlgorithm()
		{
			throw new NotImplementedException();
		}
		public void GetCurrentFeedRation()
		{
			throw new NotImplementedException();
		}

		public float GetTotalKGDM()
		{
			throw new NotImplementedException();
		}

		public float GetTotalVEM()
		{
			throw new NotImplementedException();
		}

		public List<AbstractMappedFoodItem> GetGrassRENuturalizerFeedProduct()
		{
			throw new NotImplementedException();
		}

		public List<AbstractMappedFoodItem> GenerateRENaturalFeedProductGroups()
		{
			throw new NotImplementedException();
		}

		public AbstractMappedFoodItem FindBestRENaturalFeedProductGroup(bool supplementeryFeedProductAllowed)
		{
			throw new NotImplementedException();
		}

	};

	public class ImprovementRapport
	{
		public List<AbstractMappedFoodItem> changesPerVEM = new List<AbstractMappedFoodItem>();

		public List<AbstractMappedFoodItem> changesPerKGBijprod = new List<AbstractMappedFoodItem>();



		public ImprovementRapport(List<AbstractMappedFoodItem> changesPerVEM)
		{
			this.changesPerVEM = changesPerVEM.Select(x => x.Clone()).ToList();
			
			float totalKGBijprodperVem = changesPerVEM.Sum(x => x.appliedVEM * x.KGDMPerVEM_bijprod);
			//make a copy of the list and the items in the list
			changesPerKGBijprod = changesPerVEM.Select(x => x.Clone()).ToList();
			changesPerKGBijprod.ForEach(x => x.setAppliedVEM(x.appliedVEM / totalKGBijprodperVem)); ;
		}
	}
}