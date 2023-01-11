namespace GripOpGras2.Client.Features.CreateRation
{
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