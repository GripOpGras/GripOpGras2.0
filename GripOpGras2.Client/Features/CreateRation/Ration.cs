using System.Diagnostics;
using GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;
using NUnit.Framework.Internal;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class Ration
	{
		public IReadOnlyList<AbstractMappedFoodItem> RationList = new List<AbstractMappedFoodItem>();
		private FeedAnalysis? grassFeedAnalysis { get; }
		private float grassVEM { get; }
		private float grassRE { get; }
		private float grassKGDM { get; }
		private float grassREdiff { get; }


		//public Herd GrassHerd{ get; private set; }

		//public float totalVEM  
		public float totalVEM { get { return this.RationList.Sum(x => x.appliedVEM)+grassVEM; } }
		public float totalVEM_Bijprod { get { return this.RationList.Sum(x => x.appliedVEM*x.partOfTotalVEMbijprod); } }
		public float totalDM { get { return this.RationList.Sum(x => x.appliedKGDM) + grassKGDM; } }
		public float totalDM_Bijprod { get { return this.RationList.Sum(x => x.appliedVEM*x.KGDMPerVEM_bijprod); } }
		public float totalREdiff { get { return this.RationList.Sum(x => x.appliedREdiff) + grassREdiff; } }
		public float totalRE { get { return this.RationList.Sum(x => x.appliedTotalRE) + grassRE; } }
		//Reference to the original class, so clones can be matched.
		public Ration? originalRefference { get; }

		//Constructor, with an optional originalReference. When not given, it wil create a reference to self.
		public Ration(Ration? reference = null, float? grassIntake = null, FeedAnalysis? grassAnalysis = null)
		{
			originalRefference = reference ?? this;
			if (grassIntake != null && grassAnalysis != null)
			{
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

		//adds or subtracts the amount of applied VEM to the RationList
		public void ApplyChangesToRationList(List<AbstractMappedFoodItem> rationChanges) { ApplyChangesToRationList(rationChanges.ToArray()); }
		public void ApplyChangesToRationList(params AbstractMappedFoodItem[] rationChanges)
		{
			Ration newRation = Clone();
			List<AbstractMappedFoodItem> newList = RationList.ToList();
			foreach (AbstractMappedFoodItem foodItem in rationChanges)
			{
				AbstractMappedFoodItem? itemInRationList = newList.Find(x => x.originalRefference == foodItem.originalRefference);
				if (itemInRationList != null)
				{
					itemInRationList.setAppliedVEM(itemInRationList.appliedVEM+foodItem.appliedVEM);
					if (itemInRationList.appliedVEM < 0) throw new Exception("Applied VEM cannot be negative");
				}
				else
				{
					if (foodItem.appliedVEM < 0) throw new RationAlgorithmException($"Cannot reduce an item that is not in the ration.\nChangedata:\n{foodItem.GetProductsForConsole()}");
					newList.Add(foodItem);
				}

			}
			RationList = newList;
		}
		public Ration Clone()
		{
			Ration clone = new(reference: originalRefference, grassIntake: grassKGDM, grassAnalysis: grassFeedAnalysis);
			foreach (AbstractMappedFoodItem item in RationList)
			{
				clone.ApplyChangesToRationList(item.Clone());
			}
			return clone;
		}

		public Dictionary<FeedProduct, float> getFeedProducts()
		{
			var feedproductgroup =
				new MappedFeedProductGroup(RationList.Select(x => (x.originalRefference, x.appliedVEM)).ToArray());
			feedproductgroup.setAppliedVEM(RationList.Sum(x => x.appliedVEM));
			return feedproductgroup.GetProducts().ToDictionary(x => x.Item1, x => x.Item2);
			//new MappedFeedProductGroup(RationList.Select(x => (FoodItem: x, partOfGroupInVEM: x.appliedVEM).ToTuple()).ToList());
		} 
			

	}
}