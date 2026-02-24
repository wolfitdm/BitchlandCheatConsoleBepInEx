using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class MasterDeepCopy
{
    /// <summary>
    /// Deep copies any object in Unity, including GameObjects, Components, ScriptableObjects, and POCOs.
    /// </summary>
    /// <param name="original">The object to copy.</param>
    /// <param name="cloneUnityAssets">If true, UnityEngine.Object assets (Materials, Textures, etc.) will be cloned instead of referenced.</param>
    /// <param name="preserveTransform">If true, GameObjects will keep the same world position, rotation, and scale as the original.</param>
    public static T DeepCopy<T>(T original, bool cloneUnityAssets = false, bool preserveTransform = false) where T : class
    {
        return (T)DeepCopyInternal(original, new Dictionary<object, object>(), cloneUnityAssets, preserveTransform);
    }

    private static object DeepCopyInternal(object original, Dictionary<object, object> visited, bool cloneUnityAssets, bool preserveTransform)
    {
        if (original == null)
            return null;

        var type = original.GetType();

        // Prevent circular references
        if (visited.ContainsKey(original))
            return visited[original];

        // Handle GameObject
        if (original is GameObject go)
            return CopyGameObjectRecursive(go, visited, cloneUnityAssets, preserveTransform);

        // Handle Component
        if (original is Component comp)
            return CopyComponent(comp, visited, cloneUnityAssets, preserveTransform);

        // Handle ScriptableObject
        if (original is ScriptableObject so)
        {
            var clone = ScriptableObject.CreateInstance(type);
            visited[original] = clone;
            CopyFields(so, clone, visited, cloneUnityAssets, preserveTransform);
            return clone;
        }

        // Handle UnityEngine.Object assets
        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            if (cloneUnityAssets)
                return UnityEngine.Object.Instantiate((UnityEngine.Object)original);
            return original; // Keep reference
        }

        // Value types and strings
        if (type.IsValueType || type == typeof(string))
            return original;

        // Arrays
        if (type.IsArray)
        {
            var array = (Array)original;
            var copiedArray = Array.CreateInstance(type.GetElementType(), array.Length);
            visited[original] = copiedArray;
            for (int i = 0; i < array.Length; i++)
                copiedArray.SetValue(DeepCopyInternal(array.GetValue(i), visited, cloneUnityAssets, preserveTransform), i);
            return copiedArray;
        }

        // Lists
        if (typeof(IList).IsAssignableFrom(type))
        {
            var copiedList = (IList)Activator.CreateInstance(type);
            visited[original] = copiedList;
            foreach (var item in (IList)original)
                copiedList.Add(DeepCopyInternal(item, visited, cloneUnityAssets, preserveTransform));
            return copiedList;
        }

        // Dictionaries
        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            var copiedDict = (IDictionary)Activator.CreateInstance(type);
            visited[original] = copiedDict;
            foreach (DictionaryEntry entry in (IDictionary)original)
                copiedDict.Add(
                    DeepCopyInternal(entry.Key, visited, cloneUnityAssets, preserveTransform),
                    DeepCopyInternal(entry.Value, visited, cloneUnityAssets, preserveTransform)
                );
            return copiedDict;
        }

        // Complex objects (POCOs)
        var objClone = Activator.CreateInstance(type);
        visited[original] = objClone;
        CopyFields(original, objClone, visited, cloneUnityAssets, preserveTransform);
        return objClone;
    }

    // ---------------- GameObject Copy ----------------
    private static GameObject CopyGameObjectRecursive(GameObject original, Dictionary<object, object> visited, bool cloneUnityAssets, bool preserveTransform)
    {
        GameObject copy = new GameObject(original.name);
        visited[original] = copy;

        // Preserve transform if requested
        if (preserveTransform)
        {
            copy.transform.position = original.transform.position;
            copy.transform.rotation = original.transform.rotation;
            copy.transform.localScale = original.transform.localScale;
        }

        // Copy components
        foreach (var originalComp in original.GetComponents<Component>())
        {
            if (originalComp == null) continue;
            if (originalComp is Transform) continue;

            var newComp = copy.AddComponent(originalComp.GetType());
            visited[originalComp] = newComp;
            CopyFields(originalComp, newComp, visited, cloneUnityAssets, preserveTransform);
        }

        // Copy children
        foreach (Transform child in original.transform)
        {
            var childCopy = CopyGameObjectRecursive(child.gameObject, visited, cloneUnityAssets, preserveTransform);
            childCopy.transform.SetParent(copy.transform, preserveTransform);
        }

        return copy;
    }

    // ---------------- Component Copy ----------------
    private static Component CopyComponent(Component original, Dictionary<object, object> visited, bool cloneUnityAssets, bool preserveTransform)
    {
        var newGO = new GameObject(original.gameObject.name + "_Copy");
        if (preserveTransform)
        {
            newGO.transform.position = original.transform.position;
            newGO.transform.rotation = original.transform.rotation;
            newGO.transform.localScale = original.transform.localScale;
        }

        var newComp = newGO.AddComponent(original.GetType());
        visited[original] = newComp;
        CopyFields(original, newComp, visited, cloneUnityAssets, preserveTransform);
        return newComp;
    }

    // ---------------- Field Copy Helper ----------------
    private static void CopyFields(object source, object target, Dictionary<object, object> visited, bool cloneUnityAssets, bool preserveTransform)
    {
        var type = source.GetType();
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var value = field.GetValue(source);
            var copiedValue = DeepCopyInternal(value, visited, cloneUnityAssets, preserveTransform);
            field.SetValue(target, copiedValue);
        }
    }
}
