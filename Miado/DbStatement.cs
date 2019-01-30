using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Miado.Extensions;
using Miado.Reflection;

namespace Miado
{
    /// <summary>
    /// Provides an improved implementation of using ADO.Net by removing
    /// common boiler-plate code and providing an easier API
    /// </summary>
    abstract class DbStatement : IDbStatement
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbStatement"/> class.
        /// </summary>
        /// <param name="db">The IDatabase instance.</param>
        /// <param name="cmdText">the SQL or stored procedure name.</param>
        protected DbStatement(IDatabase db, string cmdText)
        {
            if ( db == null )
            {
                throw new ArgumentNullException("db");
            }
            if ( String.IsNullOrEmpty(cmdText) )
            {
                throw new ArgumentNullException("cmdText");
            }
            this.CommandText = cmdText;
            this.Parameters = new DbParameterList(db.DbProviderFactory);
            this.Database = db;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the database the statement will use.
        /// </summary>
        /// <value>The database the statement will use.</value>
        public IDatabase Database
        {
            get;
            protected set;
        }

        /// <summary>
        /// Read-only access to the parameters that will be used in this 
        /// statement
        /// </summary>
        public IDbParameterList Parameters
        {
            get;
            protected set;
        }

        /// <summary>
        /// The text that will be assigned 
        /// to the DbCommand.CommandText property
        /// </summary>
        public string CommandText
        {
            get;
            protected set;
        }

        /// <summary>
        /// The type of command 
        /// that will be assigned to the DbCommand.CommandType 
        /// </summary>
        public abstract CommandType CommandType
        {
            get;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a parameter to the SQL statement (the DbType will be inferred)
        /// </summary>
        /// <param name="paramName">the name of the parameter in the 
        /// SQL statement</param>
        /// <param name="paramValue">the actual value that will be replaced</param>
        /// <returns>a reference to this object</returns>
        public IDbStatement AddParameter(string paramName, object paramValue)
        {
            this.Parameters.Add(paramName, paramValue);
            return this;
        }

        /// <summary>
        /// Add a parameter to the SQL statement using a specific 
        /// DbType
        /// </summary>
        /// <param name="paramName">the name of the parameter in the 
        /// SQL statement</param>
        /// <param name="paramValue">the actual value that will be replaced</param>
        /// <param name="dbType">a valid value from the DbType enum</param>
        /// <returns>a reference to this object</returns>
        public IDbStatement AddParameter(string paramName, object paramValue, DbType dbType)
        {
            this.Parameters.Add(paramName, paramValue, dbType);
            return this;
        }

        /// <summary>
        /// Adds a parameter to the SQL statement using a delegate method to
        /// populate a DbParameter instance.
        /// </summary>
        /// <param name="parameterPopulater">The parameter populater delegate
        /// method that will be used to populate a DbParameter instance.</param>
        /// <returns>a reference to this object</returns>
        public IDbStatement AddParameter(Action<DbParameter> parameterPopulater)
        {
            this.Parameters.Add(parameterPopulater);
            return this;
        }

        /// <summary>
        /// Adds parameters from an object implenting the IDbParameterList
        /// interface.
        /// </summary>
        /// <param name="dbParameters">A IDbParameterList object.</param>
        /// <returns>a reference to this object</returns>
        public IDbStatement AddParameters(IDbParameterList dbParameters)
        {
            this.Parameters.AddRange(dbParameters);
            return this;
        }

        /// <summary>
        /// Adds any number of parameters to a SQL statement by using a delegate
        /// method that passes in the current parameter list.
        /// </summary>
        /// <param name="parameterPopulater">The parameter populater delegate
        /// method that will be used to populate a IDbParameterList with
        /// instances of DbParameter objects.</param>
        /// <returns>a reference to this object</returns>
        public IDbStatement AddParameters(Action<IDbParameterList> parameterPopulater)
        {
            parameterPopulater(this.Parameters);
            return this;
        }

        /// <summary>
        /// Add all the parameters using a Dictionary which maps their name to their value
        /// </summary>
        /// <param name="paramNameValuePairs">a Dictionary mapping parameter names to parameter
        /// values</param>
        /// <returns>a reference to this object</returns>
        public IDbStatement AddParameters(IDictionary<string, object> paramNameValuePairs)
        {
            if ( paramNameValuePairs == null )
            {
                throw new ArgumentNullException("paramNameValuePairs");
            }
            foreach ( string paramName in paramNameValuePairs.Keys ) 
            {
                var val = paramNameValuePairs[paramName];
                this.AddParameter(paramName, val);
            }
            return this;
        }

        /// <summary>
        /// Return a Collection of a specific object type
        /// that is populated from the result set of a given query
        /// </summary>
        /// <typeparam name="T">the type of object that will be created
        /// using the DbDataReader</typeparam>
        /// <param name="rowMapperDelegate">the delegate used to map
        /// the results from a DbDataReader to a given object</param>
        /// <returns>a strongly-typed collection of objects</returns>
        public virtual IEnumerable<T> QueryForResults<T>(Func<IResultSetRow, T> rowMapperDelegate)
        {
            using ( DbConnection conn = this.Database.CreateConnection() )
            {
                conn.Open();

                using ( DbCommand cmd = conn.CreateCommand() )
                {
                    cmd.CommandText = this.CommandText;
                    cmd.CommandType = this.CommandType;

                    if ( this.Parameters != null )
                    {
                        cmd.Parameters.AddEnumeration(this.Parameters);
                    }

                    using ( DbDataReader reader = cmd.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            // call the delegate to map the reader to a business object
                            T instance = rowMapperDelegate(new ResultSetRow(reader));
                            yield return instance;
                        }
                    }
                }
            }
        }

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
        public virtual IEnumerable<T> QueryForResults<T>(IDictionary<string, string> propertyToColumnMapping) where T : new()
        {
            using ( DbConnection conn = this.Database.CreateConnection() )
            {
                conn.Open();

                using ( DbCommand cmd = conn.CreateCommand() )
                {
                    cmd.CommandText = this.CommandText;
                    cmd.CommandType = this.CommandType;

                    if ( this.Parameters != null )
                    {
                        cmd.Parameters.AddEnumeration(this.Parameters);
                    }

                    using ( DbDataReader reader = cmd.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            IResultSetRow row = new ResultSetRow(reader);

                            T instance = Activator.CreateInstance<T>();
                            // iterate over the mappings, pulling the Property using reflection 
                            // and (safely) retrieving the value from the DbDataReader by
                            // its column name
                            foreach ( string propertyName in propertyToColumnMapping.Keys )
                            {
                                PropertyInfo pi = PropertyCache.GetProperty<T>(propertyName);
                                if ( pi != null )
                                {
                                    string columnName = propertyToColumnMapping[propertyName];
                                    object val = row.GetValue(columnName);
                                    pi.SetValue(instance, val, null);
                                }
                            }
                            yield return instance;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return the first instance of a strongly-typed 
        /// business object that is populated from the result set
        /// of a given query
        /// </summary>
        /// <typeparam name="T">the type of object that will be created
        /// using the DbDataReader</typeparam>
        /// <param name="rowMapperDelegate">the delegate used to map
        /// the results from a DbDataReader to a given object</param>
        /// <returns>the first object returned from the query</returns>
        public virtual T QueryForOne<T>(Func<IResultSetRow, T> rowMapperDelegate)
        {
            return QueryForResults(rowMapperDelegate).FirstOrDefault();
        }

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
        public virtual T QueryForOne<T>(IDictionary<string, string> propertyToColumnMapping) where T : new()
        {
            return this.QueryForResults<T>(propertyToColumnMapping).FirstOrDefault();
        }

        /// <summary>
        /// Return a DataSet populated from the result set of a given 
        /// query
        /// </summary>
        /// <returns>a populated DataSet</returns>
        public virtual DataSet QueryForDataSet()
        {
            return QueryForDataSetUsingCustomDataAdapterFillFunction<DataSet>(
                (adapter, ds) => adapter.Fill(ds));
        }

        /// <summary>
        /// Return a TypedDataSet populated from the result set of a given
        /// query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>a populated TypedDataSet</returns>
        public virtual T QueryForDataSet<T>() where T : DataSet
        {
            return QueryForDataSetUsingCustomDataAdapterFillFunction<T>(
                (adapter, ds) => adapter.Fill(ds, ds.Tables[0].TableName));
        }

        /// <summary>
        /// Return a DataTable representing the first table of 
        /// a DataSet populated from the result set of a given 
        /// query
        /// </summary>
        /// <returns>a populated DataTable</returns>
        public virtual DataTable QueryForDataTable()
        {
            DataTable dt = CreateEmptyDataTable();

            using ( DbConnection conn = this.Database.CreateConnection() )
            {
                conn.Open();

                using ( DbCommand cmd = conn.CreateCommand() )
                {
                    cmd.CommandText = this.CommandText;
                    cmd.CommandType = this.CommandType;

                    if ( this.Parameters != null )
                    {
                        cmd.Parameters.AddEnumeration(this.Parameters);
                    }

                    using ( DbDataAdapter adapter = this.Database.CreateDataAdapter() )
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// Execute a SQL statement that doesn't return a result set
        /// (e.g. "INSERT", "UPDATE", or "DELETE")
        /// </summary>
        public virtual void ExecuteNonQuery()
        {
            using ( DbConnection conn = this.Database.CreateConnection() )
            {
                conn.Open();

                using ( DbCommand cmd = conn.CreateCommand() )
                {
                    cmd.CommandText = this.CommandText;
                    cmd.CommandType = this.CommandType;

                    if ( Parameters != null )
                    {
                        cmd.Parameters.AddEnumeration(this.Parameters);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Protected Methods 

        /// <summary>
        /// Creates an empty DataSet.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>an empty DataSet</returns>
        protected virtual T CreateEmptyDataSet<T>() where T : DataSet
        {
            var ds = Activator.CreateInstance<T>();
            ds.Locale = CultureInfo.InvariantCulture;
            return ds;
        }

        /// <summary>
        /// Creates an empty DataTable.
        /// </summary>
        /// <returns>an empty DataTable</returns>
        protected virtual DataTable CreateEmptyDataTable()
        {
            return new DataTable() { Locale = CultureInfo.InvariantCulture };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Queries for data set using a custom function to fill the 
        /// DataSet using the DbDataAdapter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="adapterFillFunction">The function that will fill the DataSet using
        /// the DbDataAdapter.</param>
        /// <returns></returns>
        private T QueryForDataSetUsingCustomDataAdapterFillFunction<T>(
            Action<DbDataAdapter, T> adapterFillFunction) where T : DataSet
        {
            T ds = CreateEmptyDataSet<T>();

            using ( DbConnection conn = this.Database.CreateConnection() )
            {
                conn.Open();

                using ( DbCommand cmd = conn.CreateCommand() )
                {
                    cmd.CommandText = this.CommandText;
                    cmd.CommandType = this.CommandType;

                    if ( this.Parameters != null )
                    {
                        cmd.Parameters.AddEnumeration(this.Parameters);
                    }

                    using ( DbDataAdapter adapter = this.Database.CreateDataAdapter() )
                    {
                        adapter.SelectCommand = cmd;
                        adapterFillFunction(adapter, ds);
                    }
                }
            }

            return ds;
        }

        #endregion

    }
}
