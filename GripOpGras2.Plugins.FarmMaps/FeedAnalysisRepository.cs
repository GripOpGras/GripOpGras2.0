using GripOpGras2.Domain;
using GripOpGras2.Plugins.PluginInterfaces.RepositoryInterfaces;

namespace GripOpGras2.Plugins.FarmMaps
{
	public class FeedAnalysisRepository : IFeedAnalysisRepository
	{
		private readonly HttpClient _httpClient;

		public FeedAnalysisRepository(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<IEnumerable<FeedAnalysis>> GetFeedAnalyses()
		{
			throw new NotImplementedException();
		}

		public async Task AddFeedAnalysis(FeedAnalysis feedAnalysis)
		{
			throw new NotImplementedException();
		}
	}
}