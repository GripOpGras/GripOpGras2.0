namespace GripOpGras2.Client.Features.CreateRation.ImprovementMethods
{
	/// <summary>
	///     This method will improve the ration by changing the targeted coverages.
	///     For example, by changing the RE to 160g/kg DM.
	/// </summary>
	public class ImprovementRationMethodChangeTargetedCoverages : IImprovementRationMethod
	{
		public List<ImprovementRapport> FindImprovementRationMethod(TargetValues targetValues,
			List<AbstractMappedFoodItem> availableFeedProducts,
			List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups, Ration currentRation)
		{
			throw new NotImplementedException();
		}
	}
}