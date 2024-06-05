using System;
using System.Reflection;

namespace MegaPint.SerializeReferenceDropdown.Editor
{

public static class TypeUtils
{
    #region Public Methods

    public static Type ExtractTypeFromString(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
            return null;

        var splitFieldTypename = typeName.Split(' ');
        var assemblyName = splitFieldTypename[0];
        var subStringTypeName = splitFieldTypename[1];
        Assembly assembly = Assembly.Load(assemblyName);
        Type targetType = assembly.GetType(subStringTypeName);

        return targetType;
    }

    public static bool IsFinalAssignableType(Type type)
    {
        return type.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface;
    }

    #endregion
}

}
