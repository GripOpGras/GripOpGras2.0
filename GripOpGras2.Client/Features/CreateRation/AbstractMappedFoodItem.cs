using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public abstract class AbstractMappedFoodItem
	{

		protected float KGDMperVEM { get; }

		protected float KGDMbijprodPerVEM { get; }

		protected float REdiffPerVEM { get; }

		public float appliedVEM { get; private set; }

		public float appliedKGDM { get; private set; }

		public float appliedREdiff { get; private set; }

		public abstract List<Tuple<FeedProduct, float>> GetProducts();


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
	}


}
	