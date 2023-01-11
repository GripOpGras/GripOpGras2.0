using GripOpGras2.Client.Features.CreateRation.ImprovementMethods;

namespace GripOpGras2.Client.Features.CreateRation
{
	public interface IImprovementSelector
	{
		void Initialize(ref Ration currentRation,
			ref TargetValues targetValues,
			ref IReadOnlyList<AbstractMappedFoodItem> availableFeedProducts,
			ref List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups);

		///  <summary>
		/// 		This method will consider the results of different improvement methods and combine them into one list of changes.
		/// 		#TODO give an log.Info in console when a change is made
		///  </summary>
		///  <param name="improvementRapports">Rapports of various changes to </param>
		///  <returns>List with values of the changes.</returns>
		List<AbstractMappedFoodItem> DetermineImprovemendRationsWithBijprod(
			params ImprovementRapport[] improvementRapports);

		List<AbstractMappedFoodItem> DetermineImprovemendRationsWithBijprod(
			params IImprovementRationMethod[] improvementMethods);
	}

	public class ImprovementSelectorV1 : IImprovementSelector
	{
		private Ration _currentRation;

		private TargetValues _targetValues;

		private IReadOnlyList<AbstractMappedFoodItem> _availableFeedProducts;

		private List<AbstractMappedFoodItem> _availableRENaturalFeedProductGroups;

		public void Initialize(ref Ration currentRation, ref TargetValues targetValues,
			ref IReadOnlyList<AbstractMappedFoodItem> availableFeedProducts,
			ref List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups)
		{
			_currentRation = currentRation;
			_targetValues = targetValues;
			_availableFeedProducts = availableFeedProducts;
			_availableRENaturalFeedProductGroups = availableRENaturalFeedProductGroups;
		}

		public List<AbstractMappedFoodItem> DetermineImprovemendRationsWithBijprod(
			params ImprovementRapport[] improvementRapports)
		{
			throw new NotImplementedException(
				"ImprovementSelectorV1: DetermineImprovedRationsWithBijprod (improvementRapports)");
		}

		public List<AbstractMappedFoodItem> DetermineImprovemendRationsWithBijprod(
			params IImprovementRationMethod[] improvementMethods)
		{
			throw new NotImplementedException(
				"ImprovementSelectorV1: DetermineImprovedRationsWithBijprod (improvementmethods)");
		}
	}
}