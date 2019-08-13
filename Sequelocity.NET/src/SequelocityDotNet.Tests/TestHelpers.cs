using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace SequelocityDotNet.Tests
{
    public class TestHelpers
    {
        /// <summary>
        /// Gets a <see cref="DbCommand"/> for testing purposes.
        /// </summary>
        /// <returns><see cref="DbCommand"/> instance.</returns>
        public static DbCommand GetDbCommand()
        {
            var dbProviderFactory = DbProviderFactories.GetFactory( "System.Data.SqlClient" );

            var connection = dbProviderFactory.CreateConnection();

            var dbCommand = connection.CreateCommand();

            return dbCommand;
        }

        /// <summary>
        /// Gets a <see cref="DbCommand"/> for testing purposes.
        /// </summary>
        /// <returns><see cref="DbCommand"/> instance.</returns>
        public static DatabaseCommand GetDatabaseCommand()
        {
            return Sequelocity.GetDatabaseCommandForSqlServer( "Server=MyServerAddress;Database=MyDataBase;Trusted_Connection=True;" );
        }

        /// <summary>
        /// Implementation of an <see cref="IDataRecord"/> for testing purposes.
        /// </summary>
        public class DataRecord : IDataRecord
        {
            private readonly IList<KeyValuePair<string, object>> _keyValuePairs;

            public DataRecord( IList<KeyValuePair<string, object>> keyValuePairs )
            {
                _keyValuePairs = keyValuePairs;
            }

            #region IDataRecord Members

            public int FieldCount
            {
                get { return _keyValuePairs.Count; }
            }

            public bool GetBoolean( int i )
            {
                throw new NotImplementedException();
            }

            public byte GetByte( int i )
            {
                throw new NotImplementedException();
            }

            public long GetBytes( int i, long fieldOffset, byte[] buffer, int bufferoffset, int length )
            {
                throw new NotImplementedException();
            }

            public char GetChar( int i )
            {
                throw new NotImplementedException();
            }

            public long GetChars( int i, long fieldoffset, char[] buffer, int bufferoffset, int length )
            {
                throw new NotImplementedException();
            }

            public IDataReader GetData( int i )
            {
                throw new NotImplementedException();
            }

            public string GetDataTypeName( int i )
            {
                throw new NotImplementedException();
            }

            public DateTime GetDateTime( int i )
            {
                throw new NotImplementedException();
            }

            public decimal GetDecimal( int i )
            {
                throw new NotImplementedException();
            }

            public double GetDouble( int i )
            {
                throw new NotImplementedException();
            }

            public Type GetFieldType( int i )
            {
                throw new NotImplementedException();
            }

            public float GetFloat( int i )
            {
                throw new NotImplementedException();
            }

            public Guid GetGuid( int i )
            {
                throw new NotImplementedException();
            }

            public short GetInt16( int i )
            {
                throw new NotImplementedException();
            }

            public int GetInt32( int i )
            {
                throw new NotImplementedException();
            }

            public long GetInt64( int i )
            {
                throw new NotImplementedException();
            }

            public string GetName( int i )
            {
                return _keyValuePairs[i].Key;
            }

            public int GetOrdinal( string name )
            {
                throw new NotImplementedException();
            }

            public string GetString( int i )
            {
                throw new NotImplementedException();
            }

            public object GetValue( int i )
            {
                return _keyValuePairs[i].Value;
            }

            public int GetValues( object[] values )
            {
                throw new NotImplementedException();
            }

            public bool IsDBNull( int i )
            {
                throw new NotImplementedException();
            }

            public object this[string name]
            {
                get { throw new NotImplementedException(); }
            }

            public object this[int i]
            {
                get { throw new NotImplementedException(); }
            }

            #endregion IDataRecord Members
        }
    }
}