using System;
using System.Collections.Generic;
using System.Reflection;
using Miado.Threading;

namespace Miado.Reflection
{
    /// <summary>
    /// This class provides a way to access Properties from an 
    /// object's Type using reflection.  Since using reflection 
    /// at runtime is an expensive operation, this class will 
    /// cache the properties on a given type the first time 
    /// that class is accessed.  Any subsequent requests for that
    /// class's properties will hit the cache instead of a runtime
    /// lookup, thereby vastly improving performance.
    /// </summary>
    public sealed class PropertyCache
    {
        #region Members

        private const int LOCK_TIMEOUT = 60*1000; //60 seconds

        private static readonly Dictionary<Type, List<PropertyInfo>> _dictionary = new Dictionary<Type, List<PropertyInfo>>();
        private static readonly object _handle = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>private - all methods are static</remarks>
        private PropertyCache() { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the properties for a given Type
        /// </summary>
        /// <param name="type">the type of object</param>
        /// <returns>an array of PropertyInfo objects</returns>
        /// <remarks>lazy loads the properties for a given 
        /// object type and caches them for later lookup</remarks>
        public static ICollection<PropertyInfo> GetProperties(Type type)
        {
            return GetPropertyList(type);
        }

        /// <summary>
        /// Gets the properties for a given T
        /// </summary>
        /// <typeparam name="T">the Type of object</typeparam>
        /// <returns>a collection of properties for a given
        /// object</returns>
        public static ICollection<PropertyInfo> GetProperties<T>()
        {
            return GetProperties(typeof(T));
        }

        /// <summary>
        /// Get a given property from an object type
        /// </summary>
        /// <param name="type">the type of object</param>
        /// <param name="name">the name of the property</param>
        /// <returns>a PropertyInfo object</returns>
        /// <remarks></remarks>
        public static PropertyInfo GetProperty(Type type, string name)
        {
            if ( String.IsNullOrEmpty(name) )
            {
                throw new ArgumentNullException("name");
            }
            List<PropertyInfo> properties = GetPropertyList(type);
            if ( properties != null && properties.Count > 0 )
            {
                return properties.Find(
                    pi => String.Compare(pi.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
            }
            return null;
        }

        /// <summary>
        /// Gets the property for a given T
        /// </summary>
        /// <typeparam name="T">the Type of object</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <returns>a PropertyInfo object</returns>
        public static PropertyInfo GetProperty<T>(string name)
        {
            return GetProperty(typeof(T), name);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Return a List containing all the Properties for a given 
        /// Type
        /// </summary>
        /// <param name="type">the Type</param>
        /// <returns>a List of PropertyInfo objects</returns>
        private static List<PropertyInfo> GetPropertyList(Type type)
        {
            var properties = new List<PropertyInfo>();

            using ( var propertyLock = new LockHolder<object>(_handle, LOCK_TIMEOUT) )
            {
                if ( !_dictionary.ContainsKey(type) )
                {
                    var propList = new List<PropertyInfo>();

                    foreach ( PropertyInfo pi in type.GetProperties(BindingFlags.Public |
                                                                        BindingFlags.Instance |
                                                                        BindingFlags.NonPublic) )
                    {
                        propList.Add(pi);
                    }

                    _dictionary[type] = propList;
                }

                properties = _dictionary[type];
            }

            return properties;
        }

        #endregion
    }
}