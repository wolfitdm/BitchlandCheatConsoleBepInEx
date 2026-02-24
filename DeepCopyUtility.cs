using System;
using System.Reflection;
using UnityEngine;

public static class DeepCopyUtility
{
    /// <summary>
    /// Creates a deep copy of an object, including inherited members.
    /// UnityEngine.Object types are cloned using Object.Instantiate.
    /// </summary>
    public static T DeepCopy<T>(T original)
    {
        return (T)DeepCopyInternal(original, new System.Collections.Generic.Dictionary<object, object>());
    }

    private static object DeepCopyInternal(object original, System.Collections.Generic.Dictionary<object, object> visited)
    {
        if (original == null)
            return null;

        var type = original.GetType();

        // Prevent infinite recursion for cyclic references
        if (visited.ContainsKey(original))
            return visited[original];

        // Handle Unity objects separately
        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            var clonedUnityObj = UnityEngine.Object.Instantiate(original as UnityEngine.Object);
            visited[original] = clonedUnityObj;
            return clonedUnityObj;
        }

        // Handle primitive types, strings, enums
        if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal))
            return original;

        // Handle arrays
        if (type.IsArray)
        {
            var array = (Array)original;
            var elementType = type.GetElementType();
            var copiedArray = Array.CreateInstance(elementType, array.Length);
            visited[original] = copiedArray;

            for (int i = 0; i < array.Length; i++)
                copiedArray.SetValue(DeepCopyInternal(array.GetValue(i), visited), i);

            return copiedArray;
        }

        // Create instance without calling constructor
        var clone = Activator.CreateInstance(type, true);
        visited[original] = clone;

        // Copy all fields (including private & inherited)
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var fieldValue = field.GetValue(original);
            var copiedValue = DeepCopyInternal(fieldValue, visited);
            field.SetValue(clone, copiedValue);
        }

        return clone;
    }
}