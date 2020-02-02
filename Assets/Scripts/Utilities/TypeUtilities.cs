using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class TypeUtilities
{
	// This function returns all the inherited classes of the base class
	// This function is very very slow please use only at load times
	public static List<System.Type> GetInheritedClassesTypes(System.Type baseType)
	{
		System.Type[] types = Assembly.GetCallingAssembly().GetTypes();
		
		//Debug.Log("Types count = " + types.Length);
		
		List<System.Type> newList = new List<System.Type>();
		
		for( int i = 0 ; i < types.Length ; i++ )
		{
			if( types[i].IsSubclassOf(baseType) == true )
			{
				newList.Add(types[i]);
			}
		}
		//types.
		//return .Where(type => type.IsSubclassOf(baseType)).ToList();
		return newList;
	}
}
