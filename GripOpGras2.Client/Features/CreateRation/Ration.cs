using GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class Ration
	{
		public List<AbstractMappedFoodItem> RationList = new();
		//public float totalVEM  

		public float totalVEM { get { return this.RationList.Sum(x => x.appliedVEM); } }
		public float totalVEM_Bijprod { get { return this.RationList.Sum(x => x.appliedVEM*x.partOfTotalVEMbijprod); } }
		public float totalDM { get { return this.RationList.Sum(x => x.appliedKGDM); } }
		public float totalDM_Bijprod { get { return this.RationList.Sum(x => x.appliedVEM*x.KGDMPerVEM_bijprod); } }
		public float totalREdiff { get { return this.RationList.Sum(x => x.appliedREdiff); } }
		//Reference to the original class, so clones can be matched.
		public Ration? originalRefference { get; }

		//Constructor, with an optional originalReference. When not given, it wil create a reference to self.
		public Ration(Ration? reference = null)
		{
			originalRefference = reference ?? this;
		}

		//adds or subtracts the amount of applied VEM to the RationList
		public void ApplyChangesToRationList(List<AbstractMappedFoodItem> rationChanges)
		{
			Ration newRation = Clone();
			foreach (AbstractMappedFoodItem foodItem in rationChanges)
			{
				AbstractMappedFoodItem? itemInRationList = newRation.RationList.Find(x => x.originalRefference == foodItem.originalRefference);
				if (itemInRationList != null)
				{
					itemInRationList.setAppliedVEM(itemInRationList.appliedVEM+foodItem.appliedVEM);
					if (itemInRationList.appliedVEM < 0) throw new Exception("Applied VEM cannot be negative");
				}
				else
				{
					if (foodItem.appliedVEM < 0) throw new RationAlgorithmException("Cannot reduce an item that is not in the ration");
					this.RationList.Add(foodItem);
				}

			}
			RationList = newRation.RationList;
		}
		public Ration Clone()
		{
			Ration clone = new(reference: originalRefference);
			foreach (AbstractMappedFoodItem item in RationList)
			{
				clone.RationList.Add(item.Clone());
			}
			return clone;
		}

	}
}