using System;
using System.Collections.Generic;
using System.Reflection;

public class TypeHelper
{
	public static Dictionary<string, Type> _fastTypes = new Dictionary<string, Type>(); 

	public static Type GetTypeByName(string name, string assemblyName)
	{
		if (_fastTypes.ContainsKey(name))
			return _fastTypes[name];

		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (!assembly.FullName.Contains(assemblyName))
				continue;

			foreach (Type type in assembly.GetTypes())
			{
				if (type.Name == name)
				{
					_fastTypes.Add(name, type);
					return type;
				}
			}
		}

		return null;
	}
}