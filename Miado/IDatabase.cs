using System;
using System.Data.Common;
using Miado.Configuration;
using Miado.Query;

namespace Miado
{
    /// <summary>
    /// This interface defines the methods that will be used to interact 
    /// with a particular vendor's database.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Gets or sets the underlying DbProviderFactory.
        /// </summary>
        /// <value>The underlying DbProviderFactory.</value>
        DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get ; }

		/// <summary>
		/// Gets the parser that represents the syntax used 
        /// by the DbProvider to represent parameters in SQL.
		/// </summary>
		/// <value>The DbProvider SQL syntax parser.</value>
		IParameterParser DbProviderSyntaxParser { get; }

		/// <summary>
        /// Gets the parser that represents the syntax used 
        /// in the source code to represent parameters in SQL.
		/// </summary>
		/// <value>The source code SQL syntax parser.</value>
		IParameterParser SourceCodeSyntaxParser { get; }

		/// <summary>
		/// Gets the query registry.
		/// </summary>
		/// <value>The query registry.</value>
		IQueryRegistry QueryRegistry { get; }

        /// <summary>
        /// Creates a DbConnection using the underlying DbProviderFactory.
        /// </summary>
        /// <returns>A DbConnection.</returns>
        DbConnection CreateConnection();

        /// <summary>
        /// Creates a DbDataAdapter using the underlying DbProviderFactory.
        /// </summary>
        /// <returns>A DbDataAdapter.</returns>
        DbDataAdapter CreateDataAdapter();

        /// <summary>
        /// Creates a DbParameter using the underlying DbProviderFactory.
        /// </summary>
        /// <returns>A DbParameter.</returns>
        DbParameter CreateParameter();

        /// <summary>
        /// Configures the query registry.
        /// </summary>
        /// <param name="configurator">The configurator delegate method that will
        /// be passed he QueryRegistry in this object.</param>
		void ConfigureQueryRegistry(Action<IQueryRegistry> configurator);

        /// <summary>
        /// Creates a database statement using standard 
        /// SQL.
        /// </summary>
        /// <param name="sql">The SQL that will be invoked.</param>
        /// <returns>A populated instance of the IDbStatement.</returns>
        IDbStatement ExecutingSql(string sql);

        /// <summary>
        /// Creates a database statement using a stored procedure.
        /// </summary>
        /// <param name="storedProcName">The name of the stored procedure
        /// that will be invoked.</param>
        /// <returns>A populated instance of the IDbStatement.</returns>
        IDbStatement CallingStoredProcedureNamed(string storedProcName);

        /// <summary>
        /// Creates a database statement using a custom query.
        /// </summary>
        /// <param name="query">The custom query that knows how
        /// to build its own SQL and register its parameters.</param>
        /// <returns>A populated instance of the IDbStatement.</returns>
        IDbStatement RunningQuery(ISqlQuery query);

        /// <summary>
        /// Creates a database statement using standard 
        /// SQL that was registered in the Query Registry.
        /// </summary>
        /// <param name="queryName">The name in which the query was
        /// registered.</param>
        /// <returns>A populated instance of the IDbStatement.</returns>
        IDbStatement LoadQueryRegisteredAs(string queryName);

        /// <summary>
        /// Creates a database statement using standard 
        /// SQL that was registered in the Query Registry.
        /// </summary>
        /// <param name="queryFinder">A function that takes in the IQueryRegistry
        /// and returns back a SQL statement.</param>
        /// <returns>A populated instance of the IDbStatement.</returns>
        IDbStatement RetrieveQueryFromRegistry(Func<IQueryRegistry, string> queryFinder);

		/// <summary>
		/// Fluent interface for configuring the Miado repository to use the specified
		/// IParameterParser to parse the parameters in the SQL as designated in the 
		/// actual source code.
		/// </summary>
		/// <param name="sourceCodeSyntaxParser">The IParameterParser that will be used 
		/// to parse the parameters in the SQL found in the source code.</param>
		/// <returns>a reference to this object</returns>
        IDatabase UsingSourceCodeSyntax(IParameterParser sourceCodeSyntaxParser);

		/// <summary>
		/// Fluent interface to maps the source code syntax to the one used in the 
		/// underlying DbProvider.
		/// </summary>
		/// <param name="dbProviderSyntaxParser">The IParameterParser that will be used 
		/// to translate the SQL parameter syntax in the source code to the one 
		/// used by the DbProvider.</param>
		/// <returns>a reference to this object</returns>
        IDatabase MapSourceCodeSyntaxTo(IParameterParser dbProviderSyntaxParser);
    }
}
