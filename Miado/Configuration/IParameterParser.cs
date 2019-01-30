using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Miado.Configuration
{
    /// <summary>
    /// The interface defines the method for determining parameter
    /// syntax defined by a given provider
    /// </summary>
    public interface IParameterParser
    {
        /// <summary>
        /// Parse the given SQL to extract the parameter names.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>a list of parameter names from the SQL</returns>
        IList<string> FindParameterNames(string sql);

        /// <summary>
        /// Using regular expressions, find the next parameter in the 
        /// SQL statement
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>the next group of matching parameters</returns>
        Group NextParameterMatch(string sql);

        /// <summary>
        /// Replaces the parameter syntax.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="sourceParameterParser">the IParameterParser 
        /// that represents the format of the SQL in the source code</param>
        /// <returns>the formatted SQL</returns>
        string ReplaceParameterSyntax(string sql, IParameterParser sourceParameterParser);
    }
}
