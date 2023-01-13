namespace GripOpGras2.Client.Features.CreateRation
{
	public class ImprovementRapport
	{
		public List<AbstractMappedFoodItem> ChangesPerVem;

		public float KgdmChangePerVem => ChangesPerVem.Sum(x => x.AppliedVem * x.KgdMperVem);

		public float KgdmSupplementaryFeedProductChangePerVem =>
			ChangesPerVem.Sum(x => x.AppliedVem * x.KgdmSupplementaryFeedProductPerVem);

		public List<AbstractMappedFoodItem> ChangesPerKgSupplementaryFeedProduct;

		public float KgdmChangePerKgSupplementaryFeedProduct =>
			ChangesPerKgSupplementaryFeedProduct.Sum(x => x.AppliedVem * x.KgdMperVem);

		//The amount of change in VEM that is required with this improvement method.
		public float ChangeInVemRequired;

		/// <summary>
		/// The amount of change in VEM that can be made at max, untill there are no more products to change.
		/// For example, if the imporovement changes prod1 for bijprod 1, it can only change the amount of VEM that was applied on bijpord1.
		/// </summary>
		public float MaxChangeInVem;


		public ImprovementRapport(List<AbstractMappedFoodItem> changesPerVEM, TargetValues targetValues,
			Ration currentRation)
		{
			ChangesPerVem = changesPerVEM.Select(x => x.Clone()).ToList();
			Ration currentRation1 = currentRation.Clone();
			float totalKgSupplementaryFeedProductPerVem = changesPerVEM.Sum(x => x.AppliedVem * x.KgdmSupplementaryFeedProductPerVem);

			float kgOversupply = currentRation.totalDM - targetValues.TargetedMaxKgDm;

			//make a copy of the list and the items in the list
			ChangesPerKgSupplementaryFeedProduct = changesPerVEM.Select(x => x.Clone()).ToList();
			ChangesPerKgSupplementaryFeedProduct.ForEach(x => x.SetAppliedVem(x.AppliedVem / totalKgSupplementaryFeedProductPerVem));
			MaxChangeInVem = GetMaxChangeInVem(ration: currentRation1);
			ChangeInVemRequired = kgOversupply / KgdmChangePerVem;
			changesPerVEM.ForEach(x =>
				Console.WriteLine(
					$"improvementrapport|setup|singlechange: {x.AppliedKgdm}, {x.KgdMperVem}, {x.GetProductsForConsole()}"));
		}

		public float GetMaxChangeInVem(Ration ration)
		{
			List<float> changelist = new();
			foreach (AbstractMappedFoodItem item in ChangesPerVem)
			{
				AbstractMappedFoodItem? existingItem =
					ration.RationList.FirstOrDefault(x => x.OriginalReference == item.OriginalReference);
				if (existingItem == null) continue;
				changelist.Add(existingItem.AppliedVem);
			}

			return changelist.Min();
		}
	}
}