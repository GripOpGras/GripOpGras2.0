using GripOpGras2.Domain;
using GripOpGras2.Plugins.FarmMaps.QuickType;
using GripOpGras2.Plugins.PluginInterfaces.RepositoryInterfaces;

namespace GripOpGras2.Plugins.FarmMaps
{
	public class FarmsRepository : IFarmsRepository
	{
		private readonly HttpClient _httpClient;
		private const string ApiUri = "/api/v1/items?it=vnd.farmmaps.itemtype.croppingscheme";

		public FarmsRepository(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<IEnumerable<Farm>> GetFarms()
		{
			string jsonString = await _httpClient.GetStringAsync(ApiUri);
			FarmMapsCroppingscheme[] croppingscheme = FarmMapsCroppingscheme.FromJson(jsonString);
			return croppingscheme.Select(croppingscheme => new Farm
			{
				Name = croppingscheme.Name,
				Code = croppingscheme.Code
			});
		}
	}
}