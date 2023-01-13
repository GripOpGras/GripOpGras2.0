namespace GripOpGras2.Client.Features.CreateRation.ImprovementMethods
{
	/// <summary>
	///     Improves the ration by changing the RE-natural feed product groups to more efficient Feedproduct groups by using
	///     supplementaryFeedProducts.
	/// </summary>
	public class ImprovementRationMethodNaturalReGroups : IImprovementRationMethod
	{
		public List<ImprovementRapport> FindImprovementRationMethod(TargetValues targetValues,
			List<AbstractMappedFoodItem> availableFeedProducts,
			List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups, Ration currentRation)
		{
			List<ImprovementRapport> improvementRapportOptions = new();
			foreach (AbstractMappedFoodItem foodItem in currentRation.RationList.Where(x => x.REdiffPerVem < 0.001f))
			foreach (AbstractMappedFoodItem availableGroup in availableRENaturalFeedProductGroups)
			{
				if (foodItem.KgdMperVem < availableGroup.KgdMperVem || foodItem.AppliedVem < 1)
					continue;
				AbstractMappedFoodItem oldItem = foodItem.Clone();
				AbstractMappedFoodItem newItem = availableGroup.Clone();
				oldItem.SetAppliedVem(-1);
				newItem.SetAppliedVem(1);
				List<AbstractMappedFoodItem> changes = new()
				{
					oldItem,
					newItem
				};
				Ration rationWithChanges = currentRation.Clone();
				rationWithChanges.ApplyChangesToRationList(changes);
				ImprovementRapport improvementRapport = new(changes, targetValues, currentRation);
				if (improvementRapport.KgdmChangePerVem != 0)
					improvementRapportOptions.Add(improvementRapport);
			}

			return improvementRapportOptions;
		}
	}
}