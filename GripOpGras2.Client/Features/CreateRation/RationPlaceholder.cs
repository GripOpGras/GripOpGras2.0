using GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class RationPlaceholder
	{
		public IReadOnlyList<AbstractMappedFoodItem> RationList = new List<AbstractMappedFoodItem>();

		//Constructor, with an optional originalReference. When not given, it wil create a reference to self.
		public RationPlaceholder(RationPlaceholder? reference = null, float? grassIntake = null,
			FeedAnalysis? grassAnalysis = null)
		{
			OriginalRefference = reference ?? this;
			if (grassIntake != null && grassAnalysis != null)
			{
				if (grassAnalysis.Vem == null || grassAnalysis.Re == null)
					throw new RationAlgorithmException("Grass analysis is missing data");

				GrassVem = (float)grassAnalysis.Vem * (float)grassIntake;
				GrassRe = (float)(grassAnalysis.Re * grassIntake);
				GrassKgdm = (float)grassIntake;
				GrassREdiff = (float)((grassAnalysis.Re - TargetValues.OptimalReCoverageInGramsPerKgDm) * grassIntake);
				GrassFeedAnalysis = grassAnalysis;
			}
			else
			{
				GrassVem = 0;
				GrassRe = 0;
				GrassKgdm = 0;
				GrassREdiff = 0;
			}
		}

		private FeedAnalysis? GrassFeedAnalysis { get; }

		private float GrassVem { get; }

		private float GrassRe { get; }

		private float GrassKgdm { get; }

		private float GrassREdiff { get; }

		//public float totalVEM  
		public float TotalVem
		{
			get { return RationList.Sum(x => x.AppliedVem) + GrassVem; }
		}

		public float TotalVemSupplementaryFeedProduct
		{
			get { return RationList.Sum(x => x.AppliedVem * x.SupplmenteryPartOfTotalVem); }
		}

		public float TotalDm
		{
			get { return RationList.Sum(x => x.AppliedKgdm) + GrassKgdm; }
		}

		public float TotalDmSupplementaryFeedProduct
		{
			get { return RationList.Sum(x => x.AppliedVem * x.KgdmSupplementaryFeedProductPerVem); }
		}

		public float TotalReDiff
		{
			get { return RationList.Sum(x => x.AppliedREdiff) + GrassREdiff; }
		}

		public float TotalRe
		{
			get { return RationList.Sum(x => x.AppliedTotalRe) + GrassRe; }
		}

		//Reference to the original class, so clones can be matched.
		public RationPlaceholder? OriginalRefference { get; }

		//adds or subtracts the amount of applied VEM to the RationList
		public void ApplyChangesToRationList(List<AbstractMappedFoodItem> rationChanges)
		{
			ApplyChangesToRationList(rationChanges.ToArray());
		}

		public void ApplyChangesToRationList(params AbstractMappedFoodItem[] rationChanges)
		{
			RationPlaceholder newRation = Clone();
			List<AbstractMappedFoodItem> newList = RationList.ToList();
			foreach (AbstractMappedFoodItem foodItem in rationChanges)
			{
				AbstractMappedFoodItem? itemInRationList =
					newList.Find(x => x.OriginalReference == foodItem.OriginalReference);
				if (itemInRationList != null)
				{
					itemInRationList.SetAppliedVem(itemInRationList.AppliedVem + foodItem.AppliedVem);
					if (itemInRationList.AppliedVem < 0)
						throw new Exception(
							$"Applied VEM cannot be negative.\nItem to change: Vem: {foodItem.AppliedVem}, product: {foodItem.GetProductsForConsole()}\nNew applied vem: {itemInRationList.AppliedVem}");
				}
				else
				{
					if (foodItem.AppliedVem < 0)
						throw new RationAlgorithmException(
							$"Cannot reduce an item that is not in the ration.\nChangedata:\n{foodItem.GetProductsForConsole()}");
					newList.Add(foodItem);
				}
			}

			newList.RemoveAll(x => x.AppliedVem == 0);
			RationList = newList;
		}

		public RationPlaceholder Clone()
		{
			RationPlaceholder clone = new(OriginalRefference, GrassKgdm, GrassFeedAnalysis);
			foreach (AbstractMappedFoodItem item in RationList) clone.ApplyChangesToRationList(item.Clone());

			return clone;
		}

		public Dictionary<FeedProduct, float> GetFeedProducts()
		{
			MappedFeedProductGroup feedproductgroup =
				new(RationList
					.Select(x => (originalRefference: x.OriginalReference, appliedVEM: x.AppliedVem)).ToArray());
			feedproductgroup.SetAppliedVem(RationList.Sum(x => x.AppliedVem));
			return feedproductgroup.GetProducts();
			//new MappedFeedProductGroup(RationList.Select(x => (FoodItem: x, partOfGroupInVEM: x.appliedVEM).ToTuple()).ToList());
		}

		public void PrintProducts()
		{
			Console.WriteLine("Rotation Algorithm finished, rotation:");
			Console.WriteLine($"- {"grass",-25}|{GrassKgdm,10} kg | type: grass");
			foreach (KeyValuePair<FeedProduct, float> feedRationFeedProduct in GetFeedProducts())
				Console.WriteLine(
					$"- {feedRationFeedProduct.Key.Name,-25}|{feedRationFeedProduct.Value,10} kg | type: {feedRationFeedProduct.Key.GetType().Name}");
		}
	}
}