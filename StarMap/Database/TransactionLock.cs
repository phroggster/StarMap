/*
 * Copyright © 2017 phroggie <phroggster@gmail.com>, StarMap development team
 * Copyright © 2015 - 2016 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;

namespace StarMap.Database
{
    /// <summary>
    /// An more thorough locking mechanism for SQLite transactions. See EDDiscovery.DB.SQLiteTxnLockED (SQLiteCommandED.cs)
    /// </summary>
    /// <seealso cref="ADBConnection"/>
    /// <seealso cref="Command{TConn}"/>
    /// <seealso cref="DataReader{TConn}"/>
    /// <seealso cref="Transaction{TConn}"/>
    public class TransactionLock<TConn> : IDisposable
        where TConn : ADBConnection
    {
        public static bool IsReadWaiting { get { return _lock.IsWriteLockHeld && _readsWaiting > 0; } }

        public DbCommand ExecutingCommand;
        public bool IsCommandExecuting { get; protected set; } = false;
        public bool IsDisposed { get; private set; } = false;

        public TransactionLock()
        {
            _owningThread = Thread.CurrentThread;
        }

        ~TransactionLock()
        {
#if DEBUG
            Debug.WriteLine($"[WARN] Transaction lock leaked. Did you forget to call Dispose()?");
#endif
            Dispose(false);
        }


        #region Open and shut

        public void BeginCommand(DbCommand cmd)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().ToString());
            ExecutingCommand = cmd;
            _commandText = cmd.CommandText;
            IsCommandExecuting = true;

            if (_isLongRunning && !_longRunningLogged)
            {
                _isLongRunning = false;
                DebugLongRunningOperation(this);
                _longRunningLogged = true;
            }
        }

        public void OpenReader()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().ToString());
            else if (_owningThread != Thread.CurrentThread)
                throw new InvalidOperationException("Transaction lock passed between threads.");

            if (!_lock.IsWriteLockHeld && _isReader)
            {
                try
                {
                    Interlocked.Increment(ref _readsWaiting);
                    while (!_lock.TryEnterReadLock(1000))
                    {
                        TransactionLock<TConn> lockowner = _writeLockOwner;
                        if (lockowner != null)
                        {
                            Debug.WriteLine($"Thread {Thread.CurrentThread.Name} waiting for thread {lockowner._owningThread.Name} to close write lock.");
                            DebugLongRunningOperation(lockowner);
                        }
                    }
                    _isReader = true;
                }
                finally
                {
                    Interlocked.Decrement(ref _readsWaiting);
                }
            }
        }

        public void OpenWriter()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().ToString());
            else if (_owningThread != Thread.CurrentThread)
                throw new InvalidOperationException("Transaction lock passed between threads.");
            else if (_lock.IsReadLockHeld)
                throw new InvalidOperationException("Write attempted using a read-only connection.");

            if (!_isWriter)
            {
                try
                {
                    if (!_lock.IsUpgradeableReadLockHeld)
                    {
                        while (!_lock.TryEnterUpgradeableReadLock(1000))
                        {
                            TransactionLock<TConn> lockowner = _writeLockOwner;
                            if (lockowner != null)
                            {
                                Debug.WriteLine($"Thread {Thread.CurrentThread.Name} waiting for thread {lockowner._owningThread.Name} to close write lock.");
                                DebugLongRunningOperation(lockowner);
                            }
                        }
                        _isWriter = true;
                        _writeLockOwner = this;
                    }
                    while (!_lock.TryEnterWriteLock(1000))
                        Trace.WriteLine($"Thread {Thread.CurrentThread.Name} waiting for readers to finish.");
                }
                catch
                {
                    if (_isWriter)
                    {
                        if (_lock.IsWriteLockHeld)
                            _lock.ExitWriteLock();
                        if (_lock.IsUpgradeableReadLockHeld)
                            _lock.ExitUpgradeableReadLock();
                    }
                }
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void CloseReader()
        {
            if (_lock.IsReadLockHeld)
                _lock.ExitReadLock();
        }

        public void CloseWriter()
        {
            if (_lock.IsWriteLockHeld)
            {
                _lock.ExitWriteLock();
                if (!_lock.IsWriteLockHeld && _lock.IsUpgradeableReadLockHeld)
                    _lock.ExitUpgradeableReadLock();
            }
        }

        public void EndCommand()
        {
            IsCommandExecuting = false;
        }

        #endregion // Open and shut


        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (_owningThread != Thread.CurrentThread)
                    Debug.WriteLine("[WARN] Transaction lock attempting to be disposed of on incorrect thread!");
                else
                {
                    if (_isWriter)
                        CloseWriter();
                    else if (_isReader)
                        CloseReader();
                }
                IsDisposed = true;
            }
        }

        #endregion // IDisposable implementation


        #region Private implementation

        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private static int _readsWaiting;
        private static TransactionLock<TConn> _writeLockOwner;

        private string _commandText = null;
        private bool _isLongRunning = false;
        private bool _isWriter = false;
        private bool _isReader = false;
        private bool _longRunningLogged = false;
        private Thread _owningThread;

        private static void DebugLongRunningOperation(TransactionLock<TConn> txnlock)
        {
            if (txnlock != null)
            {
                txnlock._isLongRunning = true;
#if DEBUG
                if (txnlock.IsCommandExecuting)
                {
                    if (txnlock._isLongRunning)
                        Trace.WriteLine($"The following command is taking a long time to execute:\n{txnlock._commandText}");
                    if (txnlock._owningThread == Thread.CurrentThread)
                    {
                        StackTrace trace = new StackTrace(1, true);
                        Trace.WriteLine(trace.ToString());
                    }
                }
                else
                {
                    Trace.WriteLine($"The transaction lock has been held for a long time.");

                    if (txnlock._commandText != null)
                        Trace.WriteLine($"Last command to execute:\n{txnlock._commandText}");
                }
#endif
            }
        }

        #endregion // Private implementation
    }
}
