using System;
using System.Runtime.Serialization;

namespace Miado.Query
{
    /// <summary>
    /// This exception class contains information about unsuccessfully 
    /// attempting to retrieve a query by name from the Query Registry
    /// </summary>
    public class QueryNotFoundException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryNotFoundException"/> class.
        /// </summary>
        public QueryNotFoundException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public QueryNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public QueryNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that 
        /// holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> 
        /// that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected QueryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion
    }
}
