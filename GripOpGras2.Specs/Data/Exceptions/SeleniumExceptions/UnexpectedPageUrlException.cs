namespace GripOpGras2.Specs.Data.Exceptions.SeleniumExceptions
{
	internal class UnexpectedPageUrlException : SeleniumException
	{
		public UnexpectedPageUrlException(string pageUrl, string nameOfExpectedPage) : base(
			$"The application did not navigate to the {nameOfExpectedPage}. The current page url is: {pageUrl}")
		{
		}
	}
}