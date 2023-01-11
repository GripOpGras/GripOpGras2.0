namespace GripOpGras2.Client.Features.CreateRation.ImprovementMethods
{
	/// <summary>
	///#TODO finish improvementMethod with the description below
	/// Finds possible improvements by replacing the product used to combine with the grass, to have the RE/KG on the targeted level.
	/// It checks by listing all products that can replace the product used to combine with the grass, checks what would be the changes per VEM (it not only changes the product used to combine with the grass, but a bit of REnaturalFeedProductGroups in order to keep the VEM the same.)
	/// And then it checks which products have the most difference in KG/DS.
	/// #TODO: return not only 1 improvementRapport, but a few for these catogories: 1. Best improvement with roughages (if possible), 2. Best improvement in KGDM improvement per KGDM supplementery, 3. Best improvement per VEM.
	/// </summary>
	public class ImprovementRationMethodGrassRENuterilizer : IImprovementRationMethod
	{
		public ImprovementRapport FindImprovementRationMethod(TargetValues targetValues,
			List<AbstractMappedFoodItem> availableFeedProducts,
			List<AbstractMappedFoodItem> availableRENaturalFeedProductGroups, Ration currentRation)
		{
			throw new NotImplementedException();
		}
	}
}