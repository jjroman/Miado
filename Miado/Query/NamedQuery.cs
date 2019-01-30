using System;

namespace Miado.Query
{
	/// <summary>
	/// This class contains the information for a named query mapping to 
	/// SQL.
	/// </summary>
    public class NamedQuery
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="NamedQuery"/> class.
		/// </summary>
		/// <param name="name">The name of the query.</param>
		/// <param name="sql">The SQL.</param>
        public NamedQuery(string name, string sql)
        {
            Name = name;
            Sql = sql;
        }

		/// <summary>
		/// Gets or sets the query name.
		/// </summary>
		/// <value>The query name.</value>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the SQL.
		/// </summary>
		/// <value>The SQL.</value>
		public string Sql
		{
			get;
			set;
		}
    }
}
