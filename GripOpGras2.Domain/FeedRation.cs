namespace GripOpGras2.Domain
{
	public class FeedRation
	{
		public Herd? Herd { get; set; }

		public DateTime Date { get; set; }

		public Plot? Plot { get; set; }

		/// <summary>
		/// The value is in kg dm roughage.
		/// </summary>
		public Dictionary<Roughage, float>? Roughages { get; set; }

		/// <summary>
		/// In kg dm.
		/// </summary>
		public float? GrassIntake { get; set; }
	}
}