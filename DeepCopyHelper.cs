using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class DeepCopyHelper
{
    /// <summary>
    /// Creates a deep copy of any object, including inherited members.
    /// </summary>
    public static T DeepCopy<T>(T original)
    {
        return (T)DeepCopyInternal(original, new Dictionary<object, object>(new ReferenceEqualityComparer()));
    }

    private static object DeepCopyInternal(object original, IDictionary<object, object> visited)
    {
        if (original == null)
            return null;

        Type typeToReflect = original.GetType();

        // Handle primitive, string, decimal, DateTime, etc.
        if (IsPrimitive(typeToReflect))
            return original;

        // Prevent circular reference infinite loop
        if (visited.ContainsKey(original))
            return visited[original];

        // Handle arrays
        if (typeToReflect.IsArray)
        {
            Type arrayType = typeToReflect.GetElementType();
            Array originalArray = (Array)original;
            Array copiedArray = Array.CreateInstance(arrayType, originalArray.Length);
            visited.Add(original, copiedArray);

            for (int i = 0; i < originalArray.Length; i++)
                copiedArray.SetValue(DeepCopyInternal(originalArray.GetValue(i), visited), i);

            return copiedArray;
        }

        // Handle collections
        if (typeof(IList).IsAssignableFrom(typeToReflect))
        {
            IList originalList = (IList)original;
            IList copiedList = (IList)Activator.CreateInstance(typeToReflect);
            visited.Add(original, copiedList);

            foreach (var item in originalList)
                copiedList.Add(DeepCopyInternal(item, visited));

            return copiedList;
        }

        // Create instance without calling constructors
        object clone = Activator.CreateInstance(typeToReflect);
        visited.Add(original, clone);

        // Copy all fields (including private and inherited)
        foreach (FieldInfo field in GetAllFields(typeToReflect))
        {
            object originalValue = field.GetValue(original);
            object copiedValue = DeepCopyInternal(originalValue, visited);
            field.SetValue(clone, copiedValue);
        }

        return clone;
    }

    private static bool IsPrimitive(Type type)
    {
        return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
    }

    private static IEnumerable<FieldInfo> GetAllFields(Type type)
    {
        if (type == null)
            return new FieldInfo[0];

        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        List<FieldInfo> fields = new List<FieldInfo>(type.GetFields(flags));
        fields.AddRange(GetAllFields(type.BaseType));
        return fields;
    }

    /// <summary>
    /// Custom comparer to handle reference equality in dictionary keys.
    /// </summary>
    private class ReferenceEqualityComparer : EqualityComparer<object>
    {
        public override bool Equals(object x, object y) => ReferenceEquals(x, y);
        public override int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }
}
