﻿using System;
using System.Linq;
using System.Reflection;

namespace ClassToDataTable.Shared
{
    /// <summary>Reflection helper</summary>
    public class ReflectionHelper
    {
        /// <summary>Finds a property by name.</summary>
        /// <typeparam name="T">Type that has the property</typeparam>
        /// <param name="propertyName">Name of the property on the type.</param>
        /// <returns></returns>
        public static PropertyInfo FindPropertyInfoByName<T>(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return null;

            return typeof(T).GetProperties().FirstOrDefault(w => w.Name == propertyName);
        }    

        /// <summary>Creates a generic type</summary>
        /// <param name="typeWithGeneric">So, this is the class with the generic type MyClass of T</param>
        /// <param name="typeOfT">This is the T in MyClass of T</param>
        /// <returns>An instance of the generic type.</returns>
        public static object CreateGenericType(Type typeWithGeneric, Type typeOfT)
        {
            Type constructed = typeWithGeneric.MakeGenericType(typeOfT);
            return Activator.CreateInstance(constructed);
        }
    }
}
