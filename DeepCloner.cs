using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DeepClonerUtil
{
 public static class DeepCloner
    {
        public static T DeepClone<T>(T source)
        {
            var visited = new Dictionary<object, object>(new ReferenceEqualityComparer());
            return (T)DeepCloneObject(source, visited);
        }

        private static object DeepCloneObject(object source, Dictionary<object, object> visited)
        {
            if (source == null)
                return null;

            Type type = source.GetType();

            // Handle primitive, string, decimal, DateTime, etc.
            if (type.IsValueType || type == typeof(string))
                return source;

            // Prevent circular reference loops
            if (visited.ContainsKey(source))
                return visited[source];

            // Handle arrays
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                Array array = (Array)source;
                Array clonedArray = Array.CreateInstance(elementType, array.Length);
                visited[source] = clonedArray;

                for (int i = 0; i < array.Length; i++)
                    clonedArray.SetValue(DeepCloneObject(array.GetValue(i), visited), i);

                return clonedArray;
            }

            // Handle collections (IList)
            if (typeof(IList).IsAssignableFrom(type))
            {
                IList list = (IList)Activator.CreateInstance(type);
                visited[source] = list;

                foreach (var item in (IList)source)
                    list.Add(DeepCloneObject(item, visited));

                return list;
            }

            // Create instance of the object
            object clone = Activator.CreateInstance(type, true);
            visited[source] = clone;

            // Copy all properties (public + non-public, inherited)
            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            foreach (var prop in properties)
            {
                if (!prop.CanRead || !prop.CanWrite) continue;
                if (prop.GetIndexParameters().Length > 0) continue; // Skip indexers

                object value = prop.GetValue(source, null);
                object clonedValue = DeepCloneObject(value, visited);
                prop.SetValue(clone, clonedValue, null);
            }

            // Copy all fields (public + non-public, inherited)
            FieldInfo[] fields = type.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            foreach (var field in fields)
            {
                object value = field.GetValue(source);
                object clonedValue = DeepCloneObject(value, visited);
                field.SetValue(clone, clonedValue);
            }

            return clone;
        }
        public class ReferenceEqualityComparer : EqualityComparer<object>
        {
            public override bool Equals(object x, object y) => ReferenceEquals(x, y);
            public override int GetHashCode(object obj) =>
                System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}