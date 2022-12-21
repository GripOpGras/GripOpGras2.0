using GripOpGras2.Domain;

namespace GripOpGras2.Plugins.PluginInterfaces.RepositoryInterfaces
{
	public interface IFarmRepository
	{
		public Task<IEnumerable<Farm>> GetFarms();
	}
}
