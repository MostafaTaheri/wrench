using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Wrench.StreamTransaction.Helpers;

internal class TypeHelper
{
    private string exceptTypesRegex = "(System.*|Microsoft.*)";
    private readonly WeakReference<IList<Type>> _types = new WeakReference<IList<Type>>(null);
    public WeakReference<IList<Type>> Types1 => _types;

    public Type? GetType(string typeFullName)
    {
        return Types.FirstOrDefault(type => type.FullName == typeFullName);
    }

    private IEnumerable<Type> Types
    {
        get
        {
            IList<Type> types;

            if (!_types.TryGetTarget(out types))
            {
                types = new List<Type>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    string assemblyName = assembly.GetName().FullName;

                    if (!assembly.IsDynamic)
                    {
                        Type?[]? exportedTypes = null;

                        try
                        {
                            exportedTypes = assembly.GetExportedTypes();
                        }
                        catch (ReflectionTypeLoadException e)
                        {
                            exportedTypes = e.Types;
                        }

                        if (exportedTypes != null)
                        {
                            foreach (Type? exportedType in exportedTypes)
                            {
                                if (string.IsNullOrEmpty(exportedType?.FullName))
                                    throw new NullReferenceException();

                                if (!Regex.IsMatch(exportedType.FullName, exceptTypesRegex))
                                    types.Add(exportedType);
                            }
                        }
                    }
                }

                _types.SetTarget(types);
            }

            return types;
        }
    }

}
