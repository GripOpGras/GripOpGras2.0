using GripOpGras2.Client.Features.CreateRation.ImprovementMethods;

namespace GripOpGras2.Client.Features.CreateRation
{
	public interface IImprovementSelector
	{
		void Initialize(ref Ration currentRation,
			ref TargetValues targetValues);

		List<AbstractMappedFoodItem> DetermineImprovementRationsWithSupplementaryFeedProduct(
			IReadOnlyList<AbstractMappedFoodItem> availableFeedProducts,
			List<AbstractMappedFoodItem> availableReNaturalFeedProductGroups,
			params IImprovementRationMethod[] improvementMethods);
	}

	public class ImprovementSelectorV1 : IImprovementSelector
	{
		private readonly IImprovementRationMethod[] _basicImprovementMethods =
		{
			new ImprovementRationMethodNaturalReGroups(),
			new ImprovementRationMethodGrassReNuterilizer()
		};

		private Ration _currentRation = null!;

		private TargetValues _targetValues = null!;

		public void Initialize(ref Ration currentRation, ref TargetValues targetValues)
		{
			_currentRation = currentRation;
			_targetValues = targetValues;
		}

		public List<AbstractMappedFoodItem> DetermineImprovementRationsWithSupplementaryFeedProduct(
			IReadOnlyList<AbstractMappedFoodItem> availableFeedProducts,
			List<AbstractMappedFoodItem> availableReNaturalFeedProductGroups,
			params IImprovementRationMethod[] improvementMethods)
		{
			ImprovementRapport[] improvementRapports = improvementMethods.Select(delegate(IImprovementRationMethod x)
				{
					Console.WriteLine($"Improvementselector | DetermineImprovements | By improvementmethod: {x.GetType().FullName}");
					return x.FindImprovementRationMethod(_targetValues,
						(List<AbstractMappedFoodItem>)availableFeedProducts,
						availableReNaturalFeedProductGroups,
						_currentRation.Clone());
				}).SelectMany(x => x)
				.ToArray();
			return RunImprovementAlgorithm(improvementRapports
				, availableFeedProducts, availableReNaturalFeedProductGroups,
				improvementMethods);
		}

		private List<AbstractMappedFoodItem> RunImprovementAlgorithm(ImprovementRapport[] improvementRapports,
			IReadOnlyList<AbstractMappedFoodItem> availableFeedProducts,
			List<AbstractMappedFoodItem> availableReNaturalFeedProductGroups,
			IImprovementRationMethod[]? improvementMethods)
		{
			improvementMethods ??= _basicImprovementMethods;
			Console.WriteLine("Improvementselector: Run Improvement Algorithm");
			Ration testRation = TestImprovements(improvementRapports,
				out Dictionary<ImprovementRapport, float> firstRoundChanges, _currentRation);
			List<AbstractMappedFoodItem> changeList =
				firstRoundChanges.SelectMany(x => SetVemPerFoodItem(x.Key, x.Value)).ToList();
			if (testRation.TotalDm < _targetValues.TargetedMaxKgDm
			    && testRation.TotalDmSupplementaryFeedProduct < _targetValues.TargetedMaxKgDmSupplementaryFeedProduct)
			{
				ImprovementRapport[] newRapports = improvementMethods.SelectMany(x => x.FindImprovementRationMethod(
						_targetValues,
						(List<AbstractMappedFoodItem>)availableFeedProducts,
						availableReNaturalFeedProductGroups,
						testRation.Clone()))
					.ToArray();
				Ration testRation2 = TestImprovements(newRapports,
					out Dictionary<ImprovementRapport, float> secondRoundChanges, testRation);
				List<AbstractMappedFoodItem> secondChangeList =
					secondRoundChanges.SelectMany(x => SetVemPerFoodItem(x.Key, x.Value)).ToList();
				//combine Lists
				foreach (AbstractMappedFoodItem item in secondChangeList)
				{
					;
					AbstractMappedFoodItem? original =
						changeList.FirstOrDefault(x => x.OriginalReference == item.OriginalReference);
					if (original == null) changeList.Add(item);
					else original.SetAppliedVem(original.AppliedVem + item.AppliedVem);
				}
			}

			return changeList;
		}

		private Ration TestImprovements(ImprovementRapport[] improvementRapports1,
			out Dictionary<ImprovementRapport, float> dictionary, Ration testRation)
		{
			testRation = testRation.Clone();
			float kgChangeNeeded = _targetValues.TargetedMaxKgDm - testRation.TotalDm;
			Console.WriteLine(
				$"Improvementselector | improvementround | Amount of rapports: {improvementRapports1.Length}");
			dictionary = new Dictionary<ImprovementRapport, float>();
			//Sort the rapports on the amount of KG chang per VM. See how much can be changed, and then try the next product with the most change. When the bijprod is at it's max, replace the product with the least difference with the changes with the most change per KGDM bijprod.
			IEnumerable<ImprovementRapport> orderedByKgdmPerVem =
				improvementRapports1.OrderBy(x => x.KgdmChangePerKgSupplementaryFeedProduct).Reverse();
			foreach (ImprovementRapport rapport in orderedByKgdmPerVem)
			{
				float maxChangeInVem = rapport.GetMaxChangeInVem(testRation);
				float changeInVemRequired = kgChangeNeeded / rapport.KgdmChangePerVem;
				float maxChangeForKgDmSupplementaryFeedProduct =
					(_targetValues.TargetedMaxKgDmSupplementaryFeedProduct -
					 testRation.TotalDmSupplementaryFeedProduct) / rapport.KgdmSupplementaryFeedProductChangePerVem;
				Console.WriteLine(
					$"Improvementselector | improvementround | testrapport: maxChangeInVem: {maxChangeInVem} changeInVemRequired: {changeInVemRequired}, maxChangeForKgDmSupplementaryFeedProduct: {maxChangeForKgDmSupplementaryFeedProduct}");
				float changeInVem = new float[] { maxChangeInVem, changeInVemRequired, maxChangeForKgDmSupplementaryFeedProduct }.Min();
				if (changeInVem < 0) changeInVem = 0;
				dictionary.Add(rapport, changeInVem);
				testRation.ApplyChangesToRationList(SetVemPerFoodItem(rapport, changeInVem));
				Console.WriteLine(
					$"Improvementselector | improvementround | testrapport: amount of change: {changeInVem}");
			}

			Console.WriteLine("Improvementselector | improvementround | returning improved ration:");
			testRation.PrintProducts();
			return testRation;
		}

		public List<AbstractMappedFoodItem> DetermineImprovemendRationsWithSupplementaryFeedProduct(
			IReadOnlyList<AbstractMappedFoodItem> availableFeedProducts,
			List<AbstractMappedFoodItem> availableReNaturalFeedProductGroups,
			params ImprovementRapport[] improvementRapports)
		{
			return RunImprovementAlgorithm(improvementRapports, availableFeedProducts,
				availableReNaturalFeedProductGroups, null);
		}

		private static List<AbstractMappedFoodItem> SetVemPerFoodItem(ImprovementRapport rapport, float vem)
		{
			IEnumerable<AbstractMappedFoodItem> tmpclone = rapport.ChangesPerVem.Select(x => x.Clone());
			List<AbstractMappedFoodItem> returnList = new();
			foreach (AbstractMappedFoodItem tmpCloneItem in tmpclone)
			{
				tmpCloneItem.SetAppliedVem(vem * tmpCloneItem.AppliedVem);
				returnList.Add(tmpCloneItem);
			}

			return returnList;
		}
	}
}