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

		private bool isSupplementaryFeedProduct;

		public MappedFeedProduct(FeedProduct feedProduct, float REtarget = 150)
		{
			if (feedProduct.FeedAnalysis == null) throw new GripOpGras2Exception("FeedAnalysis cannot be null");
			if (feedProduct.FeedAnalysis.VEM == null) throw new GripOpGras2Exception("VEM cannot be null");
			if (feedProduct.FeedAnalysis.RE == null) throw new GripOpGras2Exception("RE cannot be null");
			isSupplementaryFeedProduct = (feedProduct.GetType() == typeof(SupplementaryFeedProduct));
			KGDMperVEM = (float)(1f/feedProduct.FeedAnalysis.VEM);
			KGDMPerVEM_bijprod = (isSupplementaryFeedProduct) ? KGDMperVEM : 0;
			REdiffPerVEM = (float)(feedProduct.FeedAnalysis.RE - 150);
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
		private float _currentREtarget;

		private IReadOnlyList<(AbstractMappedFoodItem FoodItem, float partOfVemInGroup)> sourceproducts;

		/// <summary>
		/// Constructor for a combination of multiple products. Takes a list with products and how much VEM is in .
		/// </summary>
		/// <param name="products">A list with tuples of 1. another abstractmappedFeedItem and 2. a number that represents the amount in VEM in comparison to the other products. </param>
		/// <exception cref="NotImplementedException"></exception>
		public MappedFeedProductGroup(params (AbstractMappedFoodItem FoodItem, float partOfGroupInVEM)[] products)
		{
			//TODO check if the group contains only 1 product, if so, return that product instead of a group
			float totalVEM = products.Sum(x => x.partOfGroupInVEM);
			KGDMperVEM = (float)(products.Sum(x => x.FoodItem.KGDMperVEM * x.partOfGroupInVEM)) / totalVEM;
			KGDMPerVEM_bijprod = products.Sum(x => x.FoodItem.KGDMPerVEM_bijprod * x.partOfGroupInVEM) / totalVEM;
			REdiffPerVEM = products.Sum(x => x.FoodItem.REdiffPerVEM * x.partOfGroupInVEM) / totalVEM;
			REdiffPerVEM_bijprod = products.Sum(x => x.FoodItem.REdiffPerVEM_bijprod * x.partOfGroupInVEM) / totalVEM;
			partOfTotalVEMbijprod = products.Sum(x => x.FoodItem.partOfTotalVEMbijprod * x.partOfGroupInVEM) / totalVEM;
			setAppliedVEM(0);
			originalRefference = this;
			//fixing the partOfGroupInVem so that it combines to a maximum of 1
			List<(AbstractMappedFoodItem FoodItem, float partOfGroupInVEM)> productsFixedList = new();
			foreach ((AbstractMappedFoodItem FoodItem, float partOfGroupInVEM) product in products.ToList())
			{
				productsFixedList.Add((FoodItem: product.FoodItem,
					partOfGroupInVEM: (product.partOfGroupInVEM / totalVEM)));
			}

			sourceproducts = productsFixedList;
		}

		//Returns a list of products and the amount of KGDM that is needed. When the soruce gives a product that is allrady listed, it will be combined.
		public override List<Tuple<FeedProduct, float>> GetProducts()
		{
			List<Tuple<FeedProduct, float>> products = new();
			foreach ((AbstractMappedFoodItem FoodItem, float partOfGroupInVEM) product in
			         sourceproducts) //sourceproducts. use this PartOfGroupInVem.
			{
				foreach (Tuple<FeedProduct, float> product2 in product.FoodItem.GetProducts())
				{
					float amountToBeAdded = product.FoodItem.KGDMperVEM * appliedVEM * product.partOfGroupInVEM;
					if (products.Any(x => x.Item1 == product2.Item1))
					{
						amountToBeAdded = products.First(x => x.Item1 == product2.Item1).Item2 + amountToBeAdded;
						products.Remove(products.First(x => x.Item1 == product2.Item1));
					}

					products.Add(new Tuple<FeedProduct, float>(product2.Item1, amountToBeAdded));

				}
			}

			return products;
		}

		public override AbstractMappedFoodItem Clone()
		{
			MappedFeedProductGroup newMappedFeedProductGroup = new MappedFeedProductGroup(sourceproducts.ToArray());
			newMappedFeedProductGroup.setAppliedVEM(appliedVEM);
			newMappedFeedProductGroup.originalRefference = originalRefference;
			return newMappedFeedProductGroup;
		}
	}

}
	