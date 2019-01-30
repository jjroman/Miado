using System;
using System.Collections;
using System.Collections.Generic;

namespace Miado.Extensions
{
    /// <summary>
    /// This class provides extension methods for IList and List implementations.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// This extension method provides a way to adds an enumeration to an 
        /// existing IList.
        /// </summary>
        /// <param name="internalList">The internal list.</param>
        /// <param name="enumeration">The enumeration.</param>
        public static void AddEnumeration(this IList internalList, IEnumerable enumeration)
        {
            if ( enumeration == null )
            {
                throw new ArgumentNullException("enumeration");
            }
            foreach ( var obj in enumeration )
            {
                internalList.Add(obj);
            }
        }

        /// <summary>
        /// This extension method provides a way to adds an enumeration to an 
        /// existing IList&gt;T&lt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="internalList">The internal list.</param>
        /// <param name="enumeration">The enumeration.</param>
		public static void AddEnumeration<T>(this IList<T> internalList, IEnumerable<T> enumeration)
		{
            if ( enumeration == null )
            {
                throw new ArgumentNullException("enumeration");
            }
			foreach ( var obj in enumeration )
			{
				internalList.Add(obj);
			}
		}
    }
}
