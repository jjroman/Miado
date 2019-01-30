using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Miado.Configuration
{
    /// <summary>
    /// This class implements the methods defined in the 
    /// <see cref="IParameterParser"/> interface by using the standard 
    /// syntax found in most modern ADO.Net providers (e.g. SQL Server, 
    /// Oracle, etc.) by using the @Param syntax
    /// </summary>
    public class StandardParameterParser : IParameterParser
    {
        #region Members

        private static readonly Regex _regEx = new Regex(@"[^@](@([A-Za-z0-9_-]+))(\s|,|\))");

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardParameterParser"/> class.
        /// </summary>
        public StandardParameterParser() {}

        #endregion

        #region IParameterParser Members

        /// <summary>
        /// Parse the given SQL to extract the parameter names.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>a list of parameter names from the SQL</returns>
        public IList<string> FindParameterNames(string sql)
        {
            IList<string> paramNames = new List<string>();

            // match on @Param1
            foreach ( Match match in _regEx.Matches(sql + " ") )
            {
                paramNames.Add(match.Groups[2].Value.Trim());
            }

            return paramNames;
        }

        /// <summary>
        /// Using regular expressions, find the next parameter in the
        /// SQL statement
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>the next group of matching parameters</returns>
        public Group NextParameterMatch(string sql)
        {
            return _regEx.Match(sql).Groups[1];
        }

        /// <summary>
        /// Replaces the parameter syntax.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="sourceParameterParser">the IParameterParser
        /// that represents the format of the SQL in the source code</param>
        /// <returns>the formatted SQL.</returns>
        public string ReplaceParameterSyntax(string sql, IParameterParser sourceParameterParser)
        {
            if ( sql == null )
            {
                throw new ArgumentNullException("sql");
            }
            return sourceParameterParser != null ? this.ReplaceParameterSyntax(sql, sourceParameterParser, 0) : sql;
        }

        #endregion

        /// <summary>
        /// Replaces the parameter syntax.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="sourceParameterParser">The source parameter parser.</param>
        /// <param name="paramNbr">The param NBR.</param>
        /// <returns>the formatted SQL.</returns>
        private string ReplaceParameterSyntax(string sql, IParameterParser sourceParameterParser, int paramNbr)
        {
            string modSql = sql;
            if ( sourceParameterParser != null )
            {
                sql += " ";

                Group group = sourceParameterParser.NextParameterMatch(sql);
                if ( group != null && group.Success )
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(sql.Substring(0, group.Index))
                      .AppendFormat("@p{0}", paramNbr)
                      .Append(sql.Substring(group.Index + group.Length));
                    modSql = this.ReplaceParameterSyntax(sb.ToString(), sourceParameterParser, ++paramNbr);
                }
            }

            return modSql.Trim();
        }
    }
}
