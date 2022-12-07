namespace GripOpGras2.Domain
{
	public class GrazingActivity : GrasslandCalendarActivity
	{
		public Herd? Herd { get; set; }

		public Plot? Plot { get; set; }
	}
}