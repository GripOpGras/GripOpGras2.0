using GripOpGras2.Client.Features.CreateRation.ImprovementMethods;

namespace GripOpGras2.Client.Features.CreateRation
{
	public interface IImprovementSelector
	{
		void Initialize(ref RationPlaceholder currentRation,
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

		private RationPlaceholder _currentRation = null!;

		private TargetValues _targetValues = null!;

		public void Initialize(ref RationPlaceholder currentRation, ref TargetValues targetValues)
		{
			_currentRation = currentRation;
			_targetValues = targetValues;
		}

		public List<AbstractMappedFoodItem> DetermineImprovementRationsWithSupplementaryFeedProduct(
			IReadOnlyList<AbstractMappedFoodItem> availableFeedProducts,
			List<AbstractMappedFoodItem> availableReNaturalFeedProductGroups,
			params IImprovementRationMethod[] improvementMethods)
		{
			ImprovementRapport[] improvementRapports = improvementMethods.Select(delegate (IImprovementRationMethod x)
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
			IEnumerable<ImprovementRapport> orderedByKgdmPerVem =
				improvementRapports.OrderBy(x => x.KgdmChangePerKgSupplementaryFeedProduct);
			RationPlaceholder testRationPlaceholder = TestImprovements(orderedByKgdmPerVem,
				out Dictionary<ImprovementRapport, float> firstRoundChanges, _currentRation.Clone());
			List<AbstractMappedFoodItem> changeList =
				firstRoundChanges.SelectMany(x => SetVemPerFoodItem(x.Key, x.Value)).ToList();
			bool secondRoundNeeded = (testRationPlaceholder.TotalDm > _targetValues.TargetedMaxKgDm);
			Console.WriteLine(_targetValues.TargetedMaxKgDmSupplementaryFeedProduct);
			Console.WriteLine($"Improvementselector: Second improvement round: {secondRoundNeeded}");
			if (secondRoundNeeded)
			{
				IEnumerable<ImprovementRapport> newRapports = improvementMethods.SelectMany(x => x.FindImprovementRationMethod(
					_targetValues,
					(List<AbstractMappedFoodItem>)availableFeedProducts,
					availableReNaturalFeedProductGroups,
					testRationPlaceholder.Clone())).OrderBy(x => x.KgdmChangePerVem);
				TestImprovements(newRapports,
					out Dictionary<ImprovementRapport, float> secondRoundChanges, testRationPlaceholder);
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

			Console.WriteLine("Improvementselector: Improvementrounds ended.");
			changeList.RemoveAll(x => x.AppliedVem == 0f);
			return changeList;
		}

		private RationPlaceholder TestImprovements(IEnumerable<ImprovementRapport> orderedImprovements,
			out Dictionary<ImprovementRapport, float> dictionary, RationPlaceholder testRationPlaceholder)
		{
			Console.WriteLine(
				$"Improvementselector | improvementround | Amount of rapports: {orderedImprovements.Count()}");
			dictionary = new Dictionary<ImprovementRapport, float>();
			//Sort the rapports on the amount of KG chang per VM. See how much can be changed, and then try the next product with the most change. When the bijprod is at it's max, replace the product with the least difference with the changes with the most change per KGDM bijprod.
			foreach (ImprovementRapport rapport in orderedImprovements)
			{
				float kgChangeNeeded = _targetValues.TargetedMaxKgDm - testRationPlaceholder.TotalDm;
				float maxChangeInVem = rapport.GetMaxChangeInVem(testRationPlaceholder);
				float changeInVemRequired = kgChangeNeeded / rapport.KgdmChangePerVem;
				float maxChangeForKgDmSupplementaryFeedProduct =
					(_targetValues.TargetedMaxKgDmSupplementaryFeedProduct -
					 testRationPlaceholder.TotalDmSupplementaryFeedProduct) / rapport.KgdmSupplementaryFeedProductChangePerVem;
				if (maxChangeForKgDmSupplementaryFeedProduct < 0)
					maxChangeForKgDmSupplementaryFeedProduct = float.MaxValue;
				Console.WriteLine(
					$"Improvementselector | improvementround | testrapport: maxChangeInVem: {maxChangeInVem} changeInVemRequired: {changeInVemRequired}, maxChangeForKgDmSupplementaryFeedProduct: {maxChangeForKgDmSupplementaryFeedProduct}");
				float changeInVem = new[] { maxChangeInVem, changeInVemRequired, maxChangeForKgDmSupplementaryFeedProduct }.Min();
				if (changeInVem < 0) changeInVem = 0;
				dictionary.Add(rapport, changeInVem);
				testRationPlaceholder.PrintProducts();
				testRationPlaceholder.ApplyChangesToRationList(SetVemPerFoodItem(rapport, changeInVem));
				Console.WriteLine(
					$"Improvementselector | improvementround | testrapport: amount of change: {changeInVem}");
			}

			Console.WriteLine("Improvementselector | improvementround | returning improved ration:");
			testRationPlaceholder.PrintProducts();
			return testRationPlaceholder;
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