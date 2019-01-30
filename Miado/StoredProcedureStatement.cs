using System.Data;

namespace Miado
{
    /// <summary>
    /// This internal class extends DbStatement to represent
    /// a stored procedure statement.
    /// </summary>
    class StoredProcedureStatement : DbStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureStatement"/> class.
        /// </summary>
        /// <param name="db">The IDatabase instance.</param>
        /// <param name="storedProcName">The name of the stored procedure.</param>
        public StoredProcedureStatement(IDatabase db, string storedProcName) : base(db, storedProcName) { }

        /// <summary>
        /// DbCommand.CommandType.StoredProcedure
        /// </summary>
        /// <value></value>
        public override CommandType CommandType
        {
            get { return CommandType.StoredProcedure; }
        }
    }
}
