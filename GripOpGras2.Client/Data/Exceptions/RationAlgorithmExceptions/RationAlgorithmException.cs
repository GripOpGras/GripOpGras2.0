namespace GripOpGras2.Client.Data.Exceptions.RationAlgorithmExceptions
{
	public class RationAlgorithmException : Exception
	{
		public RationAlgorithmException() : base("An error occurred during the preparation of the feed ration.")
		{
		}

		public RationAlgorithmException(string message) : base(
			$"An error occurred during the preparation of the feed ration: {message}.")
		{
		}
	}
}