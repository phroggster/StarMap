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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace StarMap.Database
{
    public enum DBAccessMode
    {
        Readonly,
        ReadWrite,
        Indeterminate
    }

    public enum DBSelection
    {
        Invalid,
        EDDCombined,
        EDDSystem,
        EDDUser,
        StarMap,
        Unknown
    }

    /// <summary>
    /// An abstract database connection base class. Based upon EDDiscovery.DB.SQLiteConnectionED{TConn} (SQLiteConnectionED.cs)
    /// </summary>
    public abstract class ADBConnection<TConn> : ADBConnection
        where TConn : ADBConnection, new()
    {
        public static Dictionary<string, RegisterEntry> EarlyRegister { get; protected set; }
        public static bool IsInitialized { get { return _initialized; } }
        public static bool IsReadWaiting { get { return TransactionLock<TConn>.IsReadWaiting; } }

        public class SchemaLock : IDisposable
        {
            public bool IsDisposed { get; private set; } = false;

            public SchemaLock()
            {
                if (_schemaLock.RecursiveReadCount != 0)
                    throw new InvalidOperationException("Cannot take a schema lock while holding an open database connection");

                _schemaLock.EnterWriteLock();
            }

            ~SchemaLock()
            {
#if DEBUG
                Debug.Print("[WARN] SchemaLock leaked. Did you forget to call Dispose()?");
#endif
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!IsDisposed && disposing && _schemaLock.IsWriteLockHeld)
                    _schemaLock.ExitWriteLock();
                IsDisposed = true;
            }
        }

        static public bool PutSettingBool(string key, bool boolvalue, TConn conn = null)
        {
            return RegisterPut(cn => cn.PutSettingBoolCN(key, boolvalue), conn);
        }

        static public bool PutSettingInt(string key, int intvalue, TConn conn = null)
        {
            return RegisterPut(cn => cn.PutSettingIntCN(key, intvalue), conn);
        }

        static public bool PutSettingString(string key, string strvalue, TConn conn = null)
        {
            return RegisterPut(cn => cn.PutSettingStringCN(key, strvalue), conn);
        }

        public ADBConnection(DBSelection? db = null, bool initializing = false, bool shortlived = true)
            : base(initializing)
        {
            bool locktaken = false;
            try
            {
                if (!initializing && !_initialized)
                {
                    TraceLog.Warn($"Database {typeof(TConn).Name} initialized before Initialize().");
                    TraceLog.Warn(new StackTrace(2, true).ToString());
                    if (typeof(TConn) == typeof(EDDSystemsDBConnection))
                        EDDSystemsDBConnection.Initialize();
                    else if (typeof(TConn) == typeof(EDDUserDBConnection))
                        EDDUserDBConnection.Initialize();
                    else if (typeof(TConn) == typeof(SMDBConnection))
                        SMDBConnection.Initialize();
                }

                _schemaLock.EnterReadLock();
                locktaken = true;

                DBFile = GetSQLiteDBFile(db ?? DBSelection.StarMap);
                _cn = DbFactory.CreateConnection();
                _cn.ConnectionString = "Data Source=" + DBFile + ";Pooling=true;DateTimeKind=Utc;";
                if (db.HasValue && (db.Value == DBSelection.EDDCombined || db.Value == DBSelection.EDDSystem || db.Value == DBSelection.EDDUser))
                    _cn.ConnectionString += "Read Only=True;";
                _transactionLock = new TransactionLock<TConn>();
                _cn.Open();
            }
            catch
            {
                if (_transactionLock != null)
                    _transactionLock.Dispose();
                if (locktaken)
                    _schemaLock.ExitReadLock();
                throw;
            }
        }

        public override DbTransaction BeginTransaction()
        {
            AssertThreadOwner();
            _transactionLock.OpenWriter();
            return new Transaction<TConn>(_cn.BeginTransaction(), _transactionLock);
        }

        public override DbTransaction BeginTransaction(IsolationLevel isolevel)
        {
            AssertThreadOwner();
            _transactionLock.OpenWriter();
            return new Transaction<TConn>(_cn.BeginTransaction(isolevel), _transactionLock);
        }

        public override DbCommand CreateCommand(string query, DbTransaction tn = null)
        {
            AssertThreadOwner();
            DbCommand cmd = _cn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            cmd.CommandText = query;
            return new Command<TConn>(cmd, this, _transactionLock, tn);
        }

        public override DbDataAdapter CreateDataAdapter(DbCommand cmd)
        {
            DbDataAdapter da = DbFactory.CreateDataAdapter();
            da.SelectCommand = cmd;
            return da;
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        #region Protected implementation

        protected static bool RegisterPut(Func<TConn, bool> action, TConn conn)
        {
            if (conn != null)
                return action(conn);

            if (!_initialized && !_schemaLock.IsWriteLockHeld)
                TraceLog.Error("Write to register before Initialize()");

            using (TConn cn = new TConn())
                return action(cn);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cn != null)
                {
                    if (_cn.State != ConnectionState.Closed)
                        _cn.Close();
                    _cn.Dispose();
                }
                if (_schemaLock.IsReadLockHeld)
                    _schemaLock.ExitReadLock();

                _transactionLock?.Dispose();
            }
            _cn = null;
            _transactionLock = null;
            base.Dispose(disposing);
        }

        protected void GetRegister(Dictionary<string, RegisterEntry> regs)
        {
            using (DbCommand cmd = CreateCommand("SELECT Id, ValueInt, ValueDouble, ValueBlob, ValueString FROM register"))
            {
                using (DbDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string id = (string)rdr["Id"];
                        object valint = rdr["ValueInt"];
                        object valdbl = rdr["ValueDouble"];
                        object valblob = rdr["ValueBlob"];
                        object valstr = rdr["ValueString"];
                        regs[id] = new RegisterEntry(
                            valstr as string,
                            valblob as byte[],
                            (valint as long?) ?? 0L,
                            (valdbl as double?) ?? Double.NaN
                        );
                    }
                }
            }
        }

        protected static void InitializeIfNeeded(Action initializer)
        {
            if (!_initialized)
            {
                var cur = Interlocked.Increment(ref _initsem);
                if (cur == 1)
                {
                    using (var slock = new SchemaLock())
                    {
                        _initbarrier.Set();
                        initializer();
                        _initialized = true;
                    }
                }
                if (!_initialized)
                    _initbarrier.WaitOne();
            }
        }

        #endregion // Protected implementation

        #region Private implementation

        private static ReaderWriterLockSlim _schemaLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private static bool _initialized = false;
        private static int _initsem = 0;
        private static ManualResetEvent _initbarrier = new ManualResetEvent(false);
        private TransactionLock<TConn> _transactionLock;

        #endregion // Private implementation
    }

    /// <summary>
    /// An abstract database connection base class. Based upon EDDiscovery.DB.SQLiteConnectionED (SQLiteConnectionED.cs)
    /// </summary>
    public abstract class ADBConnection : IDisposable
    {
        public string DBFile { get; protected set; }


        public static string GetSQLiteDBFile(DBSelection selector)
        {
            if (selector == DBSelection.EDDUser)
                return Environment.ExpandEnvironmentVariables(Path.Combine("%LOCALAPPDATA%", "EDDiscovery", "EDDUser.sqlite"));
            else if (selector == DBSelection.EDDSystem)
                return Environment.ExpandEnvironmentVariables(Path.Combine("%LOCALAPPDATA%", "EDDiscovery", "EDDSystem.sqlite"));
            else if (selector == DBSelection.StarMap)
                return Environment.ExpandEnvironmentVariables(Path.Combine("%LOCALAPPDATA%", "StarMap", "starmap.sqlite"));
            else
                throw new InvalidOperationException("Invalid database type specified.");
        }

        public abstract DbTransaction BeginTransaction();
        public abstract DbTransaction BeginTransaction(IsolationLevel isolevel);
        public abstract DbCommand CreateCommand(string query, DbTransaction tn = null);
        public abstract DbDataAdapter CreateDataAdapter(DbCommand cmd);
        public abstract void Dispose();

        public void ExecuteNonQuery(string nonQuery)
        {
            using (DbCommand command = CreateCommand(nonQuery))
                command.ExecuteNonQuery();
        }

        public bool keyExistsCN(string sKey)
        {
            using (DbCommand cmd = CreateCommand("select ID from Register WHERE ID=@key"))
            {
                cmd.AddParameterWithValue("@key", sKey);
                return cmd.ExecuteScalar() != null;
            }
        }

        public int GetSettingIntCN(string key, int defaultvalue)
        {
            try
            {
                using (DbCommand cmd = CreateCommand("SELECT ValueInt from Register WHERE ID = @ID"))
                {
                    cmd.AddParameterWithValue("@ID", key);

                    object ob = cmd.ExecuteScalar();

                    if (ob == null || ob == DBNull.Value)
                        return defaultvalue;

                    int val = Convert.ToInt32(ob);

                    return val;
                }
            }
            catch
            {
                return defaultvalue;
            }
        }

        public bool PutSettingIntCN(string key, int intvalue)
        {
            try
            {
                if (keyExistsCN(key))
                {
                    using (DbCommand cmd = CreateCommand("Update Register set ValueInt = @ValueInt Where ID=@ID"))
                    {
                        cmd.AddParameterWithValue("@ID", key);
                        cmd.AddParameterWithValue("@ValueInt", intvalue);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                else
                {
                    using (DbCommand cmd = CreateCommand("Insert into Register (ID, ValueInt) values (@ID, @valint)"))
                    {
                        cmd.AddParameterWithValue("@ID", key);
                        cmd.AddParameterWithValue("@valint", intvalue);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public double GetSettingDoubleCN(string key, double defaultvalue)
        {
            try
            {
                using (DbCommand cmd = CreateCommand("SELECT ValueDouble from Register WHERE ID = @ID"))
                {
                    cmd.AddParameterWithValue("@ID", key);

                    object ob = cmd.ExecuteScalar();

                    if (ob == null || ob == DBNull.Value)
                        return defaultvalue;

                    double val = Convert.ToDouble(ob);

                    return val;
                }
            }
            catch
            {
                return defaultvalue;
            }
        }

        public bool PutSettingDoubleCN(string key, double doublevalue)
        {
            try
            {
                if (keyExistsCN(key))
                {
                    using (DbCommand cmd = CreateCommand("Update Register set ValueDouble = @ValueDouble Where ID=@ID"))
                    {
                        cmd.AddParameterWithValue("@ID", key);
                        cmd.AddParameterWithValue("@ValueDouble", doublevalue);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                else
                {
                    using (DbCommand cmd = CreateCommand("Insert into Register (ID, ValueDouble) values (@ID, @valdbl)"))
                    {
                        cmd.AddParameterWithValue("@ID", key);
                        cmd.AddParameterWithValue("@valdbl", doublevalue);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool GetSettingBoolCN(string key, bool defaultvalue)
        {
            return GetSettingIntCN(key, defaultvalue ? 1 : 0) != 0;
        }

        public bool PutSettingBoolCN(string key, bool boolvalue)
        {
            return PutSettingIntCN(key, boolvalue ? 1 : 0);
        }

        public string GetSettingStringCN(string key, string defaultvalue)
        {
            try
            {
                using (DbCommand cmd = CreateCommand("SELECT ValueString from Register WHERE ID = @ID"))
                {
                    cmd.AddParameterWithValue("@ID", key);
                    object ob = cmd.ExecuteScalar();

                    if (ob == null || ob == DBNull.Value)
                        return defaultvalue;

                    string val = (string)ob;

                    return val;
                }
            }
            catch
            {
                return defaultvalue;
            }
        }

        public bool PutSettingStringCN(string key, string strvalue)
        {
            try
            {
                if (keyExistsCN(key))
                {
                    using (DbCommand cmd = CreateCommand("Update Register set ValueString = @ValueString Where ID=@ID"))
                    {
                        cmd.AddParameterWithValue("@ID", key);
                        cmd.AddParameterWithValue("@ValueString", strvalue);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                else
                {
                    using (DbCommand cmd = CreateCommand("Insert into Register (ID, ValueString) values (@ID, @valint)"))
                    {
                        cmd.AddParameterWithValue("@ID", key);
                        cmd.AddParameterWithValue("@valint", strvalue);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #region Protected implementation

        protected static List<ADBConnection> _openConnections = new List<ADBConnection>();
        protected static DbProviderFactory DbFactory = new SQLiteFactory();

        public DbConnection _cn;
        protected Thread _owningThread;

        protected ADBConnection(bool initializing)
        {
            lock (_openConnections)
                _openConnections.Add(this);
            _owningThread = Thread.CurrentThread;
        }


        protected static void ExecuteNonQuery(ADBConnection conn, string nonQuery)
        {
            conn.ExecuteNonQuery(nonQuery);
        }


        protected void AssertThreadOwner()
        {
            if (Thread.CurrentThread != _owningThread)
                throw new InvalidOperationException($"DB connection was passed between threads.  Owning thread: {_owningThread.Name} ({_owningThread.ManagedThreadId}); this thread: {Thread.CurrentThread.Name} ({Thread.CurrentThread.ManagedThreadId})");
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (_openConnections)
                ADBConnection._openConnections.Remove(this);
        }

        #endregion // Protected implementation
    }
}
