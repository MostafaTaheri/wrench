using System;
using EasyNetQ;
using Wrench.StreamTransaction.Helpers;


namespace Wrench.StreamTransaction.Services;

public class CustomTypeNameSerializer : ITypeNameSerializer
{
    public string? Serialize(Type type)
    {
        return type?.FullName;
    }

    public Type? DeSerialize(string typeName)
    {
        try
        {
            var typeHelper = new TypeHelper();
            
            return typeHelper.GetType(typeName);
        }
        catch { }

        return null;
    }
}