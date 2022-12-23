namespace GripOpGras2.Client.Data.Exceptions
{
	public class NoCowsInTheHerdException : GripOpGras2Exception
	{
		public NoCowsInTheHerdException() : base("The herd doesn't contain any cows.")
		{
		}
	}
}