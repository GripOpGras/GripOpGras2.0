using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace GripOpGras2.Client.Features.LoginToFarmMaps
{
	/// <summary>
	/// A MessageHandler that attaches the FarmMaps OIDC access token to outgoing HTTP requests that target the FarmMaps domains.
	/// </summary>
	public class FarmMapsAuthorizationMessageHandler : AuthorizationMessageHandler
	{
		public FarmMapsAuthorizationMessageHandler(IAccessTokenProvider provider,
			NavigationManager navigationManager, IConfiguration configuration)
			: base(provider, navigationManager)
		{
			string farmMapsBaseUrl = configuration.GetValue<string>("FarmMapsBaseUrl");
			string farmMapsAccountsUrl = configuration.GetValue<string>("FarmMapsAccountBaseUrl");

			ConfigureHandler(
				authorizedUrls: new[] { farmMapsBaseUrl, farmMapsAccountsUrl }
			);
		}
	}
}