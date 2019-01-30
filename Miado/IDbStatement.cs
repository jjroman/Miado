using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;


namespace Miado
{
    /// <summary>
    /// Defines methods for simplifying the use of ADO.Net in a
    /// data access layer
    /// </summary>
    /// <example>
    /// Most of the methods on this interface return a reference to 
    /// itself, so you can use the IDbStatement to build up 
    /// the SQL call in a Domain-Specific-Language sort of way.
    /// Assuming the following SQL statements:
    /// <code>
    /// private static readonly string INSERT_ADDRESS_SQL = 
    ///     "insert into Address(Address1, Address2, City, State, ZipCode) " +
    ///     "values(@Addr1, @Addr2, @City, @State, @ZipCode)";
    ///     
    /// private static readonly string SELECT_ADDRESS_SQL = 
    ///     "select AddressId, Address1, Address2, City, " +
    ///     "State, ZipCode " +
    ///     "from Address ";
    /// 
    /// private static readonly string SELECT_ADDRESS_BY_ID = 
    ///     SELECT_ADDRESS_SQL + "where AddressId = @Id ";
    ///     
    /// private static readonly string SELECT_ADDRESS_BY_ZIP_CODE =
    ///     SELECT_ADDRESS_SQL + "where ZipCode = @ZipCode ";
    ///     
    /// private static readonly string SELECT_COUNT_BY_STATE = 
    ///     "select count(*) from Address where State = @State ";
    ///     
    /// </code>
    /// You can see the IDbStatement in action:
    /// <code>
    /// private string _connString = 
    ///     ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
    ///     
    /// private IDatabase _db = new SqlServerDatabase(_connString);
    /// 
    /// public IDatabase Database 
    /// { 
    ///     get { return _db; }
    /// }
    /// 
    /// public void SaveAddress(Address addr)
    /// {
    ///     this.Database
    ///         .ExecutingSql(INSERT_ADDRESS_SQL)
    ///         .AddParameter("Addr1", addr.Address1)
    ///         .AddParameter("Addr2", addr.Address2)
    ///         .AddParameter("City", addr.City)
    ///         .AddParameter("State", addr.State)
    ///         .AddParameter("ZipCode", addr.ZipCode)
    ///         .ExecuteNonQuery();
    /// }
    /// 
    /// public IEnumerable&lt;Address&gt; FindAddressesByZipCode(string zipCode) 
    /// {
    ///     IEnumerable&lt;Address&gt; addresses = 
    ///         this.Database
    ///             .ExecutingSql(SELECT_ADDRESS_BY_ZIP_CODE)
    ///             .AddParameter("ZipCode", zipCode)
    ///             .QueryForResults&lt;Address&gt;(
    ///                 row =&gt; new Address() 
    ///                     {
    ///                         AddressId = row.GetValue&lt;int&gt;("AddressId"), 
    ///                         Address1 = row.GetValue&lt;string&gt;("Address1"),
    ///                         Address2 = row.GetValue&lt;string&gt;("Address2"),
    ///                         City = row.GetValue&lt;string&gt;("City"),
    ///                         State = row.GetValue&lt;string&gt;("State"),
    ///                         ZipCode = row.GetValue&lt;string&gt;("ZipCode")
    ///                     });
    ///     return addresses;
    /// }
    /// 
    /// public Address LoadAddress(int addrId) 
    /// {
    ///     return 
    ///         this.Database
    ///             .ExecutingSql(SELECT_ADDRESS_BY_ID)
    ///             .AddParameter("Id", addrId)
    ///             .QueryForOne&lt;Address&gt;(
    ///                 row =&gt; new Address() 
    ///                     {
    ///                         AddressId = row.GetValue&lt;int&gt;("AddressId"), 
    ///                         Address1 = row.GetValue&lt;string&gt;("Address1"),
    ///                         Address2 = row.GetValue&lt;string&gt;("Address2"),
    ///                         City = row.GetValue&lt;string&gt;("City"),
    ///                         State = row.GetValue&lt;string&gt;("State"),
    ///                         ZipCode = row.GetValue&lt;string&gt;("ZipCode")
    ///                     });
    /// }
    /// 
    /// public int FindCountByState(string state) 
    /// { 
    ///     return 
    ///         this.Database
    ///             .ExecutingSql(SELECT_COUNT_BY_STATE)
    ///             .AddParameter("State", state)
    ///             .QueryForOne&lt;int&gt;(row =&gt; row.GetValue&lt;int&gt;(0));
    /// }
    /// </code>
    /// Instead of using lambda expressions like in the example above, you could also define a method as pass 
    /// it as a delegate:
    /// <code>
    /// private Address CreateAddress(IResultSetRow row) 
    /// { 
    ///     return new Address() 
    ///         { 
    ///             AddressId = row.GetValue&lt;int&gt;(0)
    ///             // remainder omitted for brevity
    ///         };
    /// }
    /// </code>
    /// <code>
    /// public IEnumerable&lt;Address&gt; FindAddressesByZipCode(string zipCode) 
    /// {
    ///     return 
    ///         this.Database
    ///             .ExecutingSql(SELECT_ADDRESS_BY_ZIP_CODE)
    ///             .AddParameter("ZipCode", zipCode)
    ///             .QueryForResults&lt;Address&gt;(CreateAddress);
    /// }
    /// </code>
    /// </example>
    public interface IDbStatement
    {
        /// <summary>
        /// Gets the database the statement will use.
        /// </summary>
        /// <value>The database the statement will use.</value>
        IDatabase Database
        {
            get;
        }

