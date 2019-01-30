using System;
using System.Collections.Generic;
using System.IO;

namespace Miado.Query
{
	/// <summary>
	/// This interface defines the methods used to 
	/// store and retrieve queries by name
	/// </summary>
	public interface IQueryRegistry
	{
        /// <summary>
        /// Gets the count of queries that have been registered
        /// </summary>
        /// <value>The count of queries that have been registered.</value>
        int Count { get; }

        /// <summary>
        /// Registers the queries from a path on the file system.
        /// </summary>
        /// <param name="startPath">The path of the file system in which to start.</param>
        /// <param name="processQueryFile">a delegate that processes the file containing
        /// the queries and creates a set of NamedQuery objects.</param>
        void RegisterQueriesFromPath(string startPath, 
                                     Func<FileInfo, IEnumerable<NamedQuery>> processQueryFile);

        /// <summary>
        /// Registers the queries from a path on the file system using a selector to 
        /// determine the files upon which to process.
        /// </summary>
        /// <param name="startPath">The path of the file system in which to start.</param>
        /// <param name="fileSelector">The file selector to determine which files to process.</param>
        /// <param name="processQueryFile">a delegate that processes the file containing
        /// the queries and creates a set of NamedQuery objects.</param>
        void RegisterQueriesFromPath(string startPath,
                                     Predicate<FileInfo> fileSelector,
                                     Func<FileInfo, IEnumerable<NamedQuery>> processQueryFile);

        /// <summary>
        /// Registers the queries from a set of files.
        /// </summary>
        /// <param name="files">The files with queries in them.</param>
        /// <param name="processQueryFile">a delegate that processes the file containing
        /// the queries and creates a set of NamedQuery objects.</param>
        void RegisterQueriesFromFiles(IEnumerable<string> files,
                                      Func<FileInfo, IEnumerable<NamedQuery>> processQueryFile);

        /// <summary>
        /// Registers the queries from a set of files.
        /// </summary>
        /// <param name="files">The files with queries in them.</param>
        /// <param name="fileSelector">The file selector to determine which files to process.</param>
        /// <param name="processQueryFile">a delegate that processes the file containing
        /// the queries and creates a set of NamedQuery objects.</param>
        void RegisterQueriesFromFiles(IEnumerable<string> files,
                                      Predicate<FileInfo> fileSelector,
                                      Func<FileInfo, IEnumerable<NamedQuery>> processQueryFile);

        /// <summary>
        /// Registers queries from files with a .sql extension.  This method starts at the 
        /// given directory and traverses all subdirectories to find files ending in .sql.
        /// Any matching file will be used to populate its contents in the registry and 
        /// the name of the file (without the extension) will be used as the key.
        /// </summary>
        /// <param name="startPath">The start path.</param>
        void RegisterQueriesFromSqlFiles(string startPath);

        /// <summary>
        /// Registers the query.
        /// </summary>
        /// <param name="query">the NamedQuery to register</param>
        void RegisterQuery(NamedQuery query);

		/// <summary>
		/// Registers the query.
		/// </summary>
		/// <param name="name">The name of the query.</param>
		/// <param name="sql">The SQL.</param>
		void RegisterQuery(string name, string sql);

        /// <summary>
        /// Determines whether the registry contains a query registered 
        /// with the specified name.
        /// </summary>
        /// <param name="name">The name of the query.</param>
        /// <returns>
        /// 	<c>true</c> if the registry contains the named query; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsQuery(string name);

		/// <summary>
		/// Finds the query by name.
		/// </summary>
		/// <param name="name">The name of the query.</param>
		/// <returns>the SQL mapped to the query name.</returns>
		string FindQuery(string name);

		/// <summary>
		/// Removes the query.
		/// </summary>
		/// <param name="name">The name of the query.</param>
		bool RemoveQuery(string name);

		/// <summary>
		/// Clears the named queries.
		/// </summary>
		void Clear();
	}
}
