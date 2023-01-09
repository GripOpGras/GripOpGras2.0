namespace GripOpGras2.Specs.Data.Exceptions.SpecFlowTestExceptions
{
	internal class MissingUserSecretsException : SpecFlowTestException
	{
		public MissingUserSecretsException() : base("The FarmMapsTestAccount variable in the secrets.json file has not been configured. See the following link on how to configure this file: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows")
		{
		}
	}
}