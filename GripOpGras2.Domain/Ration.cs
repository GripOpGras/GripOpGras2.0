namespace GripOpGras2.Domain
{
	public class FeedRation
	{
		public Herd? Herd { get; set; }
		public DateTime Date { get; set; }
		public Plot? Plot { get; set; }
		/// <summary>
		/// The value is in kg roughage.
		/// </summary>
		public Dictionary<Roughage, float>? Roughages { get; set; }
	}
}
