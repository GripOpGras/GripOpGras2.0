using GripOpGras2.Domain;

namespace GripOpGras2.Plugins.PluginInterfaces.RepositoryInterfaces
{
	public interface IPlotRepository
	{
		public Task<IEnumerable<Plot>> GetPlots(string farmId);
	}
}
