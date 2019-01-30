using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Miado.Configuration
{
    /// <summary>
    /// This class implements the methods defined in the 
    /// <see cref="IParameterParser"/> interface by using the older 
    /// syntax found in ODBC, Ole, etc. (i.e. uses ? for parameters
    /// instead of the traditional @Param syntax)
    /// </summary>
    public class LegacyParameterParser : IParameterParser
    {
        #region Members

        private static readonly Regex _reIsInsert = new Regex(@"^\s*insert\s+into\s+", RegexOptions.IgnoreCase);
        private static readonly Regex _reWhereParams = new Regex(@"([A-Za-z0-9_-]+)\s*[>=?<=?=]\s*\?", RegexOptions.IgnoreCase);
        private static readonly Regex _reInsertParams = new Regex(@"\(?\s*(\?|[A-Za-z0-9_-]+|'.*')\s*(,|\))", RegexOptions.IgnoreCase);
        private static readonly Regex _reParam = new Regex(@"\?", RegexOptions.IgnoreCase);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LegacyParameterParser"/> class.
        /// </summary>
        public LegacyParameterParser() { }

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

            if ( IsInsertSql(sql) )
            {
                MatchCollection matches = _reInsertParams.Matches(sql);

                // need an even number of columns and parameters
                if ( matches.Count % 2 == 0 )
                {
                    // first half are column names, second half are the values
                    int halfWayPoint = matches.Count / 2;
                    for ( int i = 0; i < halfWayPoint; i++ )
                    {
                        if ( matches[i + halfWayPoint].Groups[1].Value.Equals("?") )
                        {
                            paramNames.Add(String.Format(CultureInfo.InvariantCulture, "@{0}", matches[i].Groups[1].Value));
                        }
                    }
                }
            }
            else
            {
                foreach ( Match match in _reWhereParams.Matches(sql + " ") )
                {
                    paramNames.Add(String.Format(CultureInfo.InvariantCulture, "@{0}", match.Groups[1].Value.Trim()));
                }
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
            return _reParam.Match(sql).Groups[0];
        }

        /// <summary>
        /// Replaces the parameter syntax.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="sourceParameterParser">the IParameterParser
        /// that represents the format of the SQL in the source code</param>
        /// <returns>the formatted SQL</returns>
        public string ReplaceParameterSyntax(string sql, IParameterParser sourceParameterParser)
        {
            if ( sql == null )
            {
                throw new ArgumentNullException("sql");
            }
            string modSql = sql;
            if ( sourceParameterParser != null )
            {
                sql += " ";

                // use the original syntax parser to find the next parameter 
                Group group = sourceParameterParser.NextParameterMatch(sql);
                if ( group != null && group.Success )
                {
                    // replace it with a "?"
                    StringBuilder sb = new StringBuilder();
                    sb.Append(sql.Substring(0, group.Index))
                      .Append("?")
                      .Append(sql.Substring(group.Index + group.Length));

                    modSql =
                        this.ReplaceParameterSyntax(sb.ToString(), sourceParameterParser);
                }
            }

            return modSql.Trim();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines whether this SQL is an insert statement 
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>
        /// 	<c>true</c> if the SQL is an insert statement; 
        /// 	otherwise, <c>false</c>.
        /// </returns>
        private static bool IsInsertSql(string sql)
        {
            return _reIsInsert.IsMatch(sql);
        }

        #endregion
    }
}
