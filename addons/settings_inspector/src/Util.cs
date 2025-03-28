using System;
using System.Collections.Generic;
using System.Linq;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src;

public static class Util
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

    public static bool IsSet(this LayoutFlags flags, LayoutFlags flag)
    {
        return (flags & flag) != 0;
    }

    public static LayoutFlags Set(this LayoutFlags flags, LayoutFlags flag, bool value)
    {
        return value ? flags | flag : flags & ~flag;
    }
}