        /// <summary>
        /// Gets the CommandText.
        /// </summary>
        /// <value>The command text</value>
        string CommandText
        {
            get;
        }

        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
		CommandType CommandType 
		{
			get;
		}

        /// <summary>
        /// Read-only access to the parameters that will be used in this 
        /// statement
        /// </summary>
        /// <value>The parameters.</value>
        IDbParameterList Parameters { get; }

        /// <summary>
        /// Add a parameter to the SQL statement (the DbType will be inferred)
        /// </summary>
        /// <param name="paramName">the name of the parameter in the 
        /// SQL statement</param>
        /// <param name="paramValue">the actual value that will be replaced</param>
        /// <returns>a reference to this object</returns>
        IDbStatement AddParameter(string paramName, object paramValue);

        /// <summary>
        /// Add a parameter to the SQL statement using a specific 
        /// DbType
        /// </summary>
        /// <param name="paramName">the name of the parameter in the 
        /// SQL statement</param>
        /// <param name="paramValue">the actual value that will be replaced</param>
        /// <param name="dbType">a valid value from the DbType enum</param>
        /// <returns>a reference to this object</returns>
        IDbStatement AddParameter(string paramName, object paramValue, DbType dbType);

        /// <summary>
        /// Adds a parameter to the SQL statement using a delegate method to 
        /// populate a DbParameter instance.
        /// </summary>
        /// <param name="parameterPopulater">The parameter populater delegate
        /// method that will be used to populate a DbParameter instance.</param>
        /// <returns>a reference to this object</returns>
        IDbStatement AddParameter(Action<DbParameter> parameterPopulater);

        /// <summary>
        /// Adds any number of parameters to a SQL statement by using a delegate 
        /// method that passes in the current parameter list.
        /// </summary>
        /// <param name="parameterPopulater">The parameter populater delegate
        /// method that will be used to populate a IDbParameterList with 
        /// instances of DbParameter objects.</param>
        /// <returns>a reference to this object</returns>
        IDbStatement AddParameters(Action<IDbParameterList> parameterPopulater);

        /// <summary>
        /// Adds parameters from an object implenting the IDbParameterList 
        /// interface.
        /// </summary>
        /// <param name="dbParameters">A IDbParameterList object.</param>
        /// <returns>a reference to this object</returns>
		IDbStatement AddParameters(IDbParameterList dbParameters);

