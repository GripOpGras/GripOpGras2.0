using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace GripOpGras2.Client.Features.FarmMapsLogin
{
	/// <summary>
	/// A MessageHandler that attaches the FarmMaps OIDC access token to outgoing HTTP requests that target the FarmMaps domains.
	/// </summary>
	public class FarmMapsAuthorizationMessageHandler : AuthorizationMessageHandler
	{
		public FarmMapsAuthorizationMessageHandler(IAccessTokenProvider provider,
			NavigationManager navigationManager)
			: base(provider, navigationManager)
		{
			// TODO set these urls in appsetting.json
			ConfigureHandler(
				authorizedUrls: new[] { "https://accounts.test.farmmaps.eu", "https://test.farmmaps.eu" }
			);
		}
	}
}