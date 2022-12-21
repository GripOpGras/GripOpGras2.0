using GripOpGras2.Domain;

namespace GripOpGras2.Plugins.PluginInterfaces.RepositoryInterfaces
{
	public interface IFarmsRepository
	{
		public Task<IEnumerable<Farm>> GetFarms();
	}
}
