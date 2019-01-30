using System;
using System.Collections.Generic;
using System.Threading;

namespace Miado.Threading
{
    /// <summary>
    /// This class provides a mechanism for caching data for 
    /// a given time.  You provide it with a delegate that will 
    /// be used to re-populate itself along with a time-to-live 
    /// in milliseconds.
    /// </summary>
    /// <typeparam name="T">the type of object to be cached</typeparam>
    public class CachedData<T>
    {
        #region Members

        private int _ttl;
        private static readonly int _timeout = 60 * 1000; // 60 seconds
        private T _cache; // the actual data that is cached
        private DateTime _lastUpdatedTs = DateTime.MinValue;
        private readonly object _handle = new object();

        #endregion

        #region Delegates

        /// <summary>
        /// No-arg method used to populate the data in the cache
        /// </summary>
        /// <returns>the object that is cached</returns>      
        private Func<T> _populatingDelegate;

        #endregion

        #region Constructors

        /// <summary>
        /// No-arg constructor for sub-classes
        /// </summary>
        protected CachedData() {}

        /// <summary>
        /// Constructor - the time-to-live will be defaulted to one hour
        /// </summary>
        /// <param name="populatingDelegate">a delegate that will be used to 
        /// populate the cache</param>
        public CachedData(Func<T> populatingDelegate) :
            this(populatingDelegate, 60 * 60 * 1000) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="populatingDelegate">a delegate that will be used to 
        /// populate the cache</param>
        /// <param name="timeToLive">the time-to-live in milliseconds</param>
        public CachedData(Func<T> populatingDelegate, int timeToLive)
        {
            this._populatingDelegate = populatingDelegate;
            this._ttl = timeToLive;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the time-to-live of the cache (in milliseconds)
        /// </summary>
        public int TimeToLive
        {
            get { return _ttl; }
            set { _ttl = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method is used to actually pull the data out of the underlying
        /// cache.  If the cache has expired, it will be refreshed by calling
        /// the delegate that is in charge of populating it.
        /// </summary>
        /// <returns>the object that is cached</returns>
        public T RetrieveData()
        {
            using ( var cacheLock = new LockHolder<object>(_handle, _timeout) )
            {
                double millisPassed = DateTime.Now.Subtract(_lastUpdatedTs).TotalMilliseconds;
                if ( millisPassed > _ttl )
                {
                    // cache is expired - (re)populate the cache
                    _cache = this.InvokeCachePopulater();
                    this._lastUpdatedTs = DateTime.Now;

                }
            }

            return _cache;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Invoke the delegate to actually populate the cache.  It is marked
        /// as virtual so that it can be overridden in sub-classes
        /// </summary>
        /// <returns>a collection of strongly-typed objects</returns>
        protected virtual T InvokeCachePopulater()
        {
            return this._populatingDelegate();
        }

        #endregion

    }
}