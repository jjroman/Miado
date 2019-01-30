using System.Data;

namespace Miado
{
    /// <summary>
    /// This internal class extends DbStatement to represent
    /// a SQL statement.
    /// </summary>
    class SqlStatement : DbStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStatement"/> class.
        /// </summary>
        /// <param name="db">The IDatabase instance.</param>
        /// <param name="sql">The SQL.</param>
        public SqlStatement(IDatabase db, string sql) : base(db, sql) { }

        /// <summary>
        /// DbCommand.CommandType.Text (i.e. SQL)
        /// </summary>
        /// <value></value>
        public override CommandType CommandType
        {
            get { return CommandType.Text; }
        }
    }
}
