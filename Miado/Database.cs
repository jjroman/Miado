using System;
using System.Data.Common;
using Miado.Configuration;
using Miado.Query;

namespace Miado
{
    /// <summary>
    /// This class implements the methods defined in the IDatabase 
    /// interface.
    /// </summary>
    public class Database : IDatabase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="factory">The DbProvidenFactory used to create
        /// vendor specific ADO.Net instances.</param>
        /// <param name="connString">The connection string.</param>
        public Database(DbProviderFactory factory, string connString)
        {
            if ( factory == null )
            {
                throw new ArgumentNullException("factory");
            }
            if ( String.IsNullOrEmpty(connString) )
            {
                throw new ArgumentNullException("connString");
            }
            DbProviderFactory = factory;
            ConnectionString = connString;
            QueryRegistry = new QueryRegistry();
            var paramParser = new StandardParameterParser();
            this.UsingSourceCodeSyntax(paramParser).MapSourceCodeSyntaxTo(paramParser);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the underlying DbProviderFactory.
        /// </summary>
        /// <value>The underlying DbProviderFactory.</value>
        public DbProviderFactory DbProviderFactory
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the parser that represents the syntax used
        /// by the DbProvider to represent parameters in SQL.
        /// </summary>
        /// <value>The DbProvider SQL syntax parser.</value>
        public IParameterParser DbProviderSyntaxParser
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the parser that represents the syntax used
        /// in the source code to represent parameters in SQL.
        /// </summary>
        /// <value>The source code SQL syntax parser.</value>
        public IParameterParser SourceCodeSyntaxParser
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the query registry.
        /// </summary>
        /// <value>The query registry.</value>
        public IQueryRegistry QueryRegistry
        {
            get;
            protected set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a DbConnection using the underlying DbProviderFactory.
        /// </summary>
        /// <returns>A DbConnection.</returns>
        public DbConnection CreateConnection()
        {
            DbConnection conn = this.DbProviderFactory.CreateConnection();
            conn.ConnectionString = this.ConnectionString;
            return conn;
        }

        /// <summary>
        /// Creates a DbDataAdapter using the underlying DbProviderFactory.
        /// </summary>
        /// <returns>A DbDataAdapter.</returns>
        public DbDataAdapter CreateDataAdapter()
        {
            return this.DbProviderFactory.CreateDataAdapter();
        }

        /// <summary>
        /// Creates a DbParameter using the underlying DbProviderFactory.
        /// </summary>
        /// <returns>A DbParameter.</returns>
        public DbParameter CreateParameter()
        {
            return this.DbProviderFactory.CreateParameter();
        }

        /// <summary>
        /// Configures the query registry.
        /// </summary>
        /// <param name="configurator">The configurator delegate method that will
        /// be passed he QueryRegistry in this object.</param>
        public void ConfigureQueryRegistry(Action<IQueryRegistry> configurator)
        {
            configurator(this.QueryRegistry);
        }

        /// <summary>
        /// Creates a database statement using standard
        /// SQL.
        /// </summary>
        /// <param name="sql">The SQL that will be invoked.</param>
        /// <returns>
        /// A populated instance of the IDbStatement.
        /// </returns>
        public IDbStatement ExecutingSql(string sql)
        {
            return new SqlStatement(this, sql);
        }

        /// <summary>
        /// Creates a database statement using a stored procedure.
        /// </summary>
        /// <param name="storedProcName">The name of the stored procedure
        /// that will be invoked.</param>
        /// <returns>
        /// A populated instance of the IDbStatement.
        /// </returns>
        public IDbStatement CallingStoredProcedureNamed(string storedProcName)
        {
            return new StoredProcedureStatement(this, storedProcName);
        }

        /// <summary>
        /// Creates a database statement using a custom query.
        /// </summary>
        /// <param name="query">The custom query that knows how
        /// to build its own SQL and register its parameters.</param>
        /// <returns>
        /// A populated instance of the IDbStatement.
        /// </returns>
        public IDbStatement RunningQuery(ISqlQuery query)
        {
            if ( query == null )
            {
                throw new ArgumentNullException("query");
            }
            query.Build();
            var stmt = new SqlStatement(this, query.Sql);
            stmt.AddParameters(query.Parameters);
            return stmt;
        }

        /// <summary>
        /// Creates a database statement using standard
        /// SQL that was registered in the Query Registry.
        /// </summary>
        /// <param name="queryName">The name in which the query was
        /// registered.</param>
        /// <returns>
        /// A populated instance of the IDbStatement.
        /// </returns>
        public IDbStatement LoadQueryRegisteredAs(string queryName)
        {
            return new SqlStatement(this, this.QueryRegistry.FindQuery(queryName));
        }

        /// <summary>
        /// Creates a database statement using standard
        /// SQL that was registered in the Query Registry.
        /// </summary>
        /// <param name="queryFinder">A function that takes in the IQueryRegistry
        /// and returns back a SQL statement.</param>
        /// <returns>
        /// A populated instance of the IDbStatement.
        /// </returns>
        public IDbStatement RetrieveQueryFromRegistry(Func<IQueryRegistry, string> queryFinder)
        {
            return new SqlStatement(this, queryFinder(this.QueryRegistry));
        }

        /// <summary>
        /// Fluent interface for configuring the Miado repository to use the specified
        /// IParameterParser to parse the parameters in the SQL as designated in the
        /// actual source code.
        /// </summary>
        /// <param name="sourceCodeSyntaxParser">The IParameterParser that will be used
        /// to parse the parameters in the SQL found in the source code.</param>
        /// <returns>a reference to this object</returns>
        public IDatabase UsingSourceCodeSyntax(IParameterParser sourceCodeSyntaxParser)
        {
            SourceCodeSyntaxParser = sourceCodeSyntaxParser;
            return this;
        }

        /// <summary>
        /// Fluent interface to maps the source code syntax to the one used in the
        /// underlying DbProvider.
        /// </summary>
        /// <param name="dbProviderSyntaxParser">The IParameterParser that will be used
        /// to translate the SQL parameter syntax in the source code to the one
        /// used by the DbProvider.</param>
        /// <returns>a reference to this object</returns>
        public IDatabase MapSourceCodeSyntaxTo(IParameterParser dbProviderSyntaxParser)
        {
            DbProviderSyntaxParser = dbProviderSyntaxParser;
            return this;
        }

        #endregion
    }
}
