using System.Runtime.CompilerServices;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public abstract class AbstractMappedFoodItem
	{

		public float KGDMperVEM { get; private set; }

		public float KGDMPerVEM_bijprod { get; private set; }

		public float REdiffPerVEM { get; private set; }
		public float REdiffPerVEM_bijprod { get; private set; }

		public float appliedVEM { get; private set; }

		public float appliedKGDM { get; private set; }

		public float appliedREdiff { get; private set; }

		//Gives a number between 1 and 0, which reperesents the percentage of VEM that is bijproduct
		public float partOfTotalVEMbijprod { get; protected set; }

		public abstract List<Tuple<FeedProduct, float>> GetProducts();

		public abstract AbstractMappedFoodItem Clone();
		//reference to original
		public AbstractMappedFoodItem originalRefference { get; } = null!;


		public void setAppliedVEM(float VEM)
		{
			appliedVEM = VEM;
			appliedKGDM = VEM * KGDMperVEM;
			appliedREdiff = VEM * REdiffPerVEM;
		}
	}

	public class MappedFeedProduct : AbstractMappedFoodItem
	{

		public MappedFeedProduct(FeedProduct feedProduct)
		{
			throw new NotImplementedException();
		}

		public override List<Tuple<FeedProduct, float>> GetProducts()
		{
			throw new NotImplementedException();
		}

		public override AbstractMappedFoodItem Clone()
		{
			throw new NotImplementedException();
		}
	}

	public class MappedFeedProductGroup : AbstractMappedFoodItem
	{

		public MappedFeedProductGroup(List<Tuple<AbstractMappedFoodItem, float>> products)
		{
			throw new NotImplementedException();
		}

		public override List<Tuple<FeedProduct, float>> GetProducts()
		{
			throw new NotImplementedException();
		}

		public override AbstractMappedFoodItem Clone()
		{
			throw new NotImplementedException();
		}
	}


}
	