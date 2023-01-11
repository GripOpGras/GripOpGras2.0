namespace GripOpGras2.Client.Features.CreateRation.ImprovementMethods
{
	/// <summary>
	/// Improves the ration by changing the RE-natural feed product groups to more efficient Feedproduct groups by using supplementaryFeedProducts.
	/// </summary>
	public class ImprovementRationMethodNaturalREGroups : IImprovementRationMethod
	{
		public ImprovementRapport FindImprovementRationMethod(TargetValues targetValues, List<AbstractMappedFoodItem> availableFeedProducts, List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups, Ration currentRation)
		{
			throw new NotImplementedException();
		}
	}
}