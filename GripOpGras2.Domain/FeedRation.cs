using GripOpGras2.Domain.FeedProducts;

namespace GripOpGras2.Domain
{
	public class FeedRation
	{
		public Herd? Herd { get; set; }

		public DateTime Date { get; set; }

		public Plot? Plot { get; set; }

		/// <summary>
		/// The value is in kg dm product.
		/// </summary>
		public Dictionary<FeedProduct, float>? FeedProducts { get; set; }

		/// <summary>
		/// In kg dm.
		/// </summary>
		public float? GrassIntake { get; set; }
	}
}