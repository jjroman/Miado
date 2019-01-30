using System;
using System.Collections.Generic;
using System.IO;
using Miado.Threading;

namespace Miado.Query
{
    /// <summary>
    /// This class implements the IQueryRegistry interface to provides methods
    /// for accessing named queries registered in this registry.
    /// </summary>
    class QueryRegistry : IQueryRegistry
    {
        #region Members
        
        private readonly IDictionary<string, string> _queries = new Dictionary<string, string>();
        private readonly object _lockHandle = new object();
        private const int TIMEOUT = 1000 * 60;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryRegistry"/> class.
        /// </summary>
        public QueryRegistry() { }

        #endregion

        /// <summary>
        /// Gets the count of queries that have been registered
        /// </summary>
        /// <value>The count of queries that have been registered.</value>
        public int Count
        {
            get 
            {
                using ( new LockHolder<object>(_lockHandle, TIMEOUT) )
                {
                    return _queries.Count;
                }
            }
        }

        /// <summary>
        /// Registers the queries from a path on the file system.
        /// </summary>
        /// <param name="startPath">The path of the file system in which to start.</param>
        /// <param name="processQueryFile">a delegate that processes the file containing
        /// the queries and creates a set of NamedQuery objects.</param>
        public void RegisterQueriesFromPath(string startPath, 
                                            Func<FileInfo, IEnumerable<NamedQuery>> processQueryFile)
        {
            RegisterQueriesFromPath(startPath, fileInfo => true, processQueryFile);
        }

        /// <summary>
        /// Registers the queries from a path on the file system using a selector to
        /// determine the files upon which to process.
        /// </summary>
        /// <param name="startPath">The path of the file system in which to start.</param>
        /// <param name="fileSelector">The file selector to determine which files to process.</param>
        /// <param name="processQueryFile">a delegate that processes the file containing
        /// the queries and creates a set of NamedQuery objects.</param>
        public void RegisterQueriesFromPath(string startPath, 
                                            Predicate<FileInfo> fileSelector, 
                                            Func<FileInfo, IEnumerable<NamedQuery>> processQueryFile)
        {
            if ( String.IsNullOrEmpty(startPath) )
            {
                throw new ArgumentNullException("startPath");
            }
            var files = Directory.GetFiles(startPath, "*", SearchOption.AllDirectories);
            RegisterQueriesFromFiles(files, fileSelector, processQueryFile);
        }

        /// <summary>
        /// Registers the queries from a set of files.
        /// </summary>
        /// <param name="files">The files with queries in them.</param>
        /// <param name="processQueryFile">a delegate that processes the file containing
        /// the queries and creates a set of NamedQuery objects.</param>
        public void RegisterQueriesFromFiles(IEnumerable<string> files, 
                                             Func<FileInfo, IEnumerable<NamedQuery>> processQueryFile)
        {
            RegisterQueriesFromFiles(files, fileInfo => true, processQueryFile);
        }

        /// <summary>
        /// Registers the queries from a set of files.
        /// </summary>
        /// <param name="files">The files with queries in them.</param>
        /// <param name="fileSelector">The file selector to determine which files to process.</param>
        /// <param name="processQueryFile">a delegate that processes the file containing
        /// the queries and creates a set of NamedQuery objects.</param>
        public void RegisterQueriesFromFiles(IEnumerable<string> files, 
                                             Predicate<FileInfo> fileSelector, 
                                             Func<FileInfo, IEnumerable<NamedQuery>> processQueryFile)
        {
            if ( files != null )
            {
                using ( new LockHolder<object>(_lockHandle, TIMEOUT) )
                {
                    foreach ( var file in files )
                    {
                        if ( File.Exists(file) )
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            if ( fileSelector(fileInfo) )
                            {
                                var queries = processQueryFile(fileInfo);
                                if ( queries != null )
                                {
                                    foreach ( var query in queries )
                                    {
                                        RegisterQuery(query);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registers queries from files with a .sql extension.  This method starts at the
        /// given directory and traverses all subdirectories to find files ending in .sql.
        /// Any matching file will be used to populate its contents in the registry and
        /// the name of the file (without the extension) will be used as the key.
        /// </summary>
        /// <param name="startPath">The start path.</param>
        public void RegisterQueriesFromSqlFiles(string startPath)
        {
            RegisterQueriesFromPath(
                startPath,
                fileInfo => fileInfo.Extension.Contains(".sql"),
                fileInfo =>
                {
                    string queryName = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.'));
                    string sql = File.ReadAllText(fileInfo.FullName);
                    return new NamedQuery[] { new NamedQuery(queryName, sql) };
                });
        }

        /// <summary>
        /// Registers the query.
        /// </summary>
        /// <param name="query">the NamedQuery to register</param>
        public void RegisterQuery(NamedQuery query)
        {
            if ( query == null )
            {
                throw new ArgumentNullException("query");
            }
            RegisterQuery(query.Name, query.Sql);
        }

        /// <summary>
        /// Registers the query.
        /// </summary>
        /// <param name="name">The name of the query.</param>
        /// <param name="sql">The SQL.</param>
        public void RegisterQuery(string name, string sql)
        {
            if ( String.IsNullOrEmpty(name) )
            {
                throw new ArgumentNullException("name");
            }
            using ( new LockHolder<object>(_lockHandle, TIMEOUT) )
            {
                _queries[name] = sql;
            }
        }

        /// <summary>
        /// Determines whether the registry contains a query registered
        /// with the specified name.
        /// </summary>
        /// <param name="name">The name of the query.</param>
        /// <returns>
        /// 	<c>true</c> if the registry contains the named query; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsQuery(string name)
        {
            if ( String.IsNullOrEmpty(name) )
            {
                throw new ArgumentNullException("name");
            }
            using ( new LockHolder<object>(_lockHandle, TIMEOUT) )
            {
                return _queries.ContainsKey(name);
            }
        }

        /// <summary>
        /// Finds the query by name.
        /// </summary>
        /// <param name="name">The name of the query.</param>
        /// <returns>the SQL.</returns>
        public string FindQuery(string name)
        {
            if ( String.IsNullOrEmpty(name) )
            {
                throw new ArgumentNullException("name");
            }
            using ( new LockHolder<object>(_lockHandle, TIMEOUT) )
            {
                if ( !_queries.ContainsKey(name) )
                {
                    throw new QueryNotFoundException(
                        String.Format("Could not find query registered with name '{0}'", name));
                }
                return _queries[name];
            }
        }

        /// <summary>
        /// Removes the query.
        /// </summary>
        /// <param name="name">The name of the query.</param>
        public bool RemoveQuery(string name)
        {
            if ( String.IsNullOrEmpty(name) )
            {
                throw new ArgumentNullException("name");
            }
            using ( new LockHolder<object>(_lockHandle, TIMEOUT) )
            {
                if ( !_queries.ContainsKey(name) )
                {
                    throw new QueryNotFoundException(
                        String.Format("Could not find query registered with name '{0}'", name));
                }
                return _queries.Remove(name);
            }
        }

        /// <summary>
        /// Clears the named queries.
        /// </summary>
        public void Clear()
        {
            using ( new LockHolder<object>(_lockHandle, TIMEOUT) )
            {
                _queries.Clear();
            }
        }

    }
}
