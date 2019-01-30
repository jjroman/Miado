using System;
using System.Threading;

namespace Miado.Threading
{
    /// <summary>
    /// This helper class provides locking using the Monitor class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class LockHolder<T> : IDisposable
    {
        #region Members
        private T _handle;
        private bool _isDisposed = false;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LockHolder&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="handle">The handle object that will be locked.</param>
        /// <param name="timeoutInMillis">The timeout in milliseconds.</param>
        public LockHolder(T handle, int timeoutInMillis)
        {
            _handle = handle;
            IsLocked = Monitor.TryEnter(handle, timeoutInMillis);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="LockHolder&lt;T&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~LockHolder()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value><c>true</c> if this instance is locked; otherwise, <c>false</c>.</value>
        public bool IsLocked
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with 
        /// freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion 

        #region Private Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed 
        /// and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool isDisposing)
        {
            if ( !_isDisposed && isDisposing )
            {
                if ( IsLocked )
                {
                    Monitor.Exit(_handle);
                }
                IsLocked = false;
            }
            _isDisposed = true;
        }

        #endregion
    }
}
