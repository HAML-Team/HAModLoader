using System;
using System.Reflection;

public static class ModReflectionUtils
{
    public static void SafeInvoke(object instance, string methodName, params object[] args)
    {
        var type = instance.GetType();
        var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null) return;
        try { method.Invoke(instance, args); }
        catch (Exception ex) { UnityEngine.Debug.LogError($"[ReflectionUtils] Error invoking {type.Name}.{methodName}: {ex}"); }
    }
}