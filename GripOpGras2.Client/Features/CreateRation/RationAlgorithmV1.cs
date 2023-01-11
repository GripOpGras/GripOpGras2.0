using GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions;
using GripOpGras2.Client.Features.CreateRation.ImprovementMethods;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;
using NUnit.Framework;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class TargetValues
	{
		/// <summary>
		/// The amount of VEM that a cow needs a day, if the cow doesn't product any milk
		/// </summary>
		public const float DefaultVEMNeedsOfCow = 5500;

		/// <summary>
		/// The amount of VEM that needs to be added per L Meetmilk produced.
		/// </summary>
		public const float VemNeedsPerLiterMilk = 450;
		/// <summary>
		/// The amount of KG DM Supplementery Feed Products that should be given per cow per day.
		/// </summary>
		public const float MaxAmountOfSupplementaryFeedProductInKGPerCow = 4.5f;
		
		/// <summary>
		/// The maximum amount of KG DM Feed products (inc grass) that a cow should be able to eat a day. 
		/// #TODO make this more dynamic. This should be an absolute maximum and in the feature be dynamic, based on the data of the cows, like their body weight. Taiga: #198
		/// </summary>
		public const float MaxKgDmIntakePerCow = 20;

		/// <summary>
		/// The VEM should be between 100%-110% of what the cows should actually eat. 105% is in between of that
		/// </summary>
		public const float OptimalVEMCoverage = 1.05f;

		/// <summary>
		/// The RE has an optimum of 150 RE per KG DM
		/// </summary>
		public const float OptimalRECoverageInGramsPerKgDm = 150;

		private Herd _herd;

		private MilkProductionAnalysis _milkProductionAnalysis;

		/// <summary>
		/// When the RE needs to differ fromt e optimum, the max RE/KGDM shoud be 170.
		/// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		/// </summary>
		public const float MaxRECoverageInGramsPerKgDm = 170;

		/// <summary>
		/// When the RE needs to differ from te optimum, the  RE/KGDM shoud not go below 140 .
		/// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		/// </summary>
		public const float MinRECoverageInGramsPerKgDm = 140;

		/// <summary>
		/// A class that's a centeral place while making the Rationalgorithm to refer to when values are needed that determine how much nutrients should be applied.
		/// </summary>
		/// <param name="herd">Herd class required for the amount of cows</param>
		/// <param name="milkProductionAnalysis">milkProductionAnalysis required to determine the VEM needed</param>
		/// <param name="targetedREcoveragePerKgDm">optional. how much RE should be applied per VEM (can differ while the algorithm is running)</param>
		/// <param name="targetedMaxAmountOfSupplementeryFeedProductInKgPerCow">optional. max KGDM supplementery feedproduct per cow.</param>
		/// <param name="targetedMaxKgDmIntakePerCow">optional. set the max KG DM intake per cow by hand, for testing purposes.</param>
		public TargetValues(Herd herd,
			MilkProductionAnalysis milkProductionAnalysis,
			float targetedREcoveragePerKgDm = OptimalRECoverageInGramsPerKgDm,
			float targetedMaxAmountOfSupplementeryFeedProductInKgPerCow = MaxAmountOfSupplementaryFeedProductInKGPerCow,
			float targetedMaxKgDmIntakePerCow = MaxKgDmIntakePerCow)
		{
			_herd = herd;
			_milkProductionAnalysis = milkProductionAnalysis;
			TargetedREcoveragePerKgDm = targetedREcoveragePerKgDm;
			TargetedMaxKgDmSupplementeryFeedProductPerCow = targetedMaxAmountOfSupplementeryFeedProductInKgPerCow;
			TargetedMaxKgDmIntakePerCow = targetedMaxKgDmIntakePerCow;
			TargetedVEM = (_milkProductionAnalysis.Amount * VemNeedsPerLiterMilk + DefaultVEMNeedsOfCow*herd.NumberOfAnimals) * OptimalVEMCoverage;
		}

		public float TargetedREcoveragePerKgDm { get; set; } = OptimalRECoverageInGramsPerKgDm;

		public float TargetedMaxKgDmSupplementeryFeedProductPerCow { get; set; } =
			MaxAmountOfSupplementaryFeedProductInKGPerCow;

		public float TargetedMaxKgDmIntakePerCow { get; set; } = MaxKgDmIntakePerCow;

		public float TargetedVEMPerCow => TargetedVEM / _herd.NumberOfAnimals;
		public float TargetedMaxKgDm => TargetedMaxKgDmIntakePerCow*_herd.NumberOfAnimals;
		public float TargetedMaxKgDmSupplementeryFeedProduct => TargetedMaxKgDmSupplementeryFeedProductPerCow * _herd.NumberOfAnimals;

		public float TargetedVEM { get; set; }

	}

	public class RationAlgorithmV1 : IRationAlgorithm
	{
		public TargetValues targetValues;
		
		/// <summary>
		/// Available feedproducts givven, converted to AbstractMappedoodItem (but will only contain MappedFeedProduct)
		/// </summary>
		protected IReadOnlyList<AbstractMappedFoodItem> availableFeedProducts = new List<AbstractMappedFoodItem>();

		/// <summary>
		/// List of all the generated RE natural Feed product groups. This should be combinations of feedproducts, so that the RE will be the same as the targeted RE per KGDM.
		/// </summary>
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

		/// <summary>
		/// _currentRation that can be called to apply changes.  
		/// </summary>
		protected Ration _currentRation = new();

		public async Task<FeedRation> CreateRationAsync(IReadOnlyList<FeedProduct> feedProducts, Herd herd,
			float totalGrassIntake,
			MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			SetUp(feedProducts, herd, totalGrassIntake, milkProductionAnalysis, grazingActivity);
			RunAlgorithm();

			FeedRation feedRation = new FeedRation()
			{
				Plot = grazingActivity?.Plot,
				Date = DateTime.Now,
				FeedProducts = _currentRation.getFeedProducts(),
				GrassIntake = totalGrassIntake,
				Herd = herd
			};
			return feedRation;
		}

		public void SetUp(IReadOnlyList<FeedProduct> feedProducts, Herd herd, float totalGrassIntake,
			MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			targetValues = new TargetValues(herd, milkProductionAnalysis);
			//check if the nessesary values are set. TODO: make use of standard values if FeedAnalysis is not set.
			if (grazingActivity != null && grazingActivity.Plot != null & grazingActivity.Plot.FeedAnalysis != null)
			{
				_currentRation = new Ration(grassIntake: totalGrassIntake, grassAnalysis: grazingActivity.Plot.FeedAnalysis);

			}
			//add products
			List<AbstractMappedFoodItem> availableFeedProductsList = new();
			foreach (FeedProduct x in feedProducts)
			{
				MappedFeedProduct mappedFeedProduct = new MappedFeedProduct(x, targetValues.TargetedREcoveragePerKgDm);
				availableFeedProductsList.Add(mappedFeedProduct);
			}

			availableFeedProducts = availableFeedProductsList;
			;
			_improvementSelector.Initialize(currentRation: ref _currentRation,
				targetValues: ref targetValues,
				availableFeedProducts: ref availableFeedProducts,
				availableRENaturalFeedProductGroups: ref availableRENaturalFeedProductGroups);
			//TODO: set the needs of cows based on the milk production analysis and Herd. (fill TargetValues)
			//TODO: Fill the availableFeedProducts with the available feedProducts, converted to MappedFeedProduct.
			return;
		}

		public void RunAlgorithm()
		{
			if (availableFeedProducts.Count == 0)
			{
				Console.WriteLine("no products available, return Feedproducts without products");
				return;
			};
			//fill ration with feed product to get RE on the targeted level.
			_currentRation.ApplyChangesToRationList(GetGrassRENuturalizerFeedProduct());
			//generate RE natural FeedProductGroups #TODO change targetRE when needed: taiga #193
			availableRENaturalFeedProductGroups = GenerateRENaturalFeedProductGroups();
			//fill ration with best FeedProductGroups untill the VEM is on the targeted level, only containing roughage products.
			AbstractMappedFoodItem bestREnaturalFeedProductGroup = FindBestRENaturalFeedProductGroup(false);
			bestREnaturalFeedProductGroup.setAppliedVEM(targetValues.TargetedVEM-currentRation.totalVEM);
			Console.WriteLine($"Applying REnaturalFedproductgroep. amount: {bestREnaturalFeedProductGroup.appliedVEM}");
			_currentRation.ApplyChangesToRationList(bestREnaturalFeedProductGroup);
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
			bool inlinewithtargets = true;
			Console.WriteLine("Checking if ration is in line with targetValues");
			if (Math.Abs(currentRation.totalRE/currentRation.totalDM - targetValues.TargetedREcoveragePerKgDm) > 2)
			{
				inlinewithtargets = false;
				Console.WriteLine($"\t- RE is not in line with target, ration RE/DM:\t\t{currentRation.totalRE/currentRation.totalDM},\t\ttarget RE/DM:\t{targetValues.TargetedREcoveragePerKgDm}");
			}
			if (Math.Abs(currentRation.totalVEM - targetValues.TargetedVEM) > 5)
			{
				inlinewithtargets = false;
				Console.WriteLine($"\t- VEM is not in line with target, ration VEM:\t\t{currentRation.totalVEM},\ttarget VEM:\t\t{targetValues.TargetedVEM}");
			}
			if (currentRation.totalDM > targetValues.TargetedMaxKgDm)
			{
				inlinewithtargets = false;
				Console.WriteLine($"\t- DM not in line with target, ration KGDM:\t\t\t{currentRation.totalDM},\t\ttarget max DM:\t{targetValues.TargetedMaxKgDm}");
			}
			if (currentRation.totalDM_Bijprod > targetValues.TargetedMaxKgDmSupplementeryFeedProduct)
			 {
				 inlinewithtargets = false;
				 Console.WriteLine($"\t- DM bijprod not in line with targt, ration KGDMbijprod:{currentRation.totalDM_Bijprod},\t\ttarget max DM: \t{targetValues.TargetedMaxKgDmSupplementeryFeedProduct}");
			 }
			 if (inlinewithtargets) Console.WriteLine($"\t- ration in line with targets");
			 return inlinewithtargets;
		}
		//TODO: make this method more efficient.; taiga issue #192
		public List<AbstractMappedFoodItem> GetGrassRENuturalizerFeedProduct(
			bool allowSupplementeryFeedProducts = false)
		{
			if (currentRation.totalREdiff == 0) return new List<AbstractMappedFoodItem>();

			bool grassHasPositiveREdiff = (currentRation.totalREdiff > 0);
			var products = (allowSupplementeryFeedProducts)
				? availableFeedProducts
				: availableFeedProducts.Where(x => x.partOfTotalVEMbijprod == 0);

			//Check if products are availlable
			if (products.Count() == 0 && allowSupplementeryFeedProducts == true)
			{
				Console.WriteLine("GetGrassRENuturalizerFeedProduct: No feedproducts without supplementeries available, trying again with supplementeries");
				return GetGrassRENuturalizerFeedProduct(true);
			}

			if (products.Count() == 0) throw new RationAlgorithmException("No roughage feed products available.");

			//select the best product
			var sortedProducts = products.OrderBy(x => x.REdiffPerVEM/x.KGDMperVEM);
			AbstractMappedFoodItem bestproduct = (grassHasPositiveREdiff) ? sortedProducts.First() : sortedProducts.Last();
			float vemNeeded = -currentRation.totalREdiff / bestproduct.REdiffPerVEM;

			//check if product has the right REdiff
			if (grassHasPositiveREdiff && bestproduct.REdiffPerVEM > 0)
				throw new NoProductsWithPossibleREException("Products are missing to lower the REdiff");
			if (!grassHasPositiveREdiff && bestproduct.REdiffPerVEM < 0)
				throw new NoProductsWithPossibleREException("Products are missing to raise the REdiff");
			//TODO: check if supplementeries don't have too much KG DS when using them, and throw exception if they do; taiga issue 192
			AbstractMappedFoodItem productclone = bestproduct.Clone();
			productclone.setAppliedVEM(vemNeeded);
			Console.WriteLine($"GrassNeuturilizer: totalREdiff = {currentRation.totalREdiff}, bestproduct.REdiffPerVEM = {bestproduct.REdiffPerVEM}, vemNeeded = {vemNeeded}. product contains now: {(productclone.REdiffPerVEM+1500)*productclone.appliedVEM} RE total and {productclone.appliedVEM} VEM total and {productclone.appliedREdiff.ToString()} RE difference of the target");
			Console.WriteLine($"GrassNeuturilizer: used product: {bestproduct.GetProductsForConsole()}");
			Console.WriteLine($"VEM needed after GrassRENuturalizer: {targetValues.TargetedVEM - (currentRation.totalVEM+vemNeeded)}");
			return new List<AbstractMappedFoodItem> { productclone };
		}



		public List<AbstractMappedFoodItem> GenerateRENaturalFeedProductGroups()
		{
			List<AbstractMappedFoodItem> naturalFeedProductGroups = new();
			//add products that are allready natural as a group with 1 product.
			naturalFeedProductGroups.AddRange(availableFeedProducts.Where(x => x.REdiffPerVEM == 0).Select(x => new MappedFeedProductGroup((x, 1f))).ToList());
			foreach (AbstractMappedFoodItem product in availableFeedProducts.Where(x => x.REdiffPerVEM < 0))
			{
				foreach (AbstractMappedFoodItem? product2 in availableFeedProducts.Where(x => x.REdiffPerVEM > 0))
				{
					float prod2PerProd1 = product.REdiffPerVEM/-product2.REdiffPerVEM;
					naturalFeedProductGroups.Add(new MappedFeedProductGroup((product, 1f), (product2, prod2PerProd1)));
					Console.WriteLine($"group created, product 1 RE/vem: {product.REdiffPerVEM}, product 2 RE/vem: {product2.REdiffPerVEM}, product 2 per product 1 in VEM: {prod2PerProd1}. Part supplementery prod 1: {product.partOfTotalVEMbijprod}, prod2: {product2.partOfTotalVEMbijprod}");
				}
			}
			if (naturalFeedProductGroups.Count == 0) throw new NoPossibleRENaturalProductGroupsException(targetValues.TargetedREcoveragePerKgDm.ToString());
			return naturalFeedProductGroups;
		}

		//Returns the RE natural feed product group with the least KG DM per VEM.
		public AbstractMappedFoodItem FindBestRENaturalFeedProductGroup(bool supplementeryFeedProductAllowed)
		{
			IEnumerable<AbstractMappedFoodItem> availablegroups = availableRENaturalFeedProductGroups.Where(x => x.partOfTotalVEMbijprod == 0 || (supplementeryFeedProductAllowed));
			if (availablegroups.Count() == 0 && !supplementeryFeedProductAllowed)
			{
				Console.WriteLine("No RE natural feed product groups available of only roughages. Switching to supplementeryFeedProductAllowed == true");
				return FindBestRENaturalFeedProductGroup(true);
			}

			AbstractMappedFoodItem bestgroup = availablegroups.OrderBy(x => x.KGDMperVEM).First();
			Console.WriteLine($"FindBestReNaturalFeedProductGroup: supplementeryallowed = {supplementeryFeedProductAllowed}, partsupplementery: {bestgroup.partOfTotalVEMbijprod}, DM per VEM: {bestgroup.KGDMperVEM}, RE per VEM: {bestgroup.REperVEM}");
			return bestgroup;

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