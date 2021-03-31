using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Core.Utilities
{
    public static class Reflection
    {
        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<TAttribute>()
            where TAttribute : Attribute =>
            Assembly.GetExecutingAssembly().GetTypes().SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes().OfType<TAttribute>().Any());


        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<TAssembly, TAttribute>() =>
            GetMethodsWithAttribute<TAttribute>(typeof(TAssembly));


        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<TAttribute>(Type assembly) =>
            assembly.GetMethods().Where(m => m.GetCustomAttributes().OfType<TAttribute>().Any());


        public static IEnumerable<FieldInfo> GetAllNullValueFields<T>(T instance) => instance.GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(f => (f.GetCustomAttribute<SerializeField>() != null || f.IsPublic) &&
                        f.GetValue(instance)?.ToString() == "null");


        /// <summary>
        /// Get all classes from namespace
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="nameSpace"></param>
        /// <example>
        /// var types = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "MyNamespace");
        /// // Or something like this
        /// Assembly myAssembly = typeof([Namespace].[someClass]).GetTypeInfo().Assembly;
        /// var types = GetTypesInNamespace(myAssembly, "[Namespace]");
        /// </example>
        public static IEnumerable<Type> GetTypesInNamespace(Assembly assembly, string nameSpace) =>
            assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal));

        /// <summary>
        /// Remove namespaces from class name string
        /// Core.Utilities.Reflection => Reflection
        /// </summary>
        public static string GetTypeName(object obj) => obj.GetType().ToString().Split('.').Last();
    }
}