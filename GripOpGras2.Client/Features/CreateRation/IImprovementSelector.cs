namespace GripOpGras2.Client.Features.CreateRation
{
	public interface IImprovementSelector
	{
		///  <summary>
		/// 		This method will consider the results of different improvement methods and combine them into one list of changes.
		/// 		#TODO give an log.Info in console when a change is made
		///  </summary>
		///  <param name="improvementRapports">Rapports of various changes to </param>
		///  <param name="originalRation"></param>
		///  <returns>List with values of the changes.</returns>
		List<AbstractMappedFoodItem> DetermineImprovemendRationsWithBijprod(Ration originalRation,
			params ImprovementRapport[] improvementRapports);
	}

	public class ImprovementSelectorV1 : IImprovementSelector
	{
		///  <summary>
		/// 		This method will consider the results of different improvement methods and combine them into one list of changes.
		/// 		#TODO give an log.Info in console when a change is made
		///  </summary>
		///  <param name="improvementRapports">Rapports of various changes to </param>
		///  <param name="originalRation"></param>
		///  <returns>List with values of the changes.</returns>
		public List<AbstractMappedFoodItem> DetermineImprovemendRationsWithBijprod(Ration originalRation, params ImprovementRapport[] improvementRapports)
		{
			throw new NotImplementedException();
		}
	}
}