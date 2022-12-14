namespace GripOpGras2.Domain.FeedProducts
{
	public abstract class FeedProduct
	{
		public string? Name { get; set; }

		public FeedAnalysis? FeedAnalysis { get; set; }

		public bool Available { get; set; }
	}
}