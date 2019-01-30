using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miado.Query
{
    /// <summary>
    /// This interface defines the properties and methods used 
    /// to represent a custom SQL Query
    /// </summary>
    public interface ISqlQuery
    {
        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <value>The SQL.</value>
        string Sql { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        IDictionary<string, object> Parameters { get; }

        /// <summary>
        /// Gets a value indicating whether the query has been built.
        /// </summary>
        /// <value><c>true</c> if the query has been built; otherwise, <c>false</c>.</value>
        bool IsBuilt { get; }

        /// <summary>
        /// Builds the SQL and registers the parameters.
        /// </summary>
        void Build();        
    }
}
