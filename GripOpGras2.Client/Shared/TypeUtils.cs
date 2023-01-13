using System.Reflection;

namespace GripOpGras2.Client.Shared
{
	public static class TypeUtils
	{
		/// <summary>
		/// Creates an instance of the specified type.
		/// The type must have a parameterless constructor.
		/// </summary>
		/// <typeparam name="T">The object that needs to be created.</typeparam>
		/// <param name="fullyQualifiedName">The name of the class with its namespace.</param>
		/// <returns>Ab instance of the specified type</returns>
		public static T? CreateInstance<T>(string fullyQualifiedName)
		{
			Type? type = Type.GetType(fullyQualifiedName);
			if (type != null)
			{
				return (T)Activator.CreateInstance(type);
			}
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				type = asm.GetType(fullyQualifiedName);
				if (type != null)
				{
					return (T)Activator.CreateInstance(type);
				}
			}
			return default;
		}
	}
}
