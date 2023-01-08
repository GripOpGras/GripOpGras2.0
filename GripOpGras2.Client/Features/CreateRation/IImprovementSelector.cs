using GripOpGras2.Client.Features.CreateRation.ImprovementMethods;

namespace GripOpGras2.Client.Features.CreateRation
{
	public interface IImprovementSelector
	{
		void Initialize(ref Ration currentRation,
			ref TargetValues targetValues,
			ref List<AbstractMappedFoodItem> availableFeedProducts,
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
		public void Initialize(ref Ration currentRation, ref TargetValues targetValues, ref List<AbstractMappedFoodItem> availableFeedProducts,
			ref List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups)
		{
			throw new NotImplementedException();
		}

		public List<AbstractMappedFoodItem> DetermineImprovemendRationsWithBijprod(params ImprovementRapport[] improvementRapports)
		{
			throw new NotImplementedException();
		}

		public List<AbstractMappedFoodItem> DetermineImprovemendRationsWithBijprod(params IImprovementRationMethod[] improvementMethods)
		{
			throw new NotImplementedException();
		}
	}
}