using GripOpGras2.Domain;

namespace GripOpGras2.Client.Features.CreateRation
{
	public class TargetValues
	{
		/// <summary>
		///     The amount of VEM that a cow needs a day, if the cow doesn't product any milk
		/// </summary>
		public const float DefaultVemNeedsOfCow = 5500;

		/// <summary>
		///     The amount of VEM that needs to be added per L Meetmilk produced.
		/// </summary>
		public const float VemNeedsPerLiterMilk = 450;

		/// <summary>
		///     The amount of KG DM Supplementary Feed Products that should be given per cow per day.
		/// </summary>
		public const float MaxAmountOfSupplementaryFeedProductInKgPerCow = 4.5f;

		/// <summary>
		///     The maximum amount of KG DM Feed products (inc grass) that a cow should be able to eat a day.
		///     #TODO make this more dynamic. This should be an absolute maximum and in the feature be dynamic, based on the data
		///     of the cows, like their body weight. Taiga: #198
		/// </summary>
		public const float MaxKgDmIntakePerCow = 20;

		/// <summary>
		///     The VEM should be between 100%-110% of what the cows should actually eat. 105% is in between of that
		/// </summary>
		public const float OptimalVemCoverage = 1.05f;

		/// <summary>
		///     The RE has an optimum of 150 RE per KG DM
		/// </summary>
		public const float OptimalReCoverageInGramsPerKgDm = 150;

		/// <summary>
		///     When the RE needs to differ fromt e optimum, the max RE/KGDM shoud be 170.
		///     TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet
		///     worden gewerkt
		/// </summary>
		public const float MaxReCoverageInGramsPerKgDm = 170;

		/// <summary>
		///     When the RE needs to differ from te optimum, the  RE/KGDM shoud not go below 140 .
		///     TODO alleen deze gebruiken wanneer bij de uitkomsten is gebleken dat 150 niet te doen is, dus dat er opnieuw moet
		///     worden gewerkt
		/// </summary>
		public const float MinReCoverageInGramsPerKgDm = 140;

		private readonly Herd _herd;

		private readonly MilkProductionAnalysis _milkProductionAnalysis;

		/// <summary>
		///     A class that's a centeral place while making the Rationalgorithm to refer to when values are needed that determine
		///     how much nutrients should be applied.
		/// </summary>
		/// <param name="herd">Herd class required for the amount of cows</param>
		/// <param name="milkProductionAnalysis">milkProductionAnalysis required to determine the VEM needed</param>
		/// <param name="targetedREcoveragePerKgDm">
		///     optional. how much RE should be applied per VEM (can differ while the algorithm
		///     is running)
		/// </param>
		/// <param name="targetedMaxAmountOfSupplementaryFeedProductInKgPerCow">
		///     optional. max KGDM Supplementary feedproduct per
		///     cow.
		/// </param>
		/// <param name="targetedMaxKgDmIntakePerCow">optional. set the max KG DM intake per cow by hand, for testing purposes.</param>
		public TargetValues(Herd herd,
			MilkProductionAnalysis milkProductionAnalysis,
			float targetedREcoveragePerKgDm = OptimalReCoverageInGramsPerKgDm,
			float targetedMaxAmountOfSupplementaryFeedProductInKgPerCow = MaxAmountOfSupplementaryFeedProductInKgPerCow,
			float targetedMaxKgDmIntakePerCow = MaxKgDmIntakePerCow)
		{
			_herd = herd;
			_milkProductionAnalysis = milkProductionAnalysis;
			TargetedREcoveragePerKgDm = targetedREcoveragePerKgDm;
			TargetedMaxKgDmSupplementaryFeedProductPerCow = targetedMaxAmountOfSupplementaryFeedProductInKgPerCow;
			TargetedMaxKgDmIntakePerCow = targetedMaxKgDmIntakePerCow;
			TargetedVem =
				(_milkProductionAnalysis.Amount * VemNeedsPerLiterMilk + DefaultVemNeedsOfCow * herd.NumberOfAnimals) *
				OptimalVemCoverage;
		}

		public float TargetedREcoveragePerKgDm { get; set; } = OptimalReCoverageInGramsPerKgDm;

		public float TargetedMaxKgDmSupplementaryFeedProductPerCow { get; set; } =
			MaxAmountOfSupplementaryFeedProductInKgPerCow;

		public float TargetedMaxKgDmIntakePerCow { get; set; } = MaxKgDmIntakePerCow;

		public float TargetedVemPerCow => TargetedVem / _herd.NumberOfAnimals;

		public float TargetedMaxKgDm => TargetedMaxKgDmIntakePerCow * _herd.NumberOfAnimals;

		public float TargetedMaxKgDmSupplementaryFeedProduct =>
			TargetedMaxKgDmSupplementaryFeedProductPerCow * _herd.NumberOfAnimals;

		public float TargetedVem { get; set; }
	}
}