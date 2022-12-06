namespace GripOpGras2.Domain
{
	public class Grazing : GrasslandCalendarActivity
	{
		public Herd? Herd { get; set; }

		public Plot? Plot { get; set; }
	}
}