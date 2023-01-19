namespace GripOpGras2.Client.Features.CreateRation.ImprovementMethods
{
	/// <summary>
	///     Interface for all ImprovementRationMethods
	///     ImprovementRationMethods find place after filling the ration with only roughages till the VEM and RE is on the
	///     target amount.
	///     Thus, the ImprovementRationMethods are only called when the ration is already filled with roughages.
	///     the VEM and RE are expected to be on the target amount, which makes the goal of the improvement methods to reduce
	///     the amount of DM and later possible other nutritional values..
	/// </summary>
	public interface IImprovementRationMethod
	{
		List<ImprovementRapport> FindImprovementRationMethod(TargetValues targetValues,
			List<AbstractMappedFoodItem> availableFeedProducts,
			List<AbstractMappedFoodItem> availableReNaturalFeedProductGroups, RationPlaceholder currentRation);
	}
}