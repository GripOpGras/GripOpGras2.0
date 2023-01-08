using System.Runtime.CompilerServices;
using GripOpGras2.Client.Data.Exceptions;
using GripOpGras2.Domain;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public abstract class AbstractMappedFoodItem
	{

		public float KGDMperVEM { get; protected set; }

		public float KGDMPerVEM_bijprod { get; protected set; }

		public float REdiffPerVEM { get; protected set; }
		public float REdiffPerVEM_bijprod { get; protected set; }

		public float appliedVEM { get; private set; } = 0;

		public float appliedKGDM { get; private set; }

		public float appliedREdiff { get; private set; }

		//Gives a number between 1 and 0, which reperesents the percentage of VEM that is bijproduct
		public float partOfTotalVEMbijprod { get; protected set; }

		public abstract List<Tuple<FeedProduct, float>> GetProducts();

		public abstract AbstractMappedFoodItem Clone();
		//reference to original
		public AbstractMappedFoodItem originalRefference { get; protected set; } = null!;


		public void setAppliedVEM(float VEM)
		{
			appliedVEM = VEM;
			appliedKGDM = VEM * KGDMperVEM;
			appliedREdiff = VEM * REdiffPerVEM;
		}
	}

	public class MappedFeedProduct : AbstractMappedFoodItem
	{
		FeedProduct containingFeedProduct;

		public MappedFeedProduct(FeedProduct feedProduct)
		{
			if (feedProduct.FeedAnalysis == null) throw new GripOpGras2Exception("FeedAnalysis cannot be null");
			if (feedProduct.FeedAnalysis.VEM == null) throw new GripOpGras2Exception("VEM cannot be null");
			if (feedProduct.FeedAnalysis.RE == null) throw new GripOpGras2Exception("RE cannot be null");
			bool isSupplementaryFeedProduct = (feedProduct.GetType() == typeof(SupplementaryFeedProduct));
			KGDMperVEM = (float)(1f/feedProduct.FeedAnalysis.VEM);
			KGDMPerVEM_bijprod = (isSupplementaryFeedProduct) ? KGDMperVEM : 0;
			REdiffPerVEM = (float)(feedProduct.FeedAnalysis.RE - 150); //gewenste RE is 150
			REdiffPerVEM_bijprod = (isSupplementaryFeedProduct) ? REdiffPerVEM : 0;
			partOfTotalVEMbijprod = (isSupplementaryFeedProduct) ? 1 : 0;
			setAppliedVEM(0);
			originalRefference = this;
			containingFeedProduct = feedProduct;
		}

		public override List<Tuple<FeedProduct, float>> GetProducts()
		{
			return new List<Tuple<FeedProduct, float>>() { new Tuple<FeedProduct, float>(containingFeedProduct, appliedKGDM) };
		}

		public override AbstractMappedFoodItem Clone()
		{
			MappedFeedProduct newMappedFeedProduct = new MappedFeedProduct(containingFeedProduct);
			newMappedFeedProduct.setAppliedVEM(appliedVEM);
			newMappedFeedProduct.originalRefference = originalRefference;
			return newMappedFeedProduct;
		}
	}

	public class MappedFeedProductGroup : AbstractMappedFoodItem
	{
		/// <summary>
		/// Constructor for a combination of multiple products. Takes a list with products and how much VEM is in .
		/// </summary>
		/// <param name="products">A list with tuples of 1. another abstractmappedFeedItem and 2. a number that represents the amount in VEM in comparison to the other products. </param>
		/// <exception cref="NotImplementedException"></exception>
		public MappedFeedProductGroup(params (AbstractMappedFoodItem FoodItem, float partOfGroupInVEM)[] products)
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
	