namespace GripOpGras2.Domain
{
	public class FeedAnalysis
	{
		public string? Description { get; set; }

		public DateTime Date { get; set; }

		/// <summary>
		/// In g/kg product.
		/// </summary>
		public float DryMatter { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Re { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Rvet { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Sui { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Zet { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Rc { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Ndf { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Adf { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Adl { get; set; }

		/// <summary>
		/// In /kg dm.
		/// </summary>
		public float? Vem { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Dve07 { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Oeb07 { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Fos { get; set; }

		/// <summary>
		/// In g/kg dm.
		/// </summary>
		public float? Ras { get; set; }
	}
}