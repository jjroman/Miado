using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miado.Query
{
    /// <summary>
    /// An abstract class used to provide functionality common
    /// to classes that would implement the ISqlQuery interface.
    /// </summary>
    public abstract class SqlQuery : ISqlQuery 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQuery"/> class.
        /// </summary>
        protected SqlQuery()
        {
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <value>The SQL.</value>
        public string Sql
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IDictionary<string, object> Parameters
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a value indicating whether the query has been built.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the query has been built; otherwise, <c>false</c>.
        /// </value>
        public bool IsBuilt
        {
            get;
            private set;
        }

        /// <summary>
        /// Builds the SQL and registers the parameters.  It calls the DoBuild() method 
        /// to actually perform the building and then sets the IsBuilt property to True.
        /// </summary>
        public virtual void Build()
        {
            DoBuild();
            IsBuilt = true;
        }

        /// <summary>
        /// Template method that will build the query and register
        /// the parameters.  It must be implemented in sub-classes.
        /// </summary>
        protected abstract void DoBuild();
    }
}
