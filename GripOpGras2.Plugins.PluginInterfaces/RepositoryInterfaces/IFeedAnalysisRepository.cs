using GripOpGras2.Domain;

namespace GripOpGras2.Plugins.PluginInterfaces.RepositoryInterfaces
{
	public interface IFeedAnalysisRepository
	{
		public Task<IEnumerable<FeedAnalysis>> GetFeedAnalyses();

		public Task AddFeedAnalysis(FeedAnalysis feedAnalysis);
	}
}