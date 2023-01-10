namespace GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions
{
	public class RationAlgorithmException : GripOpGras2Exception
	{
		public RationAlgorithmException() : base("An error occurred during the preparation of the feed ration.")
		{
		}

		public RationAlgorithmException(string message) : base(
			$"An error occurred during the preparation of the feed ration: {message}.")
		{
		}
	}
	public class NoProductsWithPossibleREException : RationAlgorithmException
	{
		public NoProductsWithPossibleREException() : base("No products with possible RE. Make sure to add products with different RE values")
		{
		}

		public NoProductsWithPossibleREException(string message) : base(
			$"The feed ration cannot be made because there are some products missing: {message}.")
		{
		}
	}

	public class NoPossibleRENaturalProductGroupsException : RationAlgorithmException
	{
		public NoPossibleRENaturalProductGroupsException() : base("It is not possible to make a feedproduct group with the targeted RE value. Make sure to add products with different RE values")
		{
		}


		public NoPossibleRENaturalProductGroupsException(string message) : base(
			$"It is not possible to make a feedproduct group with the targeted RE value of: {message}. Make sure to add products with different RE values")
		{
		}
	}
}