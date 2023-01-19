namespace GripOpGras2.Client.Features.CreateRation
{
	public class ImprovementRapport
	{
		//The amount of change in VEM that is required with this improvement method.
		public float ChangeInVemRequired;

		public List<AbstractMappedFoodItem> ChangesPerKgSupplementaryFeedProduct;

		public List<AbstractMappedFoodItem> ChangesPerVem;

		/// <summary>
		///     The amount of change in VEM that can be made at max, untill there are no more products to change.
		///     For example, if the imporovement changes prod1 for bijprod 1, it can only change the amount of VEM that was applied
		///     on bijpord1.
		/// </summary>
		public float MaxChangeInVem;

		private readonly TargetValues _targetValues;

		public ImprovementRapport(List<AbstractMappedFoodItem> changesPerVem, TargetValues targetValues,
			RationPlaceholder currentRation)
		{
			_targetValues = targetValues;
			RationPlaceholder currentRationClone = currentRation.Clone();
			ChangesPerVem = changesPerVem.Select(x => x.Clone()).ToList();
			float totalKgSupplementaryFeedProductPerVem =
				changesPerVem.Sum(x => x.AppliedVem * x.KgdmSupplementaryFeedProductPerVem);
			ChangeInVemRequired = GetChangeInVemRequired(currentRationClone);
			//make a copy of the list and the items in the list
			ChangesPerKgSupplementaryFeedProduct = changesPerVem.Select(x => x.Clone()).ToList();
			ChangesPerKgSupplementaryFeedProduct.ForEach(x =>
				x.SetAppliedVem(x.AppliedVem / totalKgSupplementaryFeedProductPerVem));
			MaxChangeInVem = GetMaxChangeInVem(currentRationClone);
			Console.WriteLine($"improvementrapport|setup: Change in VEM required: {ChangeInVemRequired}, Products:");
			changesPerVem.ForEach(x =>
				Console.WriteLine(
					$"improvementrapport|setup|singleFoodItemChange: Amount of KGDM change: {x.AppliedKgdm}, KGDM change pr VM:{x.KgdMperVem}, Products: {x.GetProductsForConsole()}"));
		}

		public float KgdmChangePerVem => ChangesPerVem.Sum(x => x.AppliedVem * x.KgdMperVem);

		public float KgdmSupplementaryFeedProductChangePerVem =>
			ChangesPerVem.Sum(x => x.AppliedVem * x.KgdmSupplementaryFeedProductPerVem);

		public float KgdmChangePerKgSupplementaryFeedProduct =>
			ChangesPerKgSupplementaryFeedProduct.Sum(x => x.AppliedVem * x.KgdMperVem);

		/// <summary>
		/// Returns amount of VEM (ChangesPerVem) that needs to be changed, to get the DM on the targeted level.
		/// </summary>
		/// <param name="ration"></param>
		/// <returns></returns>
		public float GetChangeInVemRequired(RationPlaceholder ration)
		{
			float kgOversupply = ration.TotalDm - _targetValues.TargetedMaxKgDm;
			return kgOversupply / -KgdmChangePerVem;
		}

		/// <summary>
		/// Returns the max value that could be subtracted from the given ration.
		/// </summary>
		/// <param name="ration"></param>
		/// <returns>The max value that could be applied, without getting an negative value inside of the ration.</returns>
		public float GetMaxChangeInVem(RationPlaceholder ration)
		{
			List<float> changelist = new();
			foreach (AbstractMappedFoodItem item in ChangesPerVem)
			{
				if (item.AppliedVem > 0) continue;
				AbstractMappedFoodItem? existingItem =
					ration.RationList.FirstOrDefault(x => x.OriginalReference == item.OriginalReference);
				if (existingItem != null)
				{
					changelist.Add(-existingItem.AppliedVem / item.AppliedVem);
				}
				else return 0;
			}

			return !changelist.Any() ? float.MaxValue : changelist.Min();
		}
	}
}