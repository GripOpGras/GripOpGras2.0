using GripOpGras2.Domain;
using GripOpGras2.Plugins.FarmMaps.QuickType.Cropfield;
using GripOpGras2.Plugins.PluginInterfaces.RepositoryInterfaces;

namespace GripOpGras2.Plugins.FarmMaps
{
	public class PlotRepository : IPlotRepository
	{
		private readonly HttpClient _httpClient;
		private const string ApiUriPart1 = "/api/v1/items/";
		private const string ApiUriPart2 = "/children?it=vnd.farmmaps.itemtype.cropfield";

		public PlotRepository(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<IEnumerable<Plot>> GetPlots(string farmId)
		{
			string jsonString = await _httpClient.GetStringAsync(ApiUriPart1 + farmId + ApiUriPart2);
			FarmMapsCropfield[] farmMapsCropfields = FarmMapsCropfield.FromJson(jsonString);
			return farmMapsCropfields.Select(cropfield => new Plot
			{
				//TODO add the other fields
				Name = cropfield.Name,
			});
		}
	}
}
