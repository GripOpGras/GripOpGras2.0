using System.Globalization;
using GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions;
using GripOpGras2.Client.Features.CreateRation.ImprovementMethods;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class RationAlgorithmV1 : IRationAlgorithm
	{
		public TargetValues targetValues = null!;

		/// <summary>
		/// Available feedproducts givven, converted to AbstractMappedoodItem (but will only contain MappedFeedProduct)
		/// </summary>
		protected IReadOnlyList<AbstractMappedFoodItem> availableFeedProducts = new List<AbstractMappedFoodItem>();

		/// <summary>
		/// List of all the generated RE natural Feed product groups. This should be combinations of feedproducts, so that the RE will be the same as the targeted RE per KGDM.
		/// </summary>
		public List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups = new();

		/// <summary>
		/// This property will be used to save and fill the ration with possible products. It has a .Clone get, to make sure the ration won't be changed by accident.
		/// </summary>
		public Ration CurrentRation = null!;

		/// <summary>
		/// The improvementSelector will be used to combine various ImprovementRapports to improve the ration based on various 
		/// </summary>
		private readonly IImprovementSelector _improvementSelector = new ImprovementSelectorV1();


		public Task<FeedRation> CreateRationAsync(IReadOnlyList<FeedProduct> feedProducts, Herd herd,
			float totalGrassIntake,
			MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			SetUp(feedProducts, herd, totalGrassIntake, milkProductionAnalysis, grazingActivity);
			RunAlgorithm();

			FeedRation feedRation = new()
			{
				Plot = grazingActivity?.Plot,
				Date = DateTime.Now,
				FeedProducts = CurrentRation.getFeedProducts(),
				GrassIntake = totalGrassIntake,
				Herd = herd
			};
			Console.WriteLine("Results:");
			CurrentRation.printProducts();
			Console.WriteLine(
				$"{"VEM",20}: target: {targetValues.TargetedVEM,5:0.00} actual: {CurrentRation.totalVEM,5:0.00}");
			Console.WriteLine(
				$"{"RE",20}:  target: {targetValues.TargetedREcoveragePerKgDm,5:0.00} actual: {CurrentRation.totalRE / CurrentRation.totalDM,5:0.00}");
			Console.WriteLine(
				$"{"DM",20}:  target: {targetValues.TargetedMaxKgDm,5:0.00} actual: {CurrentRation.totalDM,5:0.00}");
			Console.WriteLine(
				$"{"DM supplementerys",20}:  target: {targetValues.TargetedMaxKgDmSupplementeryFeedProduct,5:0.00} actual: {CurrentRation.totalDM_Bijprod,5:0.00}");
			return Task.FromResult(feedRation);
		}

		public void SetUp(IReadOnlyList<FeedProduct> feedProducts, Herd herd, float totalGrassIntake,
			MilkProductionAnalysis milkProductionAnalysis, GrazingActivity? grazingActivity)
		{
			targetValues = new TargetValues(herd, milkProductionAnalysis);
			//check if the nessesary values are set. TODO: make use of standard values if FeedAnalysis is not set.
			if (grazingActivity?.Plot?.FeedAnalysis != null)
			{
				CurrentRation = new Ration(grassIntake: totalGrassIntake,
					grassAnalysis: grazingActivity.Plot.FeedAnalysis);
			}

			//add products
			List<AbstractMappedFoodItem> availableFeedProductsList = new();
			foreach (FeedProduct x in feedProducts)
			{
				if (x.FeedAnalysis == null)
					throw new RationAlgorithmException("FeedAnalysis is not set for all feedproducts");
				if (x.FeedAnalysis.VEM == 0) throw new RationAlgorithmException("VEM is not set for all feedproducts");
				MappedFeedProduct mappedFeedProduct = new(x, targetValues.TargetedREcoveragePerKgDm);
				availableFeedProductsList.Add(mappedFeedProduct);
			}

			availableFeedProducts = availableFeedProductsList;
			;
			_improvementSelector.Initialize(currentRation: ref CurrentRation,
				targetValues: ref targetValues);
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
			}

			;
			//fill ration with feed product to get RE on the targeted level.
			CurrentRation.ApplyChangesToRationList(GetGrassRENuturalizerFeedProduct());
			Console.WriteLine("Ration after applying grassREnuturalizerFeedProduct");
			CurrentRation.printProducts();
			//generate RE natural FeedProductGroups #TODO change targetRE when needed: taiga #193
			availableRENaturalFeedProductGroups = GenerateRENaturalFeedProductGroups();
			//fill ration with best FeedProductGroups untill the VEM is on the targeted level, only containing roughage products.
			AbstractMappedFoodItem bestREnaturalFeedProductGroup = FindBestRENaturalFeedProductGroup(false);
			bestREnaturalFeedProductGroup.SetAppliedVem(targetValues.TargetedVEM - CurrentRation.totalVEM);
			Console.WriteLine($"Applying REnaturalFedproductgroep. amount: {bestREnaturalFeedProductGroup.AppliedVem}");
			CurrentRation.ApplyChangesToRationList(bestREnaturalFeedProductGroup);
			Console.WriteLine("Ration after applying REnaturalFedproductgroep");
			CurrentRation.printProducts();
			//check if Ration is in line with target values, if not, improve the ration.
			if (CheckIfRationIsInLineWithTargetValues()) return;
			//if not, improve the ration.
			IImprovementRationMethod[] improvementMethods =
			{
				new ImprovementRationMethodGrassRENuterilizer(),
				new ImprovementRationMethodNaturalREGroups()
			};
			List<AbstractMappedFoodItem> improvementChanges =
				_improvementSelector.DetermineImprovemendRationsWithBijprod(
					availableFeedProducts: availableFeedProducts,
					availableRENaturalFeedProductGroups: availableRENaturalFeedProductGroups,
					improvementMethods: improvementMethods.ToArray());
			CurrentRation.ApplyChangesToRationList(improvementChanges);
			//check if Ration is in line with target values, if not, change Targetvalues.
		}

		private bool CheckIfRationIsInLineWithTargetValues()
		{
			bool inlinewithtargets = true;
			Console.WriteLine("Checking if ration is in line with targetValues");
			if (Math.Abs(CurrentRation.totalRE / CurrentRation.totalDM - targetValues.TargetedREcoveragePerKgDm) > 2)
			{
				inlinewithtargets = false;
				Console.WriteLine(
					$"\t- RE is not in line with target, ration RE/DM:\t\t{CurrentRation.totalRE / CurrentRation.totalDM},\t\ttarget RE/DM:\t{targetValues.TargetedREcoveragePerKgDm}");
			}

			if (Math.Abs(CurrentRation.totalVEM - targetValues.TargetedVEM) > 5)
			{
				inlinewithtargets = false;
				Console.WriteLine(
					$"\t- VEM is not in line with target, ration VEM:\t\t{CurrentRation.totalVEM},\ttarget VEM:\t\t{targetValues.TargetedVEM}");
			}

			if (CurrentRation.totalDM > targetValues.TargetedMaxKgDm)
			{
				inlinewithtargets = false;
				Console.WriteLine(
					$"\t- DM not in line with target, ration KGDM:\t\t\t{CurrentRation.totalDM},\t\ttarget max DM:\t{targetValues.TargetedMaxKgDm}");
			}

			if (CurrentRation.totalDM_Bijprod > targetValues.TargetedMaxKgDmSupplementeryFeedProduct)
			{
				inlinewithtargets = false;
				Console.WriteLine(
					$"\t- DM bijprod not in line with targt, ration KGDMbijprod:{CurrentRation.totalDM_Bijprod},\t\ttarget max DM: \t{targetValues.TargetedMaxKgDmSupplementeryFeedProduct}");
			}

			if (inlinewithtargets) Console.WriteLine($"\t- ration in line with targets");
			return inlinewithtargets;
		}

		//TODO: make this method more efficient.; taiga issue #192
		public List<AbstractMappedFoodItem> GetGrassRENuturalizerFeedProduct(
			bool allowSupplementeryFeedProducts = false)
		{
			if (CurrentRation.totalREdiff == 0) return new List<AbstractMappedFoodItem>();

			bool grassHasPositiveREdiff = (CurrentRation.totalREdiff > 0);
			IEnumerable<AbstractMappedFoodItem> products = (allowSupplementeryFeedProducts)
				? availableFeedProducts
				: availableFeedProducts.Where(x => x.SupplmenteryPartOfTotalVem == 0);

			//Check if products are availlable
			if (!products.Any() && allowSupplementeryFeedProducts == true)
			{
				Console.WriteLine(
					"GetGrassRENuturalizerFeedProduct: No feedproducts without supplementeries available, trying again with supplementeries");
				return GetGrassRENuturalizerFeedProduct(true);
			}

			if (!products.Any()) throw new RationAlgorithmException("No roughage feed products available.");

			//select the best product
			IOrderedEnumerable<AbstractMappedFoodItem> sortedProducts =
				products.OrderBy(x => x.REdiffPerVem / x.KgdMperVem);
			AbstractMappedFoodItem bestproduct =
				(grassHasPositiveREdiff) ? sortedProducts.First() : sortedProducts.Last();
			float vemNeeded = -CurrentRation.totalREdiff / bestproduct.REdiffPerVem;

			//check if product has the right REdiff
			if (grassHasPositiveREdiff && bestproduct.REdiffPerVem > 0)
				throw new NoProductsWithPossibleREException("Products are missing to lower the REdiff");
			if (!grassHasPositiveREdiff && bestproduct.REdiffPerVem < 0)
				throw new NoProductsWithPossibleREException("Products are missing to raise the REdiff");
			//TODO: check if supplementeries don't have too much KG DS when using them, and throw exception if they do; taiga issue 192
			AbstractMappedFoodItem productclone = bestproduct.Clone();
			productclone.SetAppliedVem(vemNeeded);
			Console.WriteLine(
				$"GrassNeuturilizer: totalREdiff = {CurrentRation.totalREdiff}, bestproduct.REdiffPerVEM = {bestproduct.REdiffPerVem}, vemNeeded = {vemNeeded}. product contains now: {(productclone.REdiffPerVem + 1500) * productclone.AppliedVem} RE total and {productclone.AppliedVem} VEM total and {productclone.AppliedREdiff.ToString(CultureInfo.CurrentCulture)} RE difference of the target");
			Console.WriteLine($"GrassNeuturilizer: used product: {bestproduct.GetProductsForConsole()}");
			Console.WriteLine(
				$"VEM needed after GrassRENuturalizer: {targetValues.TargetedVEM - (CurrentRation.totalVEM + vemNeeded)}");
			return new List<AbstractMappedFoodItem> { productclone };
		}


		public List<AbstractMappedFoodItem> GenerateRENaturalFeedProductGroups()
		{
			List<AbstractMappedFoodItem> naturalFeedProductGroups = new();
			//add products that are allready natural as a group with 1 product.
			naturalFeedProductGroups.AddRange(availableFeedProducts.Where(x => x.REdiffPerVem == 0)
				.Select(x => new MappedFeedProductGroup((x, 1f))).ToList());
			foreach (AbstractMappedFoodItem product in availableFeedProducts.Where(x => x.REdiffPerVem < 0))
			{
				foreach (AbstractMappedFoodItem? product2 in availableFeedProducts.Where(x => x.REdiffPerVem > 0))
				{
					float prod2PerProd1 = product.REdiffPerVem / -product2.REdiffPerVem;
					naturalFeedProductGroups.Add(new MappedFeedProductGroup((product, 1f), (product2, prod2PerProd1)));
					Console.WriteLine(
						$"group created, product 1 RE/vem: {product.REdiffPerVem}, product 2 RE/vem: {product2.REdiffPerVem}, product 2 per product 1 in VEM: {prod2PerProd1}. Part supplementery prod 1: {product.SupplmenteryPartOfTotalVem}, prod2: {product2.SupplmenteryPartOfTotalVem}");
				}
			}

			if (naturalFeedProductGroups.Count == 0)
				throw new NoPossibleRENaturalProductGroupsException(targetValues.TargetedREcoveragePerKgDm.ToString());
			return naturalFeedProductGroups;
		}

		//Returns the RE natural feed product group with the least KG DM per VEM.
		public AbstractMappedFoodItem FindBestRENaturalFeedProductGroup(bool supplementeryFeedProductAllowed)
		{
			IEnumerable<AbstractMappedFoodItem> availablegroups =
				availableRENaturalFeedProductGroups.Where(x =>
					x.SupplmenteryPartOfTotalVem == 0 || (supplementeryFeedProductAllowed));
			if (!availablegroups.Any() && !supplementeryFeedProductAllowed)
			{
				Console.WriteLine(
					"No RE natural feed product groups available of only roughages. Switching to supplementeryFeedProductAllowed == true");
				return FindBestRENaturalFeedProductGroup(true);
			}

			AbstractMappedFoodItem bestgroup = availablegroups.OrderBy(x => x.KgdMperVem).First();
			Console.WriteLine(
				$"FindBestReNaturalFeedProductGroup: supplementeryallowed = {supplementeryFeedProductAllowed}, partsupplementery: {bestgroup.SupplmenteryPartOfTotalVem}, DM per VEM: {bestgroup.KgdMperVem}, RE per VEM: {bestgroup.REperVem}");
			return bestgroup;
		}
	};
}