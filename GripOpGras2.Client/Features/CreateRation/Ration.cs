using GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class Ration
	{
		public IReadOnlyList<AbstractMappedFoodItem> RationList = new List<AbstractMappedFoodItem>();

		//Constructor, with an optional originalReference. When not given, it wil create a reference to self.
		public Ration(Ration? reference = null, float? grassIntake = null, FeedAnalysis? grassAnalysis = null)
		{
			originalRefference = reference ?? this;
			if (grassIntake != null && grassAnalysis != null)
			{
				if (grassAnalysis.VEM == null || grassAnalysis.RE == null)
					throw new RationAlgorithmException("Grass analysis is missing data");

				grassVEM = (float)grassAnalysis.VEM * (float)grassIntake;
				grassRE = (float)(grassAnalysis.RE * grassIntake);
				grassKGDM = (float)grassIntake;
				grassREdiff = (float)((grassAnalysis.RE - TargetValues.OptimalRECoverageInGramsPerKgDm) * grassIntake);
				grassFeedAnalysis = grassAnalysis;
			}
			else
			{
				grassVEM = 0;
				grassRE = 0;
				grassKGDM = 0;
				grassREdiff = 0;
			}
		}

		private FeedAnalysis? grassFeedAnalysis { get; }

		private float grassVEM { get; }

		private float grassRE { get; }

		private float grassKGDM { get; }

		private float grassREdiff { get; }


		//public Herd GrassHerd{ get; private set; }

		//public float totalVEM  
		public float totalVEM
		{
			get { return RationList.Sum(x => x.AppliedVem) + grassVEM; }
		}

		public float totalVEM_SupplementaryFeedProduct
		{
			get { return RationList.Sum(x => x.AppliedVem * x.SupplmenteryPartOfTotalVem); }
		}

		public float totalDM
		{
			get { return RationList.Sum(x => x.AppliedKgdm) + grassKGDM; }
		}

		public float totalDM_SupplementaryFeedProduct
		{
			get { return RationList.Sum(x => x.AppliedVem * x.KgdmSupplementaryFeedProductPerVem); }
		}

		public float totalREdiff
		{
			get { return RationList.Sum(x => x.AppliedREdiff) + grassREdiff; }
		}

		public float totalRE
		{
			get { return RationList.Sum(x => x.AppliedTotalRe) + grassRE; }
		}

		//Reference to the original class, so clones can be matched.
		public Ration? originalRefference { get; }

		//adds or subtracts the amount of applied VEM to the RationList
		public void ApplyChangesToRationList(List<AbstractMappedFoodItem> rationChanges)
		{
			ApplyChangesToRationList(rationChanges.ToArray());
		}

		public void ApplyChangesToRationList(params AbstractMappedFoodItem[] rationChanges)
		{
			Ration newRation = Clone();
			List<AbstractMappedFoodItem> newList = RationList.ToList();
			foreach (AbstractMappedFoodItem foodItem in rationChanges)
			{
				AbstractMappedFoodItem? itemInRationList =
					newList.Find(x => x.OriginalReference == foodItem.OriginalReference);
				if (itemInRationList != null)
				{
					itemInRationList.SetAppliedVem(itemInRationList.AppliedVem + foodItem.AppliedVem);
					if (itemInRationList.AppliedVem < 0) throw new Exception("Applied VEM cannot be negative");
				}
				else
				{
					if (foodItem.AppliedVem < 0)
						throw new RationAlgorithmException(
							$"Cannot reduce an item that is not in the ration.\nChangedata:\n{foodItem.GetProductsForConsole()}");
					newList.Add(foodItem);
				}
			}

			RationList = newList;
		}

		public Ration Clone()
		{
			Ration clone = new(originalRefference, grassKGDM, grassFeedAnalysis);
			foreach (AbstractMappedFoodItem item in RationList) clone.ApplyChangesToRationList(item.Clone());

			return clone;
		}

		public Dictionary<FeedProduct, float> getFeedProducts()
		{
			MappedFeedProductGroup feedproductgroup =
				new(RationList
					.Select(x => (originalRefference: x.OriginalReference, appliedVEM: x.AppliedVem)).ToArray());
			feedproductgroup.SetAppliedVem(RationList.Sum(x => x.AppliedVem));
			return feedproductgroup.GetProducts();
			//new MappedFeedProductGroup(RationList.Select(x => (FoodItem: x, partOfGroupInVEM: x.appliedVEM).ToTuple()).ToList());
		}

		public void printProducts()
		{
			Console.WriteLine("Rotation Algorithm finished, rotation:");
			Console.WriteLine($"- {"grass",-25}|{grassKGDM,10} kg | type: grass");
			foreach (KeyValuePair<FeedProduct, float> feedRationFeedProduct in getFeedProducts())
				Console.WriteLine(
					$"- {feedRationFeedProduct.Key.Name,-25}|{feedRationFeedProduct.Value,10} kg | type: {feedRationFeedProduct.Key.GetType().Name}");
		}
	}
}