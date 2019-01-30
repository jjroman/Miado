using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miado.Query
{
    /// <summary>
    /// This class inherits from the the SqlQuery class and uses an 
    /// Action delegate to build the SQL and register the parameters.
    /// </summary>
    public class GenericSqlQuery : SqlQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericSqlQuery"/> class.
        /// </summary>
        /// <param name="buildFunction">The build function that will use
        /// the StringBuilder to construct the SQL and the IDictionary 
        /// to register the parameters.</param>
        public GenericSqlQuery(Action<StringBuilder, IDictionary<string, object>> buildFunction) : base()
        {
            if ( buildFunction == null )
            {
                throw new ArgumentNullException("buildFunction");
            }
            BuildFunction = buildFunction;
        }

        /// <summary>
        /// Gets or sets the build function.
        /// </summary>
        /// <value>The build function that will be called to create the SQL using
        /// the StringBuilder and the IDictionary to register the parameters.</value>
        private Action<StringBuilder, IDictionary<string, object>> BuildFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation of the Template method to actually build the query and register
        /// the parameters.  It will create a StringBuilder and pass it into the Action 
        /// delegate, which is responsible for creating the SQL and registering the 
        /// parameters.
        /// </summary>
        protected override void DoBuild()
        {
            var sb = new StringBuilder();
            BuildFunction(sb, Parameters);
            this.Sql = sb.ToString();
        }
    }
}