        /// <summary>
        /// Add all the parameters using a Dictionary which maps their name to their value
        /// </summary>
        /// <param name="paramNameValuePairs">a Dictionary mapping parameter names to parameter
        /// values</param>
        /// <returns>a reference to this object</returns>
        IDbStatement AddParameters(IDictionary<string, object> paramNameValuePairs);

        /// <summary>
        /// Execute a SQL statement that doesn't return a result set
        /// (e.g. "INSERT", "UPDATE", or "DELETE")
        /// </summary>
        void ExecuteNonQuery();

        /// <summary>
        /// Return a Collection of a specific object type
        /// that is populated from the result set of a given query
        /// </summary>
        /// <typeparam name="T">the type of object that will be created
        /// using the DbDataReader</typeparam>
        /// <param name="resultSetMapper">a function used to map
        /// the results from a IResultSetRow to a given object</param>
        /// <returns>a strongly-typed collection of objects</returns>
        IEnumerable<T> QueryForResults<T>(Func<IResultSetRow, T> resultSetMapper);

        /// <summary>
        /// Return a Collection of a specific object type
        /// that is populated from the result set of a given query
        /// </summary>
        /// <typeparam name="T">the type of object that will be created
        /// using the DbDataReader</typeparam>
        /// <param name="propertyToColumnMapping">a Dictionary containing 
        /// mappings of Properties on type T to columns in the result set</param>
        /// <returns>a strongly-typed collection of objects</returns>
        /// <remarks>This method is offered as pure syntactical sugar.  It has 
        /// two concerns: 1) it has no compile time safety; and 2) it uses 
        /// reflection to access the properties, so it will be much slower 
        /// than using the QueryForResults() method using a standard
        /// DbDataReader</remarks>
        IEnumerable<T> QueryForResults<T>(IDictionary<string, string> propertyToColumnMapping) where T : new();

        /// <summary>
        /// Return the first instance of a strongly-typed 
        /// business object that is populated from the result set
        /// of a given query
        /// </summary>
        /// <typeparam name="T">the type of object that will be created
        /// using the DbDataReader</typeparam>
        /// <param name="resultSetMapper">a function used to map
        /// the results from a IResultSetRow to a given object</param>
        /// <returns>the first object returned from the query</returns>
        T QueryForOne<T>(Func<IResultSetRow, T> resultSetMapper);

        /// <summary>
        /// Return the first instance of a strongly-typed 
        /// business object that is populated from the result set
        /// of a given query
        /// </summary>
        /// <typeparam name="T">the type of object that will be created
        /// using the DbDataReader</typeparam>
        /// <param name="propertyToColumnMapping">a Dictionary containing 
        /// mappings of Properties on type T to columns in the result set</param>
        /// <returns>the first object returned from the query</returns>
        /// <remarks>This method is offered as pure syntactical sugar.  It has 
        /// two concerns: 1) it has no compile time safety; and 2) it uses 
        /// reflection to access the properties, so it will be much slower 
        /// than using the QueryForResults() method using a standard
        /// DbDataReader</remarks>
        T QueryForOne<T>(IDictionary<string, string> propertyToColumnMapping) where T : new();

        /// <summary>
        /// Return a DataSet populated from the result set of a given 
        /// query
        /// </summary>
        /// <returns>a populated DataSet</returns>
        DataSet QueryForDataSet();

        /// <summary>
        /// Return a TypedDataSet populated from the result set of a given 
        /// query
        /// </summary>
        /// <returns>a populated TypedDataSet</returns>
        T QueryForDataSet<T>() where T : DataSet;

        /// <summary>
        /// Return a DataTable representing the first table of 
        /// a DataSet populated from the result set of a given 
        /// query
        /// </summary>
        /// <returns>a populated DataTable</returns>
        DataTable QueryForDataTable();
    }
}
