#region --- Apache v2.0 license ---
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
#endregion // --- Apache v2.0 license ---

#region --- using ... --
using System;
using System.Data;
using System.Data.Common;
#endregion // --- using ... ---

namespace StarMap.Database
{
    /// <summary>
    /// A <see cref="DbCommand"/> wrapper utiling strong locking. See EDDiscovery.DB.SQLiteCommandED (SQLiteCommandED.cs)
    /// </summary>
    /// <seealso cref="ADBConnection"/>
    /// <seealso cref="DataReader{TConn}"/>
    /// <seealso cref="Transaction{TConn}"/>
    /// <seealso cref="TransactionLock{TConn}"/>
    public class Command<TConn> : DbCommand
        where TConn : ADBConnection
    {
        public override string CommandText { get { return InnerCommand.CommandText; } set { InnerCommand.CommandText = value; } }
        public override int CommandTimeout { get { return InnerCommand.CommandTimeout; } set { InnerCommand.CommandTimeout = value; } }
        public override CommandType CommandType { get { return InnerCommand.CommandType; } set { InnerCommand.CommandType = value; } }
        public override bool DesignTimeVisible { get { return InnerCommand.DesignTimeVisible; } set { InnerCommand.DesignTimeVisible = value; } }
        public DbCommand InnerCommand { get; set; }
        public override UpdateRowSource UpdatedRowSource { get { return InnerCommand.UpdatedRowSource; } set { InnerCommand.UpdatedRowSource = value; } }


        public Command(DbCommand cmd, ADBConnection conn, TransactionLock<TConn> txnlock, DbTransaction txn = null)
        {
            _connection = conn;
            _txnlock = txnlock;
            InnerCommand = cmd;
            if (txn != null)
                SetTransaction(txn);
        }


        public override void Cancel()
        {
            InnerCommand.Cancel();
        }

        public override int ExecuteNonQuery()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.BeginCommand(this);
                    return InnerCommand.ExecuteNonQuery();
                }
                finally
                {
                    _transaction.EndCommand();
                }
            }
            else
            {
                try
                {
                    _txnlock.OpenWriter();
                    _txnlock.BeginCommand(this);
                    return InnerCommand.ExecuteNonQuery();
                }
                finally
                {
                    _txnlock.EndCommand();
                    _txnlock.CloseWriter();
                }
            }
        }

        public override object ExecuteScalar()
        {
            try
            {
                _txnlock.OpenReader();
                _txnlock.BeginCommand(this);
                return InnerCommand.ExecuteScalar();
            }
            finally
            {
                _txnlock.EndCommand();
                _txnlock.CloseReader();
            }
        }

        public override void Prepare()
        {
            InnerCommand.Prepare();
        }

        #region Protected implementation

        protected ADBConnection _connection;
        protected Transaction<TConn> _transaction;
        protected TransactionLock<TConn> _txnlock;

        protected override DbConnection DbConnection
        {
            get
            {
                return InnerCommand.Connection;
            }
            set
            {
                throw new InvalidOperationException("Cannot change connection of command");
            }
        }
        protected override DbParameterCollection DbParameterCollection { get { return InnerCommand.Parameters; } }
        protected override DbTransaction DbTransaction { get { return _transaction; } set { SetTransaction(value); } }


        protected override DbParameter CreateDbParameter()
        {
            return InnerCommand.CreateParameter();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                InnerCommand?.Dispose();

            _connection = null;
            _transaction = null;
            _txnlock = null;
            InnerCommand = null;
            base.Dispose(disposing);
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            _txnlock.OpenReader();
            try
            {
                return new DataReader<TConn>(InnerCommand, behavior, txnlock: _txnlock);
            }
            catch
            {
                _txnlock.CloseReader();
                throw;
            }
        }

        protected void SetTransaction(DbTransaction txn)
        {
            if (txn == null || txn is Transaction<TConn>)
            {
                _transaction = txn as Transaction<TConn>;
                InnerCommand.Transaction = _transaction.InnerTransaction;
            }
            else
                throw new InvalidOperationException($"Expected a {typeof(Transaction<TConn>).FullName}; got a {txn.GetType().FullName}!");
        }

        #endregion
    }
}
