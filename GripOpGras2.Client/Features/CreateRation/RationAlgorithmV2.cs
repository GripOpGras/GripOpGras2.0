using GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions;
using GripOpGras2.Client.Features.CreateRation.ImprovementMethods;
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

		private Herd herd;

		// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		public const float MaxRECoverageInGramsPerKgDm = 170;

		// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		public const float MinRECoverageInGramsPerKgDm = 140;

		public TargetValues(Herd herd)
		{
			this.herd = herd;
		}

		public TargetValues(Herd herd, float targetedREcoveragePerKgDm, float targetedMaxAmountOfSupplementeryFeedProductInKgPerCow, float targetedMaxKgDm, float targetedMaxKgDmIntakePerCow, float targetedVemCoverage)
		{
			this.herd = herd;
			TargetedREcoveragePerKgDm = targetedREcoveragePerKgDm;
			TargetedMaxKgDmSupplementeryFeedProductPerCow = targetedMaxAmountOfSupplementeryFeedProductInKgPerCow;
			TargetedMaxKgDmIntakePerCow = targetedMaxKgDmIntakePerCow;
			TargetedVEMCoveragePerCow = targetedVemCoverage;
		}

		public float TargetedREcoveragePerKgDm { get; set; } = OptimalRECoverageInGramsPerKgDm;

		public float TargetedMaxKgDmSupplementeryFeedProductPerCow { get; set; } =
			MaxAmountOfSupplementaryFeedProductInKGPerCow;

		public float TargetedMaxKgDmIntakePerCow { get; set; } = MaxKgDmIntakePerCow;

		public float TargetedVEMCoveragePerCow { get; set; } = OptimalVEMCoverage;
		public float TargetedMaxKgDm => TargetedMaxKgDmIntakePerCow*herd.NumberOfAnimals;
		public float TargetedMaxKgDmSupplementeryFeedProduct => TargetedMaxKgDmSupplementeryFeedProductPerCow * herd.NumberOfAnimals;

		public float TargetedVEM => TargetedVEMCoveragePerCow * herd.NumberOfAnimals;

	}

	public class RationAlgorithmV2 : IRationAlgorithm
	{
		public TargetValues targetValues;
		
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

		private Ration _currentRation;

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
			//check if the nessesary values are set. TODO: make use of standard values if FeedAnalysis is not set.
			if (grazingActivity != null && grazingActivity.Plot != null & grazingActivity.Plot.FeedAnalysis != null)
			{
				_currentRation = new Ration(grassIntake: totalGrassIntake, grassAnalysis: grazingActivity.Plot.FeedAnalysis);
			}
			_improvementSelector.Initialize(currentRation: ref _currentRation,
				targetValues: ref targetValues,
				availableFeedProducts: ref availableFeedProducts,
				availableRENaturalFeedProductGroups: ref availableRENaturalFeedProductGroups);
			//TODO: set the needs of cows based on the milk production analysis and Herd. (fill TargetValues)
			//TODO: Fill the availableFeedProducts with the available feedProducts, converted to MappedFeedProduct.
			return null;
		}

		public void RunAlgorithm()
		{
			//fill ration with feed product to get RE on the targeted level.
			currentRation.ApplyChangesToRationList(GetGrassRENuturalizerFeedProduct());
			//generate RE natural FeedProductGroups
			GenerateRENaturalFeedProductGroups();
			//fill ration with best FeedProductGroups untill the VEM is on the targeted level, only containing roughage products.
			currentRation.ApplyChangesToRationList(FindBestRENaturalFeedProductGroup(false));
			//check if Ration is in line with target values, if not, improve the ration.
			if (checkIfRationIsInLineWithTargetValues()) return;
			//if not, improve the ration.
			IImprovementRationMethod[] improvementMethods = 
			{
				new ImprovementRationMethodGrassRENuterilizer(),
				new ImprovementRationMethodNaturalREGroups()
			};
			List<AbstractMappedFoodItem> improvementChanges = _improvementSelector.DetermineImprovemendRationsWithBijprod(
				improvementMethods: improvementMethods.ToArray());
			currentRation.ApplyChangesToRationList(improvementChanges);
			//check if Ration is in line with target values, if not, change Targetvalues.
			currentRation.ApplyChangesToRationList(_improvementSelector.DetermineImprovemendRationsWithBijprod(new ImprovementRationMethodChangeTargetedCoverages()));
		}

		private bool checkIfRationIsInLineWithTargetValues()
		{
			return (Math.Abs(currentRation.totalREdiff - targetValues.TargetedREcoveragePerKgDm) < 0.1 &&
			     Math.Abs(currentRation.totalVEM - targetValues.TargetedVEM) < 0.1 &&
			     currentRation.totalDM <= targetValues.TargetedMaxKgDm &&
			     currentRation.totalDM_Bijprod <= targetValues.TargetedMaxKgDmSupplementeryFeedProduct);
		}


		public List<AbstractMappedFoodItem> GetGrassRENuturalizerFeedProduct(
			bool allowSupplementeryFeedProducts = false)
		{
			if (currentRation.totalREdiff == 0) return new List<AbstractMappedFoodItem>();

			bool grassHasPositiveREdiff = (currentRation.totalREdiff > 0);
			var products = (grassHasPositiveREdiff)
				? availableFeedProducts
				: availableFeedProducts.Where(x => x.partOfTotalVEMbijprod == 0);
			AbstractMappedFoodItem bestproduct = (grassHasPositiveREdiff) ? products.First() : products.Last();
			float vemNeeded = bestproduct.REdiffPerVEM / -currentRation.totalREdiff;
			//if not possible, try with supplementeryFeedProducts.
			if (vemNeeded < 0 && !grassHasPositiveREdiff) return GetGrassRENuturalizerFeedProduct(true);
			if (grassHasPositiveREdiff && bestproduct.REdiffPerVEM > 0)
				throw new NoProductsWithPossibleREException("Products are missing to lower the REdiff");
			if (!grassHasPositiveREdiff && bestproduct.REdiffPerVEM < 0)
				throw new NoProductsWithPossibleREException("Products are missing to raise the REdiff");
			//TODO: check if supplementeries don't have too much KG DS when using them, and throw exception if they do.
			AbstractMappedFoodItem productclone = bestproduct.Clone();
			productclone.setAppliedVEM(vemNeeded);
			return new List<AbstractMappedFoodItem> { productclone };
		}



		public List<AbstractMappedFoodItem> GenerateRENaturalFeedProductGroups()
		{
			throw new NotImplementedException();
		}

		public List<AbstractMappedFoodItem> FindBestRENaturalFeedProductGroup(bool supplementeryFeedProductAllowed)
		{
			throw new NotImplementedException();
		}

	};

	public class ImprovementRapport
	{
		public List<AbstractMappedFoodItem> changesPerVEM = new List<AbstractMappedFoodItem>();

		public List<AbstractMappedFoodItem> changesPerKGBijprod = new List<AbstractMappedFoodItem>();

		//The amount of change in VEM that is required with this improvement method.
		public float changeInVEMrequired;
		/// <summary>
		/// The amount of change in VEM that can be made at max, untill there are no more products to change.
		/// For example, if the imporovement changes prod1 for bijprod 1, it can only change the amount of VEM that was applied on bijpord1.
		/// </summary>
		public float maxChangeinVEM;
		//Repersents the amount of KG bijprod that would be required to be added to make the targetedValues
		public float changeInKGBijprodRequired;

		private TargetValues _targetValues;



		public ImprovementRapport(List<AbstractMappedFoodItem> changesPerVEM, TargetValues targetValues)
		{
			this.changesPerVEM = changesPerVEM.Select(x => x.Clone()).ToList();
			this._targetValues = targetValues;

			float totalKGBijprodperVem = changesPerVEM.Sum(x => x.appliedVEM * x.KGDMPerVEM_bijprod);
			//make a copy of the list and the items in the list
			changesPerKGBijprod = changesPerVEM.Select(x => x.Clone()).ToList();
			changesPerKGBijprod.ForEach(x => x.setAppliedVEM(x.appliedVEM / totalKGBijprodperVem)); ;
		}
	}
}