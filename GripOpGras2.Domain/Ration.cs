using GripOpGras2.Domain;

namespace GripOpGras2.Models
{
	public class FeedRation
	{
		public Herd? Herd { get; set; }
		public DateTime Date { get; set; }
		public Plot? Plot { get; set; }
		public Dictionary<Roughage, float>? Roughages { get; set; }
	}
}
