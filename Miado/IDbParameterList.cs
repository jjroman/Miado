using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Miado
{
    /// <summary>
    /// This interface defines the methods for adding DB parameters 
    /// that will be substitued in a DB command.
    /// </summary>
    public interface IDbParameterList : IEnumerable<DbParameter>
    {

        /// <summary>
        /// Gets the count of the parameter list.
        /// </summary>
        /// <value>The count of the list.</value>
        int Count { get; }

        /// <summary>
        /// Adds a DbParameter with a given parameter name
        /// and corresponding value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value.</param>
        /// <returns>a reference to this object</returns>
        IDbParameterList Add(string name, object value);

        /// <summary>
        /// Adds a DbParameter with a given parameter name
        /// and corresponding value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">The DbType.</param>
        /// <returns>a reference to this object</returns>
        IDbParameterList Add(string name, object value, DbType dbType);

        /// <summary>
        /// Adds a DbParameter with a given parameter name
        /// and corresponding value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value.</param>
        /// <param name="direction">The direction of the parameter
        /// in the DB command (defaulted to input).</param>
        /// <returns>a reference to this object</returns>
        IDbParameterList Add(string name, object value, ParameterDirection direction);


        /// <summary>
        /// Adds a DbParameter with a given parameter name
        /// and corresponding value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">The DbType.</param>
        /// <param name="direction">The direction of the parameter
        /// in the DB command (defaulted to input).</param>
        /// <returns>a reference to this object</returns>
        IDbParameterList Add(string name, 
                             object value, 
                             DbType dbType, 
                             ParameterDirection direction);

        /// <summary>
        /// Adds a DbParameter to this collection that is populated 
        /// by a function.
        /// </summary>
        /// <param name="paramAction">A function that populates an
        /// empty DbParameter.</param>
        /// <returns>a reference to this object</returns>
        IDbParameterList Add(Action<DbParameter> paramAction);

		/// <summary>
		/// Adds an Enumeration of DbParameter objects to this 
		/// collection.
		/// </summary>
		/// <param name="dbParameters">An enumeration of DbParameter objects.</param>
		/// <returns>a reference to this object</returns>
        IDbParameterList AddRange(IEnumerable<DbParameter> dbParameters);

        /// <summary>
        /// Determines whether parameter list contains a
        /// DbParameter with the given name.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns>
        /// 	<c>true</c> if the parameter list has a parameter with
        /// 	the given name; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(string name);

        /// <summary>
        /// Gets the <see cref="System.Data.Common.DbParameter"/> at the specified index.
        /// </summary>
        /// <value></value>
        DbParameter this[int index] { get; }

        /// <summary>
        /// Gets the <see cref="System.Data.Common.DbParameter"/> with the specified name.
        /// </summary>
        /// <value></value>
        DbParameter this[string name] { get; }
    }
}
