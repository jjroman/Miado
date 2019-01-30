using System.Data.SqlClient;

namespace Miado.Databases
{
    /// <summary>
    /// This class represents a SQLServer database.
    /// </summary>
	public class SqlServerDatabase : Database
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDatabase"/> class.
        /// </summary>
        /// <param name="connString">The connection string.</param>
		public SqlServerDatabase(string connString) : base(SqlClientFactory.Instance, connString) { }
	}
}
