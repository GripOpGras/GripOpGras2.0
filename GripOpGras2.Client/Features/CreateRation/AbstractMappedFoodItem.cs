using GripOpGras2.Client.Data.Exceptions;
using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Client.Features.CreateRation
{
	public abstract class AbstractMappedFoodItem
	{
		public float KgdMperVem { get; protected set; }

		public float KgdmSupplementaryFeedProductPerVem { get; protected set; }

		public float REdiffPerVem { get; protected set; }

		public float REdiffPerVemSupplementaryFeedProduct { get; protected set; }

		public float REperVem { get; protected set; }

		public float REperVemSupplmenteryFeedProduct { get; protected set; }

		public float AppliedVem { get; private set; }

		public float AppliedKgdm { get; private set; }

		public float AppliedREdiff { get; private set; }

		public float AppliedTotalRe { get; private set; }

		//Gives a number between 1 and 0, which reperesents the percentage of VEM that is bijproduct
		public float SupplmenteryPartOfTotalVem { get; protected set; }

		//reference to original
		public AbstractMappedFoodItem OriginalReference { get; protected set; } = null!;

		public abstract Dictionary<FeedProduct, float> GetProducts();

		public abstract AbstractMappedFoodItem Clone();


		public void SetAppliedVem(float vem)
		{
			AppliedVem = vem;
			AppliedKgdm = vem * KgdMperVem;
			AppliedREdiff = vem * REdiffPerVem;
			AppliedTotalRe = vem * REperVem;
		}

		public string GetProductsForConsole()
		{
			Dictionary<FeedProduct, float> products = GetProducts();

			return products.Aggregate("",
					(current, product) =>
						current + $"\t - Product: {product.Key.Name + ",",-20} {product.Value,5} KG DM")
				.TrimEnd();
		}
	}

	public class MappedFeedProduct : AbstractMappedFoodItem
	{
		private readonly FeedProduct _containingFeedProduct;

		private readonly bool _isSupplementaryFeedProduct;

		public MappedFeedProduct(FeedProduct feedProduct, float reTarget = 150)
		{
			if (feedProduct.FeedAnalysis == null) throw new GripOpGras2Exception("FeedAnalysis cannot be null");
			if (feedProduct.FeedAnalysis.Vem == null) throw new GripOpGras2Exception("VEM cannot be null");
			if (feedProduct.FeedAnalysis.Re == null) throw new GripOpGras2Exception("RE cannot be null");
			_isSupplementaryFeedProduct = feedProduct.GetType() == typeof(SupplementaryFeedProduct);
			KgdMperVem = (float)(1f / feedProduct.FeedAnalysis.Vem);
			KgdmSupplementaryFeedProductPerVem = _isSupplementaryFeedProduct ? KgdMperVem : 0;
			REperVem = (float)(feedProduct.FeedAnalysis.Re / feedProduct.FeedAnalysis.Vem);
			REperVemSupplmenteryFeedProduct = _isSupplementaryFeedProduct ? REperVem : 0;
			REdiffPerVem = (float)((feedProduct.FeedAnalysis.Re - reTarget) / feedProduct.FeedAnalysis.Vem);
			REdiffPerVemSupplementaryFeedProduct = _isSupplementaryFeedProduct ? REdiffPerVem : 0;
			SupplmenteryPartOfTotalVem = _isSupplementaryFeedProduct ? 1 : 0;
			SetAppliedVem(0);
			OriginalReference = this;
			_containingFeedProduct = feedProduct;
		}


		public override Dictionary<FeedProduct, float> GetProducts()
		{
			return new Dictionary<FeedProduct, float> { { _containingFeedProduct, AppliedKgdm } };
		}


		public override AbstractMappedFoodItem Clone()
		{
			MappedFeedProduct newMappedFeedProduct = new(_containingFeedProduct);
			newMappedFeedProduct.SetAppliedVem(AppliedVem);
			newMappedFeedProduct.OriginalReference = OriginalReference;
			return newMappedFeedProduct;
		}
	}

	public class MappedFeedProductGroup : AbstractMappedFoodItem
	{
		private readonly IReadOnlyList<(AbstractMappedFoodItem FoodItem, float partOfVemInGroup)> _sourceProducts;

		/// <summary>
		///     Constructor for a combination of multiple products. Takes a list with products and how much VEM is in .
		/// </summary>
		/// <param name="products">
		///     A list with tuples of 1. another abstractmappedFeedItem and 2. a number that represents the
		///     amount in VEM in comparison to the other products.
		/// </param>
		/// <exception cref="NotImplementedException"></exception>
		public MappedFeedProductGroup(params (AbstractMappedFoodItem FoodItem, float partOfGroupInVEM)[] products)
		{
			//TODO check if the group contains only 1 product, if so, return that product instead of a group
			float totalVem = products.Sum(x => x.partOfGroupInVEM);
			KgdMperVem = products.Sum(x => x.FoodItem.KgdMperVem * x.partOfGroupInVEM) / totalVem;
			KgdmSupplementaryFeedProductPerVem =
				products.Sum(x => x.FoodItem.KgdmSupplementaryFeedProductPerVem * x.partOfGroupInVEM) / totalVem;
			REperVem = products.Sum(x => x.FoodItem.REperVem * x.partOfGroupInVEM) / totalVem;
			REperVemSupplmenteryFeedProduct =
				products.Sum(x => x.FoodItem.REperVemSupplmenteryFeedProduct * x.partOfGroupInVEM) / totalVem;
			REdiffPerVem = products.Sum(x => x.FoodItem.REdiffPerVem * x.partOfGroupInVEM) / totalVem;
			REdiffPerVemSupplementaryFeedProduct =
				products.Sum(x => x.FoodItem.REdiffPerVemSupplementaryFeedProduct * x.partOfGroupInVEM) / totalVem;
			SupplmenteryPartOfTotalVem = products.Sum(x => x.FoodItem.SupplmenteryPartOfTotalVem * x.partOfGroupInVEM) /
			                             totalVem;
			SetAppliedVem(0);
			OriginalReference = this;
			//fixing the partOfGroupInVem so that it combines to a maximum of 1
			List<(AbstractMappedFoodItem FoodItem, float partOfGroupInVEM)> productsFixedList = new();
			foreach ((AbstractMappedFoodItem? foodItem, float partOfGroupInVem) in products.ToList())
				productsFixedList.Add((FoodItem: foodItem,
					partOfGroupInVEM: partOfGroupInVem / totalVem));

			_sourceProducts = productsFixedList;
		}

		//Returns a list of products and the amount of KGDM that is needed. When the soruce gives a product that is allrady listed, it will be combined.
		public override Dictionary<FeedProduct, float> GetProducts()
		{
			Dictionary<FeedProduct, float> products = new();
			foreach ((AbstractMappedFoodItem? foodItem, float partOfGroupInVem) in
			         _sourceProducts) //sourceproducts. use this PartOfGroupInVem.
			{
				AbstractMappedFoodItem productCustomized = foodItem.Clone();
				productCustomized.SetAppliedVem(partOfGroupInVem * AppliedVem);

				foreach (KeyValuePair<FeedProduct, float> item in productCustomized.GetProducts())
					if (products.ContainsKey(item.Key))
						products[item.Key] += item.Value;
					else
						products.Add(item.Key, item.Value);
			}

			return products;
		}

		public override AbstractMappedFoodItem Clone()
		{
			MappedFeedProductGroup newMappedFeedProductGroup = new(_sourceProducts.ToArray());
			newMappedFeedProductGroup.SetAppliedVem(AppliedVem);
			newMappedFeedProductGroup.OriginalReference = OriginalReference;
			return newMappedFeedProductGroup;
		}
	}
}