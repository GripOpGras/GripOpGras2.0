namespace GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions
{
	public class NoProductsWithPossibleReException : RationAlgorithmException
	{
		public NoProductsWithPossibleReException() : base(
			"No products with possible RE. Make sure to add products with different RE values")
		{
		}

		public NoProductsWithPossibleReException(string message) : base(
			$"The feed ration cannot be made because there are some products missing: {message}.")
		{
		}
	}
}