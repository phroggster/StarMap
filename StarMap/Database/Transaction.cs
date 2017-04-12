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
using System.Data;
using System.Data.Common;

namespace StarMap.Database
{
    /// <summary>
    /// A <see cref="DbTransaction"/> wrapper that utilizes strong locking. See EDDiscovery.DB.SQLiteTransactionED (SQLiteCommandED.cs)
    /// </summary>
    /// <seealso cref="ADBConnection"/>
    /// <seealso cref="Command{TConn}"/>
    /// <seealso cref="DataReader{TConn}"/>
    /// <seealso cref="TransactionLock{TConn}"/>
    public class Transaction<TConn> : DbTransaction
        where TConn : ADBConnection
    {
        public DbTransaction InnerTransaction { get; private set; }
        public bool IsDisposed { get; private set; } = false;
        public override IsolationLevel IsolationLevel { get { return InnerTransaction.IsolationLevel; } }

        protected override DbConnection DbConnection { get { return InnerTransaction.Connection; } }
        private TransactionLock<TConn> _transactionLock = null;

        public Transaction(DbTransaction txn, TransactionLock<TConn> txnlock)
        {
            _transactionLock = txnlock;
            InnerTransaction = txn;
        }


        public void BeginCommand(DbCommand cmd)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().ToString());

            _transactionLock.BeginCommand(cmd);
        }

        public void EndCommand()
        {
            _transactionLock.EndCommand();
        }


        public override void Commit()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().ToString());

            InnerTransaction.Commit();
        }

        public override void Rollback()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().ToString());

            InnerTransaction.Rollback();
        }


        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    InnerTransaction?.Dispose();
                    _transactionLock?.CloseWriter();
                }
                _transactionLock = null;
                InnerTransaction = null;
                IsDisposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
