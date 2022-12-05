namespace GripOpGras2.Domain
{
	public class Plot
	{
		public string? Name { get; set; }
		public DateTime LastMeasured { get; set; }
		/// <summary>
		/// In hectares.
		/// </summary>
		public float Area { get; set; }
		/// <summary>
		/// In kg DM/ha.
		/// </summary>
		public float NetDryMatter { get; set; }
		/// <summary>
		/// In kg DM/ha.
		/// </summary>
		public float DryMatterGrowth { get; set; }
		public FeedAnalysis? FeedAnalysis { get; set; }
	}
}