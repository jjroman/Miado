using System.Data.Common;

namespace Miado
{
    /// <summary>
    /// This interface defines the methods that can
    /// be used to get data from a row in a result set 
    /// </summary>
    public interface IResultSetRow
    {
        /// <summary>
        /// Gets the underlying DbDataReader.
        /// </summary>
        /// <value>The underlying DbDataReader.</value>
        DbDataReader DataReader
        {
            get;
        }

        /// <summary>
        /// Gets the value associated with the column at 
        /// the specified ordinal
        /// </summary>
        /// <typeparam name="T">the Type you are expecting 
        /// this ordinal to be</typeparam>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>the value of the column (unless the column 
        /// is NULL - then it returns the "default" of whatever 
        /// type was asked for)</returns>
        T GetValue<T>(int ordinal);

        /// <summary>
        /// Gets the value associated with the column 
        /// with the specified column name
        /// </summary>
        /// <typeparam name="T">the Type you are expecting 
        /// this ordinal to be</typeparam>
        /// <param name="columnName">The column name.</param>
        /// <returns>the value of the column (unless the column 
        /// is NULL - then it returns the "default" of whatever 
        /// type was asked for)</returns>
        T GetValue<T>(string columnName);

        /// <summary>
        /// Gets the value associated with the column at 
        /// the specified ordinal
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>the value of the column (unless the column 
        /// is NULL - then it returns the "default" of whatever 
        /// type was asked for)</returns>
        object GetValue(int ordinal);

        /// <summary>
        /// Gets the value associated with the column  
        /// with the specified column name
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>the value of the column (unless the column 
        /// is NULL - then it returns the "default" of whatever 
        /// type was asked for)</returns>
        object GetValue(string columnName);

        /// <summary>
        /// Determines whether the record is null in the DB at 
        /// the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>
        /// 	<c>true</c> if the DB record is null at the 
        /// 	specified ordinal; otherwise, <c>false</c>.
        /// </returns>
        bool IsDBNull(int ordinal);

        /// <summary>
        /// Determines whether the record is null in the DB for
        /// the specified column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>
        /// 	<c>true</c> if if the DB record is null at the 
        /// 	specified ordinal; otherwise, <c>false</c>.
        /// </returns>
        bool IsDBNull(string columnName);

    }
}
