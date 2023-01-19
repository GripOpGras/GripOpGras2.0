namespace GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions
{
	public class NoPossibleReNaturalProductGroupsException : RationAlgorithmException
	{
		public NoPossibleReNaturalProductGroupsException() : base(
			"It is not possible to make a feedproduct group with the targeted RE value. Make sure to add products with different RE values")
		{
		}

		public NoPossibleReNaturalProductGroupsException(string message) : base(
			$"It is not possible to make a feedproduct group with the targeted RE value of: {message}. Make sure to add products with different RE values")
		{
		}
	}
}