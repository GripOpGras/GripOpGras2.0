using GripOpGras2.Domain;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class TargetValues
	{
		/// <summary>
		/// The amount of VEM that a cow needs a day, if the cow doesn't product any milk
		/// </summary>
		public const float DefaultVEMNeedsOfCow = 5500;

		/// <summary>
		/// The amount of VEM that needs to be added per L Meetmilk produced.
		/// </summary>
		public const float VemNeedsPerLiterMilk = 450;

		/// <summary>
		/// The amount of KG DM Supplementery Feed Products that should be given per cow per day.
		/// </summary>
		public const float MaxAmountOfSupplementaryFeedProductInKGPerCow = 4.5f;

		/// <summary>
		/// The maximum amount of KG DM Feed products (inc grass) that a cow should be able to eat a day. 
		/// #TODO make this more dynamic. This should be an absolute maximum and in the feature be dynamic, based on the data of the cows, like their body weight. Taiga: #198
		/// </summary>
		public const float MaxKgDmIntakePerCow = 20;

		/// <summary>
		/// The VEM should be between 100%-110% of what the cows should actually eat. 105% is in between of that
		/// </summary>
		public const float OptimalVEMCoverage = 1.05f;

		/// <summary>
		/// The RE has an optimum of 150 RE per KG DM
		/// </summary>
		public const float OptimalRECoverageInGramsPerKgDm = 150;

		private Herd _herd;

		private MilkProductionAnalysis _milkProductionAnalysis;

		/// <summary>
		/// When the RE needs to differ fromt e optimum, the max RE/KGDM shoud be 170.
		/// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		/// </summary>
		public const float MaxRECoverageInGramsPerKgDm = 170;

		/// <summary>
		/// When the RE needs to differ from te optimum, the  RE/KGDM shoud not go below 140 .
		/// TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet worden gewerkt
		/// </summary>
		public const float MinRECoverageInGramsPerKgDm = 140;

		/// <summary>
		/// A class that's a centeral place while making the Rationalgorithm to refer to when values are needed that determine how much nutrients should be applied.
		/// </summary>
		/// <param name="herd">Herd class required for the amount of cows</param>
		/// <param name="milkProductionAnalysis">milkProductionAnalysis required to determine the VEM needed</param>
		/// <param name="targetedREcoveragePerKgDm">optional. how much RE should be applied per VEM (can differ while the algorithm is running)</param>
		/// <param name="targetedMaxAmountOfSupplementeryFeedProductInKgPerCow">optional. max KGDM supplementery feedproduct per cow.</param>
		/// <param name="targetedMaxKgDmIntakePerCow">optional. set the max KG DM intake per cow by hand, for testing purposes.</param>
		public TargetValues(Herd herd,
			MilkProductionAnalysis milkProductionAnalysis,
			float targetedREcoveragePerKgDm = OptimalRECoverageInGramsPerKgDm,
			float targetedMaxAmountOfSupplementeryFeedProductInKgPerCow = MaxAmountOfSupplementaryFeedProductInKGPerCow,
			float targetedMaxKgDmIntakePerCow = MaxKgDmIntakePerCow)
		{
			_herd = herd;
			_milkProductionAnalysis = milkProductionAnalysis;
			TargetedREcoveragePerKgDm = targetedREcoveragePerKgDm;
			TargetedMaxKgDmSupplementeryFeedProductPerCow = targetedMaxAmountOfSupplementeryFeedProductInKgPerCow;
			TargetedMaxKgDmIntakePerCow = targetedMaxKgDmIntakePerCow;
			TargetedVEM =
				(_milkProductionAnalysis.Amount * VemNeedsPerLiterMilk + DefaultVEMNeedsOfCow * herd.NumberOfAnimals) *
				OptimalVEMCoverage;
		}

		public float TargetedREcoveragePerKgDm { get; set; } = OptimalRECoverageInGramsPerKgDm;

		public float TargetedMaxKgDmSupplementeryFeedProductPerCow { get; set; } =
			MaxAmountOfSupplementaryFeedProductInKGPerCow;

		public float TargetedMaxKgDmIntakePerCow { get; set; } = MaxKgDmIntakePerCow;

		public float TargetedVEMPerCow => TargetedVEM / _herd.NumberOfAnimals;

		public float TargetedMaxKgDm => TargetedMaxKgDmIntakePerCow * _herd.NumberOfAnimals;

		public float TargetedMaxKgDmSupplementeryFeedProduct =>
			TargetedMaxKgDmSupplementeryFeedProductPerCow * _herd.NumberOfAnimals;

		public float TargetedVEM { get; set; }
	}
}