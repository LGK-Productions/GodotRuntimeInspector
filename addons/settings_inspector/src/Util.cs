using System;
using System.Collections.Generic;
using System.Linq;

namespace SettingInspector.addons.settings_inspector.src;

public class Util
{
    public static IEnumerable<Type> GetAssignableTypes(Type type)
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
            .Where(p => p is { IsAbstract: false, IsInterface: false } && type.IsAssignableFrom(p));
    }
    
    public static bool TryCreateInstance(Type type, out object? instance)
    {
        instance = null;
        if (type == typeof(string))
        {
            instance = string.Empty;
            return true;
        }

        try
        {
            instance = Activator.CreateInstance(type);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}