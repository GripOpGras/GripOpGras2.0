namespace GripOpGras2.Domain
{
	public class FeedAnalysis
	{
		public string? Description { get; set; }

		public DateTime Date { get; set; }

		/// <summary>
		///     In g/kg product.
		/// </summary>
		public float DryMatter { get; set; }

		/// <summary>
		///     In g/kg DM.
		/// </summary>
		public float? RE { get; set; }

		/// <summary>
		///     In g/kg DM.
		/// </summary>
		public float? RVET { get; set; }

		/// <summary>
		///     In g/kg DM.
		/// </summary>
		public float? SUI { get; set; }

		/// <summary>
		///     In g/kg DM.
		/// </summary>
		public float? ZET { get; set; }

		/// <summary>
		///     In g/kg DM.
		/// </summary>
		public float? RC { get; set; }

		/// <summary>
		///     In g/kg DM.
		/// </summary>
		public float? NDF { get; set; }

		/// <summary>
		///     In g/kg DM.
		/// </summary>
		public float? ADF { get; set; }

		/// <summary>
		///     In g/kg DM
		/// </summary>
		public float? ADL { get; set; }

		/// <summary>
		///     In /kg DM
		/// </summary>
		public float? VEM { get; set; }

		/// <summary>
		///     In g/kg DM
		/// </summary>
		public float? DVE07 { get; set; }

		/// <summary>
		///     In g/kg DM
		/// </summary>
		public float? OEB07 { get; set; }

		/// <summary>
		///     In g/kg DM
		/// </summary>
		public float? FOS { get; set; }

		/// <summary>
		///     In g/kg DM
		/// </summary>
		public float? RAS { get; set; }
	}
}