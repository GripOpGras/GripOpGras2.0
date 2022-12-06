namespace GripOpGras2.Domain
{
	public class MilkProductionAnalysis
	{
		public DateTime Date { get; set; }

		/// <summary>
		/// The total amount of milk produced in liters.
		/// </summary>
		public float Amount { get; set; }
	}
}