using System;
using System.Data.Common;

namespace Miado
{

    /// <summary>
    /// This class implements the methods defined in the 
    /// <see cref="IResultSetRow" /> interface for accessing
    /// values from a result set
    /// </summary>
    class ResultSetRow : IResultSetRow
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultSetRow"/> class.
        /// </summary>
        /// <param name="reader">The underlying DbDataReader.</param>
        public ResultSetRow(DbDataReader reader)
        {
            this.DataReader = reader;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying DbDataReader.
        /// </summary>
        /// <value>The underlying DbDataReader.</value>
        public DbDataReader DataReader
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the value associated with the column at
        /// the specified ordinal
        /// </summary>
        /// <typeparam name="T">the Type you are expecting
        /// this ordinal to be</typeparam>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>
        /// the value of the column (unless the column
        /// is NULL - then it returns the "default" of whatever
        /// type was asked for)
        /// </returns>
        public T GetValue<T>(int ordinal)
        {
            return this.IsDBNull(ordinal) ? default(T) : (T)this.GetValue(ordinal);
        }

        /// <summary>
        /// Gets the value associated with the column
        /// with the specified column name
        /// </summary>
        /// <typeparam name="T">the Type you are expecting
        /// this ordinal to be</typeparam>
        /// <param name="columnName">The column name.</param>
        /// <returns>
        /// the value of the column (unless the column
        /// is NULL - then it returns the "default" of whatever
        /// type was asked for)
        /// </returns>
        public T GetValue<T>(string columnName)
        {
            return (T)this.GetValue<T>(this.DataReader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets the value associated with the column at
        /// the specified ordinal
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>
        /// the value of the column (unless the column
        /// is NULL - then it returns the "default" of whatever
        /// type was asked for)
        /// </returns>
        public object GetValue(int ordinal)
        {
            Type fieldType = this.DataReader.GetFieldType(ordinal);
            if ( this.IsDBNull(ordinal) )
            {
                return fieldType.IsValueType ? Activator.CreateInstance(fieldType) : null;
            }
            return DataReader.GetValue(ordinal);
        }

        /// <summary>
        /// Gets the value associated with the column
        /// with the specified column name
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns>
        /// the value of the column (unless the column
        /// is NULL - then it returns the "default" of whatever
        /// type was asked for)
        /// </returns>
        public object GetValue(string columnName)
        {
            return this.GetValue(this.DataReader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Determines whether the record is null in the DB at
        /// the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>
        /// 	<c>true</c> if the DB record is null at the
        /// specified ordinal; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDBNull(int ordinal)
        {
            return DataReader.IsDBNull(ordinal);
        }

        /// <summary>
        /// Determines whether the record is null in the DB for
        /// the specified column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>
        /// 	<c>true</c> if if the DB record is null at the
        /// specified ordinal; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDBNull(string columnName)
        {
            return this.IsDBNull(this.DataReader.GetOrdinal(columnName));
        }

        #endregion
    }
}
