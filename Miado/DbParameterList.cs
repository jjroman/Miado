using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Miado.Extensions;

namespace Miado
{
    /// <summary>
    ///     This class implements the methods defined in the
    ///     IParameterCollection interface for adding DbParameter
    ///     objects to the underlying collection.
    /// </summary>
    public class DbParameterList : IDbParameterList
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbParameterList" /> class.
        /// </summary>
        /// <param name="factory">
        ///     The DbProviderFactory that will be used
        ///     to create a DbParameter.
        /// </param>
        public DbParameterList(DbProviderFactory factory)
        {
            DbProviderFactory = factory;
            DbParameters = new List<DbParameter>();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return DbParameters.GetEnumerator();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the count of the parameter list.
        /// </summary>
        /// <value>The count of the list.</value>
        public int Count
        {
            get { return DbParameters.Count; }
        }

        /// <summary>
        ///     Gets or sets the internal list of DbParameter objects.
        /// </summary>
        /// <value>The internal list of DbParameter objects.</value>
        public IList<DbParameter> DbParameters { get; private set; }

        /// <summary>
        ///     Gets or sets the DbProviderFactory.
        /// </summary>
        /// <value>The DbProviderFactory.</value>
        private DbProviderFactory DbProviderFactory { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds a DbParameter to this collection that is populated
        ///     by a function.
        /// </summary>
        /// <param name="paramPopulater">
        ///     A function that populates an
        ///     empty DbParameter.
        /// </param>
        /// <returns>a reference to this object</returns>
        public IDbParameterList Add(Action<DbParameter> paramPopulater)
        {
            DbParameter dbParm = DbProviderFactory.CreateParameter();
            paramPopulater(dbParm);
            if (String.IsNullOrEmpty(dbParm.ParameterName))
            {
                throw new ArgumentException("DbParameter.ParameterName cannot be empty or null");
            }

            DbParameters.Add(dbParm);
            return this;
        }

        /// <summary>
        ///     Adds a DbParameter with a given parameter name
        ///     and corresponding value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value.</param>
        /// <returns>a reference to this object</returns>
        public IDbParameterList Add(string name, object value)
        {
            Add(name, value, ParameterDirection.Input);
            return this;
        }

        /// <summary>
        ///     Adds a DbParameter with a given parameter name
        ///     and corresponding value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">The DbType.</param>
        /// <returns>a reference to this object</returns>
        public IDbParameterList Add(string name, object value, DbType dbType)
        {
            Add(name, value, dbType, ParameterDirection.Input);
            return this;
        }

        /// <summary>
        ///     Adds a DbParameter with a given parameter name
        ///     and corresponding value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">The DbType.</param>
        /// <param name="direction">
        ///     The direction of the parameter
        ///     in the DB command (defaulted to input).
        /// </param>
        /// <returns>a reference to this object</returns>
        public IDbParameterList Add(string name, object value, DbType dbType, ParameterDirection direction)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            DbParameter dbParam = DbProviderFactory.CreateParameter();
            dbParam.ParameterName = name;
            dbParam.Value = value;
            dbParam.DbType = dbType;
            dbParam.Direction = direction;
            DbParameters.Add(dbParam);
            return this;
        }

        /// <summary>
        ///     Adds a DbParameter with a given parameter name
        ///     and corresponding value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The value.</param>
        /// <param name="direction">
        ///     The direction of the parameter
        ///     in the DB command (defaulted to input).
        /// </param>
        /// <returns>a reference to this object</returns>
        public IDbParameterList Add(string name, object value, ParameterDirection direction)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            DbParameter dbParam = DbProviderFactory.CreateParameter();
            dbParam.ParameterName = name;
            dbParam.Value = value;
            dbParam.Direction = direction;
            DbParameters.Add(dbParam);
            return this;
        }

        /// <summary>
        ///     Adds an Enumeration of DbParameter objects to this
        ///     collection.
        /// </summary>
        /// <param name="dbParameters">An enumeration of DbParameter objects.</param>
        /// <returns>a reference to this object</returns>
        public IDbParameterList AddRange(IEnumerable<DbParameter> dbParameters)
        {
            DbParameters.AddEnumeration(dbParameters);
            return this;
        }

        /// <summary>
        ///     Determines whether parameter list contains a
        ///     DbParameter with the given name.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns>
        ///     <c>true</c> if the parameter list has a parameter with
        ///     the given name; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return DbParameters.Any(dbParm =>
                String.Compare(dbParm.ParameterName, name, StringComparison.Ordinal) == 0);
        }

        /// <summary>
        ///     Gets the <see cref="System.Data.Common.DbParameter" /> at the specified index.
        /// </summary>
        /// <value></value>
        public DbParameter this[int index]
        {
            get { return DbParameters[index]; }
            set { DbParameters[index] = value; }
        }

        /// <summary>
        ///     Gets the <see cref="System.Data.Common.DbParameter" /> with the specified name.
        /// </summary>
        /// <value></value>
        public DbParameter this[string name]
        {
            get
            {
                return DbParameters.First(dbParam =>
                    String.Compare(name, dbParam.ParameterName, StringComparison.OrdinalIgnoreCase) == 0);
            }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can
        ///     be used to iterate through the collection.
        /// </returns>
        public IEnumerator<DbParameter> GetEnumerator()
        {
            return DbParameters.GetEnumerator();
        }

        #endregion
    }
}