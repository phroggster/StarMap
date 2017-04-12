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

#region --- using ... ---
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
#endregion // --- using ... ---

namespace StarMap.Database
{
    /// <summary>
    /// A <see cref="DbDataReader"/> wrapper utilizing strong locking. See EDDiscovery.DB.SQLiteDataReaderED (SQLiteCommandED.cs)
    /// </summary>
    /// <seealso cref="ADBConnection"/>
    /// <seealso cref="Command{TConn}"/>
    /// <seealso cref="Transaction{TConn}"/>
    /// <seealso cref="TransactionLock{TConn}"/>
    public class DataReader<TConn> : DbDataReader
        where TConn : ADBConnection
    {
        public DataReader(DbCommand cmd, CommandBehavior behavior, Transaction<TConn> txn = null, TransactionLock<TConn> txnlock = null)
        {
            _command = cmd;
            InnerReader = cmd.ExecuteReader(behavior);
            _transaction = txn;
            _txnlock = txnlock;
        }

        public override void Close()
        {
            InnerReader.Close();
            _txnlock?.CloseReader();
            _txnlock = null;
        }

        public override IEnumerator GetEnumerator()
        {
            BeginCommand();
            foreach(object val in InnerReader)
            {
                EndCommand();
                yield return val;
                BeginCommand();
            }
            EndCommand();
        }

        public override bool NextResult()
        {
            BeginCommand();
            bool result = InnerReader.NextResult();
            EndCommand();
            return result;
        }

        public override bool Read()
        {
            BeginCommand();
            bool result = InnerReader.Read();
            EndCommand();
            return result;
        }

        #region Overridden methods and properties passed to inner command
        public override int Depth { get { return InnerReader.Depth; } }
        public override int FieldCount { get { return InnerReader.FieldCount; } }
        public override bool HasRows { get { return InnerReader.HasRows; } }
        public override bool IsClosed { get { return InnerReader.IsClosed; } }
        public override int RecordsAffected { get { return InnerReader.RecordsAffected; } }
        public override int VisibleFieldCount { get { return InnerReader.VisibleFieldCount; } }
        public override object this[int ordinal] { get { return InnerReader[ordinal]; } }
        public override object this[string name] { get { return InnerReader[name]; } }
        public override bool GetBoolean(int ordinal) { return InnerReader.GetBoolean(ordinal); }
        public override byte GetByte(int ordinal) { return InnerReader.GetByte(ordinal); }
        public override char GetChar(int ordinal) { return InnerReader.GetChar(ordinal); }
        public override string GetDataTypeName(int ordinal) { return InnerReader.GetDataTypeName(ordinal); }
        public override DateTime GetDateTime(int ordinal) { return InnerReader.GetDateTime(ordinal); }
        public override decimal GetDecimal(int ordinal) { return InnerReader.GetDecimal(ordinal); }
        public override double GetDouble(int ordinal) { return InnerReader.GetDouble(ordinal); }
        public override Type GetFieldType(int ordinal) { return InnerReader.GetFieldType(ordinal); }
        public override float GetFloat(int ordinal) { return InnerReader.GetFloat(ordinal); }
        public override Guid GetGuid(int ordinal) { return InnerReader.GetGuid(ordinal); }
        public override short GetInt16(int ordinal) { return InnerReader.GetInt16(ordinal); }
        public override int GetInt32(int ordinal) { return InnerReader.GetInt32(ordinal); }
        public override long GetInt64(int ordinal) { return InnerReader.GetInt64(ordinal); }
        public override string GetName(int ordinal) { return InnerReader.GetName(ordinal); }
        public override string GetString(int ordinal) { return InnerReader.GetString(ordinal); }
        public override object GetValue(int ordinal) { return InnerReader.GetValue(ordinal); }
        public override bool IsDBNull(int ordinal) { return InnerReader.IsDBNull(ordinal); }
        public override int GetOrdinal(string name) { return InnerReader.GetOrdinal(name); }
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) { return InnerReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length); }
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) { return InnerReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length); }
        public override int GetValues(object[] values) { return InnerReader.GetValues(values); }
        public override DataTable GetSchemaTable() { return InnerReader.GetSchemaTable(); }

        #endregion


        #region Protected implementation

        protected DbDataReader InnerReader { get; set; }
        protected DbCommand _command;
        protected Transaction<TConn> _transaction;
        protected TransactionLock<TConn> _txnlock;

        protected void BeginCommand()
        {
            if (_transaction != null)
                _transaction.BeginCommand(_command);
            else if (_txnlock != null)
                _txnlock.BeginCommand(_command);
        }

        protected void EndCommand()
        {
            if (_transaction != null)
                _transaction.EndCommand();
            else if (_txnlock != null)
                _txnlock.EndCommand();
        }

        #endregion // Protected implementation
    }
}
