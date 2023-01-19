namespace GripOpGras2.Client.Features.CreateRation.ImprovementMethods
{
	/// <summary>
	///     #TODO finish improvementMethod with the description below
	///     Finds possible improvements by replacing the product used to combine with the grass, to have the RE/KG on the
	///     targeted level.
	///     It checks by listing all products that can replace the product used to combine with the grass, checks what would be
	///     the changes per VEM (it not only changes the product used to combine with the grass, but a bit of
	///     REnaturalFeedProductGroups in order to keep the VEM the same.)
	///     And then it checks which products have the most difference in KG/DS.
	///     #TODO: return not only 1 improvementRapport, but a few for these catogories: 1. Best improvement with roughages (if
	///     possible), 2. Best improvement in KGDM improvement per KGDM Supplementary, 3. Best improvement per VEM.
	/// </summary>
	public class ImprovementRationMethodGrassReNuterilizer : IImprovementRationMethod
	{
		public List<ImprovementRapport> FindImprovementRationMethod(TargetValues targetValues,
			List<AbstractMappedFoodItem> availableFeedProducts,
			List<AbstractMappedFoodItem> availableReNaturalFeedProductGroups, RationPlaceholder currentRation)
		{
			List<ImprovementRapport> improvementRapportOptions = new();
			//foreach product that has a RE/KG that isn't equal to the targeted Value, check if it can be replaced by another product.
			foreach (AbstractMappedFoodItem product in currentRation.RationList)
			{
				//check if REperVEM differs more than 0.01 from 0
				if (!(Math.Abs(product.REperVem) > 0.1f)) continue;
				//make for each available product a list of changes that would be made to the ration.
				foreach (AbstractMappedFoodItem feedProduct in availableFeedProducts)
				{
					float amountOfNewProductNeededPerVem = product.REdiffPerVem / feedProduct.REdiffPerVem;
					if (amountOfNewProductNeededPerVem < 0) continue;
					//make a list of changes that would be made to the ration.
					List<AbstractMappedFoodItem> changes = new();
					AbstractMappedFoodItem oldItem = product.Clone();
					AbstractMappedFoodItem newItem = feedProduct.Clone();
					oldItem.SetAppliedVem(-1);
					newItem.SetAppliedVem(amountOfNewProductNeededPerVem);
					float extraVemChanges = newItem.AppliedVem - oldItem.AppliedVem;
					if (extraVemChanges < 0)
					{
						foreach (AbstractMappedFoodItem rationItem in currentRation.RationList)
							if (Math.Abs(rationItem.REdiffPerVem) > 0.001f)
							{
								AbstractMappedFoodItem rationItemClone = rationItem.Clone();
								rationItemClone.SetAppliedVem(extraVemChanges);
								changes.Add(rationItemClone);
								break;
							}
					}
					else
					{
						AbstractMappedFoodItem bestProductGroup = availableReNaturalFeedProductGroups
							.OrderByDescending(x => x.KgdMperVem).First();
						AbstractMappedFoodItem rationItemClone = bestProductGroup.Clone();
						rationItemClone.SetAppliedVem(extraVemChanges);
						changes.Add(rationItemClone);
					}

					ImprovementRapport improvementRapport = new(changes, targetValues, currentRation);
					if (improvementRapport.KgdmChangePerVem != 0)
						improvementRapportOptions.Add(improvementRapport);
				}
			}

			return improvementRapportOptions;
		}
	}
}