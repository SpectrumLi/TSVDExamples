/*
    Sequelocity.NET v0.5.0

    Sequelocity.NET is a simple data access library for the Microsoft .NET
    Framework providing lightweight ADO.NET wrapper, object mapper, and helper
    functions. To find out more, visit the project home page at: 
    https://github.com/AmbitEnergyLabs/Sequelocity.NET

    The MIT License (MIT)

    Copyright (c) 2015 Ambit Energy. All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SequelocityDotNet
{
    /// <summary>
    /// Sequelocity.NET is a simple data access library providing lightweight ADO.NET wrapper, object mapper, and helper
    /// functions.
    /// </summary>
    /// <remarks>
    /// This class provides various factory methods for instantiating new <see cref="DatabaseCommand" /> objects as well as
    /// configuration settings for Sequelocity.NET.
    /// </remarks>
    public static class Sequelocity
    {
        /// <summary>Gets a <see cref="DatabaseCommand" /> given a <see cref="DbCommand" /> instance.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <returns>A new <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand GetDatabaseCommand( DbCommand dbCommand )
        {
            return new DatabaseCommand( dbCommand );
        }

        /// <summary>Gets a <see cref="DatabaseCommand" /> given a <see cref="DbConnection" /> instance.</summary>
        /// <param name="dbConnection"><see cref="DbConnection" /> instance.</param>
        /// <returns>A new <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbConnection" /> parameter is null.</exception>
        public static DatabaseCommand GetDatabaseCommand( DbConnection dbConnection )
        {
            return new DatabaseCommand( dbConnection );
        }

        /// <summary>Attempts to get a <see cref="DatabaseCommand" /> using several strategies.</summary>
        /// <remarks>
        /// This method attempts to use several strategies to locate a ConnectionString and DbProviderFactory in order to create a
        /// new <see cref="DatabaseCommand" /> instance. The recommended option is to supply to this method a Connection String
        /// Name where the connection string setting in the application's configuration file also contains a populated providerName
        /// attribute specifying the DbProviderFactory invariant name to be used to instantiate a
        /// <see cref="ConfigurationSettings.Default" />. Another recommend option is to specify default values in the
        /// <see cref="ConfigurationSettings.Default" /> class that allows this method to be called with no parameters which
        /// simplifies callers to this method.
        /// </remarks>
        /// <param name="connectionStringOrName">Optional connection string or connection string name.</param>
        /// <param name="dbProviderFactoryInvariantName">Optional DbProviderFactory invariant name.</param>
        /// <returns>A new <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="Exception">
        /// Thrown when no ConnectionString could be found. A valid ConnectionString or Connection String Name must be supplied in
        /// the 'connectionStringOrName' parameter or by setting a default in either the
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionStringName' or
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionString' properties.
        /// </exception>
        /// <exception cref="DatabaseCommand.DbCommand">
        /// Thrown when no DbProviderFactory could be found. A DbProviderFactory invariant name must be supplied in the connection
        /// string settings 'providerName' attribute in the applications config file, in the 'dbProviderFactoryInvariantName'
        /// parameter, or by setting a default in the
        /// 'DatabaseCommand.ConfigurationSettings.Default.DbProviderFactoryInvariantName' property.
        /// </exception>
        /// <exception cref="DatabaseCommand">
        /// An unknown error occurred creating a connection as the call to DbProviderFactory.CreateConnection() returned null.
        /// </exception>
        public static DatabaseCommand GetDatabaseCommand( string connectionStringOrName = null, string dbProviderFactoryInvariantName = null )
        {
            DbConnection dbConnection = CreateDbConnection( connectionStringOrName, dbProviderFactoryInvariantName );

            var databaseCommand = new DatabaseCommand( dbConnection );

            return databaseCommand;
        }

        /// <summary>Gets a <see cref="DatabaseCommand" /> that interacts with a Microsoft SQL Server database.</summary>
        /// <param name="connectionStringOrName">Connection string or connection string name.</param>
        /// <param name="applicationName">Optional application name to inject into the ApplicationName property of the connection string so that the application name is accessible in SQL Server via the APP_NAME() function and during tracing.</param>
        /// <returns>A new <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ConnectionStringNotFoundException">
        /// Thrown when no ConnectionString could be found. A valid ConnectionString or Connection String Name must be supplied in
        /// the 'connectionStringOrName' parameter or by setting a default in either the
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionStringName' or
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionString' properties.
        /// </exception>
        /// <exception cref="DbProviderFactoryNotFoundException">
        /// Thrown when no DbProviderFactory could be found. A DbProviderFactory invariant name must be supplied in the connection
        /// string settings 'providerName' attribute in the applications config file, in the 'dbProviderFactoryInvariantName'
        /// parameter, or by setting a default in the
        /// 'DatabaseCommand.ConfigurationSettings.Default.DbProviderFactoryInvariantName' property.
        /// </exception>
        /// <exception cref="Exception">
        /// An unknown error occurred creating a connection as the call to DbProviderFactory.CreateConnection() returned null.
        /// </exception>
        public static DatabaseCommand GetDatabaseCommandForSqlServer( string connectionStringOrName = null, string applicationName = null )
        {
            var databaseCommand = GetDatabaseCommand( connectionStringOrName, "System.Data.SqlClient" );

            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder( databaseCommand.DbCommand.Connection.ConnectionString );

            if ( String.IsNullOrWhiteSpace( builder.ApplicationName ) )
            {
                if ( String.IsNullOrWhiteSpace( applicationName ) == false )
                {
                    builder.ApplicationName = applicationName;
                }
                else if ( String.IsNullOrWhiteSpace( ConfigurationSettings.Default.ApplicationName ) == false )
                {
                    builder.ApplicationName = ConfigurationSettings.Default.ApplicationName;
                }
            }

            databaseCommand.DbCommand.Connection.ConnectionString = builder.ToString();

            return databaseCommand;
        }

        /// <summary>Gets a <see cref="DatabaseCommand" /> that interacts with a SQLite database.</summary>
        /// <param name="connectionStringOrName">Connection string or connection string name.</param>
        /// <returns>A new <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ConnectionStringNotFoundException">
        /// Thrown when no ConnectionString could be found. A valid ConnectionString or Connection String Name must be supplied in
        /// the 'connectionStringOrName' parameter or by setting a default in either the
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionStringName' or
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionString' properties.
        /// </exception>
        /// <exception cref="DbProviderFactoryNotFoundException">
        /// Thrown when no DbProviderFactory could be found. A DbProviderFactory invariant name must be supplied in the connection
        /// string settings 'providerName' attribute in the applications config file, in the 'dbProviderFactoryInvariantName'
        /// parameter, or by setting a default in the
        /// 'DatabaseCommand.ConfigurationSettings.Default.DbProviderFactoryInvariantName' property.
        /// </exception>
        /// <exception cref="Exception">
        /// An unknown error occurred creating a connection as the call to DbProviderFactory.CreateConnection() returned null.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        public static DatabaseCommand GetDatabaseCommandForSQLite( string connectionStringOrName = null )
        {
            return GetDatabaseCommand( connectionStringOrName, "System.Data.SQLite" );
        }

        /// <summary>Gets a <see cref="DatabaseCommand" /> that interacts with a MySQL database.</summary>
        /// <param name="connectionStringOrName">Connection string or connection string name.</param>
        /// <returns>A new <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ConnectionStringNotFoundException">
        /// Thrown when no ConnectionString could be found. A valid ConnectionString or Connection String Name must be supplied in
        /// the 'connectionStringOrName' parameter or by setting a default in either the
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionStringName' or
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionString' properties.
        /// </exception>
        /// <exception cref="DbProviderFactoryNotFoundException">
        /// Thrown when no DbProviderFactory could be found. A DbProviderFactory invariant name must be supplied in the connection
        /// string settings 'providerName' attribute in the applications config file, in the 'dbProviderFactoryInvariantName'
        /// parameter, or by setting a default in the
        /// 'DatabaseCommand.ConfigurationSettings.Default.DbProviderFactoryInvariantName' property.
        /// </exception>
        /// <exception cref="Exception">
        /// An unknown error occurred creating a connection as the call to DbProviderFactory.CreateConnection() returned null.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        public static DatabaseCommand GetDatabaseCommandForMySql( string connectionStringOrName = null )
        {
            return GetDatabaseCommand( connectionStringOrName, "MySql.Data.MySqlClient" );
        }

        /// <summary>Gets a <see cref="DatabaseCommand" /> that interacts with a PostgreSQL database.</summary>
        /// <param name="connectionStringOrName">Connection string or connection string name.</param>
        /// <returns>A new <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ConnectionStringNotFoundException">
        /// Thrown when no ConnectionString could be found. A valid ConnectionString or Connection String Name must be supplied in
        /// the 'connectionStringOrName' parameter or by setting a default in either the
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionStringName' or
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionString' properties.
        /// </exception>
        /// <exception cref="DbProviderFactoryNotFoundException">
        /// Thrown when no DbProviderFactory could be found. A DbProviderFactory invariant name must be supplied in the connection
        /// string settings 'providerName' attribute in the applications config file, in the 'dbProviderFactoryInvariantName'
        /// parameter, or by setting a default in the
        /// 'DatabaseCommand.ConfigurationSettings.Default.DbProviderFactoryInvariantName' property.
        /// </exception>
        /// <exception cref="Exception">
        /// An unknown error occurred creating a connection as the call to DbProviderFactory.CreateConnection() returned null.
        /// </exception>
        public static DatabaseCommand GetDatabaseCommandForPostgreSQL(string connectionStringOrName = null)
        {
            return GetDatabaseCommand(connectionStringOrName, "Npgsql");
        }

        /// <summary>Attempts to create a <see cref="DbConnection" /> using several strategies.</summary>
        /// <remarks>
        /// This method attempts to use several strategies to locate a ConnectionString and DbProviderFactory in order to create a
        /// new <see cref="DbConnection" />. The recommended option is to supply to this method a ConnectionString Name where the
        /// connection string setting in the application's configuration file also contains a populated providerName attribute
        /// specifying the DbProviderFactory invariant name to be used to create a <see cref="DbConnection" />. Another recommend
        /// option is to specify default values in the <see cref="ConfigurationSettings.Default" /> class that allows this method
        /// to be called with no parameters which simplifies callers to this method.
        /// </remarks>
        /// <param name="connectionStringOrName">Optional connection string or connection string name.</param>
        /// <param name="dbProviderFactoryInvariantName">Optional DbProviderFactory invariant name.</param>
        /// <returns>A new <see cref="DbConnection" /> instance.</returns>
        /// <exception cref="ConnectionStringNotFoundException">
        /// Thrown when no ConnectionString could be found. A valid ConnectionString or Connection String Name must be supplied in
        /// the 'connectionStringOrName' parameter or by setting a default in either the
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionStringName' or
        /// 'DatabaseCommand.ConfigurationSettings.Default.ConnectionString' properties.
        /// </exception>
        /// <exception cref="DbProviderFactoryNotFoundException">
        /// Thrown when no DbProviderFactory could be found. A DbProviderFactory invariant name must be supplied in the connection
        /// string settings 'providerName' attribute in the applications config file, in the 'dbProviderFactoryInvariantName'
        /// parameter, or by setting a default in the
        /// 'DatabaseCommand.ConfigurationSettings.Default.DbProviderFactoryInvariantName' property.
        /// </exception>
        /// <exception cref="Exception">
        /// An unknown error occurred creating a connection as the call to DbProviderFactory.CreateConnection() returned null.
        /// </exception>
        public static DbConnection CreateDbConnection( string connectionStringOrName = null, string dbProviderFactoryInvariantName = null )
        {
            string connectionString = null;

            if ( String.IsNullOrWhiteSpace( connectionStringOrName ) == false )
            {
                // Try obtaining the connection string by name first as the connection string settings could contain a DbProviderFactory provider name
                ConnectionStringSettings connectionStringSettings = ConfigurationSettings.GetConnectionStringByNameAccessor( connectionStringOrName );

                if ( connectionStringSettings != null )
                {
                    connectionString = connectionStringSettings.ConnectionString;

                    if ( String.IsNullOrWhiteSpace( connectionStringSettings.ProviderName ) == false )
                    {
                        DbConnection connection = InternalCreateDbConnection( connectionString, connectionStringSettings.ProviderName );

                        return connection;
                    }
                }

                connectionString = connectionStringOrName;
            }

            // Try obtaining the connection string by name using the default setting
            if ( connectionString == null && String.IsNullOrWhiteSpace( ConfigurationSettings.Default.ConnectionStringName ) == false )
            {
                ConnectionStringSettings connectionStringSettings = ConfigurationSettings.GetConnectionStringByNameAccessor( ConfigurationSettings.Default.ConnectionStringName );

                if ( connectionStringSettings != null )
                {
                    connectionString = connectionStringSettings.ConnectionString;

                    if ( String.IsNullOrWhiteSpace( connectionStringSettings.ProviderName ) == false )
                    {
                        DbConnection connection = InternalCreateDbConnection( connectionString, connectionStringSettings.ProviderName );

                        return connection;
                    }
                }
            }

            // Try obtaining the connection string using the default setting
            if ( connectionString == null && String.IsNullOrWhiteSpace( ConfigurationSettings.Default.ConnectionString ) == false )
            {
                connectionString = ConfigurationSettings.Default.ConnectionString;
            }

            if ( connectionString == null )
            {
                throw new ConnectionStringNotFoundException( "No ConnectionString could be found. A valid ConnectionString or Connection String Name must be supplied in the 'connectionStringOrName' parameter or by setting a default in either the 'DatabaseCommand.ConfigurationSettings.Default.ConnectionStringName' or 'DatabaseCommand.ConfigurationSettings.Default.ConnectionString' properties." );
            }

            if ( String.IsNullOrWhiteSpace( dbProviderFactoryInvariantName ) == false )
            {
                DbConnection connection = InternalCreateDbConnection( connectionString, dbProviderFactoryInvariantName );

                return connection;
            }

            if ( String.IsNullOrWhiteSpace( ConfigurationSettings.Default.DbProviderFactoryInvariantName ) == false )
            {
                DbConnection connection = InternalCreateDbConnection( connectionString, ConfigurationSettings.Default.DbProviderFactoryInvariantName );

                return connection;
            }

            throw new DbProviderFactoryNotFoundException( "No DbProviderFactory could be found. A DbProviderFactory invariant name must be supplied in the connection string settings 'providerName' attribute in the applications config file, in the 'dbProviderFactoryInvariantName' parameter, or by setting a default in the 'DatabaseCommand.ConfigurationSettings.Default.DbProviderFactoryInvariantName' property." );
        }

        /// <summary>Creates a <see cref="DbConnection" />.</summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="dbProviderFactoryInvariantName">DbProviderFactory invariant name.</param>
        /// <returns>A new <see cref="DbConnection" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="connectionString" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="dbProviderFactoryInvariantName" /> parameter is
        /// null.
        /// </exception>
        /// <exception cref="Exception">
        /// An unknown error occurred creating a connection as the call to DbProviderFactory.CreateConnection() returned null.
        /// </exception>
        private static DbConnection InternalCreateDbConnection( string connectionString, string dbProviderFactoryInvariantName )
        {
            if ( String.IsNullOrWhiteSpace( connectionString ) )
            {
                throw new ArgumentNullException( "connectionString" );
            }

            if ( String.IsNullOrWhiteSpace( dbProviderFactoryInvariantName ) )
            {
                throw new ArgumentNullException( "dbProviderFactoryInvariantName" );
            }

            DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory( dbProviderFactoryInvariantName );

            DbConnection connection = dbProviderFactory.CreateConnection();

            if ( connection == null )
            {
                throw new Exception( "An unknown error occurred creating a connection as the call to DbProviderFactory.CreateConnection() returned null." );
            }

            connection.ConnectionString = connectionString;

            return connection;
        }

        /// <summary>Global <see cref="DatabaseCommand" /> configuration settings.</summary>
        public static class ConfigurationSettings
        {
            /// <summary>Gets a connection string by name.</summary>
            /// <remarks>
            /// This is implemented as a Func in order to facilitate library consumers to specify their own implementation if need be.
            /// </remarks>
            public static Func<string, ConnectionStringSettings> GetConnectionStringByNameAccessor;

            static ConfigurationSettings()
            {
                InitializeConfigurationSettings();
            }

            /// <summary>Initializes the <see cref="Sequelocity.ConfigurationSettings" />.</summary>
            public static void InitializeConfigurationSettings()
            {
                GetConnectionStringByNameAccessor = connectionStringName => ConfigurationManager.ConnectionStrings[ connectionStringName ];
            }

            /// <summary>
            /// <see cref="DatabaseCommand" /> default settings.
            /// </summary>
            public static class Default
            {
                /// <summary>Default DbProviderFactory invariant name.</summary>
                public static string DbProviderFactoryInvariantName = "System.Data.SqlClient";

                /// <summary>Default connection string name.</summary>
                public static string ConnectionStringName = null;

                /// <summary>Default connection string.</summary>
                public static string ConnectionString = null;

                /// <summary>The current application name.</summary>
                /// <remarks>Default implementation is Environment.MachineName-AppDomain.CurrentDomain.FriendlyName.</remarks>
                public static string ApplicationName = String.Format( "{0}-{1}", Environment.MachineName, AppDomain.CurrentDomain.FriendlyName );
            }

            /// <summary>
            /// <see cref="DatabaseCommand" /> event handlers.
            /// </summary>
            public static class EventHandlers
            {
                /// <summary>Event handler called after the <see cref="DatabaseCommand" /> has been executed.</summary>
                /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
                public delegate void DatabaseCommandPostExecuteEventHandler( DatabaseCommand databaseCommand );

                /// <summary>Event handler called before the <see cref="DatabaseCommand" /> is executed.</summary>
                /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
                public delegate void DatabaseCommandPreExecuteEventHandler( DatabaseCommand databaseCommand );

                /// <summary>Event handler called when an unhandled exception occurs.</summary>
                /// <param name="exception">Unhandled exception.</param>
                /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
                public delegate void DatabaseCommandUnhandledExceptionEventHandler( Exception exception, DatabaseCommand databaseCommand );
                
                /// <summary>Event triggered when an unhandled exception occurs.</summary>
                public static readonly List<DatabaseCommandUnhandledExceptionEventHandler> DatabaseCommandUnhandledExceptionEventHandlers = new List<DatabaseCommandUnhandledExceptionEventHandler>();

                /// <summary>Event triggered before the <see cref="DatabaseCommand" /> is executed.</summary>
                public static readonly List<DatabaseCommandPreExecuteEventHandler> DatabaseCommandPreExecuteEventHandlers = new List<DatabaseCommandPreExecuteEventHandler>();

                /// <summary>Event triggered after the <see cref="DatabaseCommand" /> has been executed.</summary>
                public static readonly List<DatabaseCommandPostExecuteEventHandler> DatabaseCommandPostExecuteEventHandlers = new List<DatabaseCommandPostExecuteEventHandler>();

                /// <summary>Invokes the <see cref="DatabaseCommand" /> unhandled exception event handlers.</summary>
                /// <param name="exception">Unhandled exception.</param>
                /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
                public static void InvokeDatabaseCommandUnhandledExceptionEventHandlers( Exception exception, DatabaseCommand databaseCommand )
                {
                    foreach ( DatabaseCommandUnhandledExceptionEventHandler databaseCommandUnhandledExceptionEventHandler in DatabaseCommandUnhandledExceptionEventHandlers )
                    {
                        databaseCommandUnhandledExceptionEventHandler.Invoke( exception, databaseCommand );
                    }
                }

                /// <summary>Invokes the <see cref="DatabaseCommand" /> pre-execute event handlers.</summary>
                /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
                public static void InvokeDatabaseCommandPreExecuteEventHandlers( DatabaseCommand databaseCommand )
                {
                    foreach ( DatabaseCommandPreExecuteEventHandler databaseCommandPreExecuteEventHandler in DatabaseCommandPreExecuteEventHandlers )
                    {
                        databaseCommandPreExecuteEventHandler.Invoke( databaseCommand );
                    }
                }

                /// <summary>Invokes the <see cref="DatabaseCommand" /> post-execute event handlers.</summary>
                /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
                public static void InvokeDatabaseCommandPostExecuteEventHandlers( DatabaseCommand databaseCommand )
                {
                    foreach ( DatabaseCommandPostExecuteEventHandler databaseCommandPostExecuteEventHandler in DatabaseCommandPostExecuteEventHandlers )
                    {
                        databaseCommandPostExecuteEventHandler.Invoke( databaseCommand );
                    }
                }
            }
        }

        /// <summary>Thrown when a ConnectionString could not be found.</summary>
        [Serializable]
        public class ConnectionStringNotFoundException : Exception
        {
            /// <summary>
            /// Instantiates a new <see cref="Sequelocity.ConnectionStringNotFoundException" /> with a specified error message.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            public ConnectionStringNotFoundException( string message )
                : base( message )
            {
            }
        }

        /// <summary>Thrown when a DbProviderFactory could not be found.</summary>
        [Serializable]
        public class DbProviderFactoryNotFoundException : Exception
        {
            /// <summary>
            /// Instantiates a new <see cref="Sequelocity.DbProviderFactoryNotFoundException" /> with a specified error message.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            public DbProviderFactoryNotFoundException( string message )
                : base( message )
            {
            }
        }
    }

    /// <summary>
    /// The <see cref="DatabaseCommand" /> is a lightweight abstraction and wrapper around the <see cref="DbCommand" /> in
    /// order to provide a streamlined fluent-style data access interface.
    /// </summary>
    public class DatabaseCommand : IDisposable
    {
        /// <summary>The underlying <see cref="DbCommand" />.</summary>
        public DbCommand DbCommand;

        /// <summary>Flag to determine if Dispose has been called.</summary>
        private bool _disposed;

        /// <summary>Instantiates a new <see cref="DatabaseCommand" /> by supplying a <see cref="DbCommand" />.</summary>
        /// <param name="dbCommand">DbCommand.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbCommand" /> parameter is null.</exception>
        public DatabaseCommand( DbCommand dbCommand )
        {
            if ( dbCommand == null )
            {
                throw new ArgumentNullException( "dbCommand" );
            }

            DbCommand = dbCommand;
        }

        /// <summary>Instantiates a new <see cref="DatabaseCommand" /> by supplying a <see cref="DbConnection" />.</summary>
        /// <param name="dbConnection">DbConnection.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbConnection" /> parameter is null.</exception>
        public DatabaseCommand( DbConnection dbConnection )
        {
            if ( dbConnection == null )
            {
                throw new ArgumentNullException( "dbConnection" );
            }

            DbCommand = dbConnection.CreateCommand();
        }

        #region IDisposable Members

        /// <summary>Disposes of the underlying <see cref="DbCommand" /> and it's <see cref="DbConnection" />.</summary>
        public void Dispose()
        {
            Dispose( true );

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize( this );
        }

        /// <summary>Disposes of the underlying <see cref="DbCommand" />.</summary>
        /// <param name="disposing">Indicates if being called from the Dispose method.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( _disposed == false )
            {
                if ( disposing )
                {
                    if ( DbCommand != null )
                    {
                        try
                        {
                            DbCommand.CloseAndDispose();
                        }
                            // ReSharper disable once EmptyGeneralCatchClause - CA1065: Do not raise exceptions in unexpected locations: http://msdn.microsoft.com/en-us/library/bb386039.aspx
                        catch
                        {
                            // Don't throw an exception while disposing
                        }
                        finally
                        {
                            DbCommand = null;
                        }
                    }
                }

                _disposed = true;
            }
        }

        #endregion IDisposable Members
    }

    /// <summary>
    /// <see cref="DatabaseCommand" /> extensions.
    /// </summary>
    public static class DatabaseCommandExtensions
    {
        /// <summary>Sets the text command to run against the data source.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="commandText">The text command to run against the data source.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand SetCommandText( this DatabaseCommand databaseCommand, string commandText )
        {
            databaseCommand.DbCommand.SetCommandText( commandText );

            return databaseCommand;
        }

        /// <summary>Appends to the text command to run against the data source.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="commandText">Text command to append to the text command to run against the data source.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AppendCommandText( this DatabaseCommand databaseCommand, string commandText )
        {
            databaseCommand.DbCommand.AppendCommandText( commandText );

            return databaseCommand;
        }

        /// <summary>Adds a <see cref="DbParameter" /> to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dbParameter"><see cref="DbParameter" /> to add.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameter( this DatabaseCommand databaseCommand, DbParameter dbParameter )
        {
            databaseCommand.DbCommand.AddParameter( dbParameter );

            return databaseCommand;
        }

        /// <summary>Adds a parameter to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameter( this DatabaseCommand databaseCommand, string parameterName, object parameterValue )
        {
            databaseCommand.DbCommand.AddParameter( parameterName, parameterValue );

            return databaseCommand;
        }

        /// <summary>Adds a parameter to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameter( this DatabaseCommand databaseCommand, string parameterName, object parameterValue, DbType dbType )
        {
            databaseCommand.DbCommand.AddParameter( parameterName, parameterValue, dbType );

            return databaseCommand;
        }

        /// <summary>Adds a parameter whose default value is <see cref="DBNull"/> when unassigned, to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddNullableParameter( this DatabaseCommand databaseCommand, string parameterName, object parameterValue, DbType dbType )
        {
            databaseCommand.DbCommand.AddNullableParameter( parameterName, parameterValue, dbType );

            return databaseCommand;
        }

        /// <summary>Adds a list of <see cref="DbParameter" />s to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dbParameters">List of database parameters.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameters( this DatabaseCommand databaseCommand, IEnumerable<DbParameter> dbParameters )
        {
            databaseCommand.DbCommand.AddParameters( dbParameters );

            return databaseCommand;
        }

        /// <summary>Adds a parameter array of <see cref="DbParameter" />s to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dbParameters">Parameter array of database parameters.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameters( this DatabaseCommand databaseCommand, params DbParameter[] dbParameters )
        {
            databaseCommand.DbCommand.AddParameters( dbParameters );

            return databaseCommand;
        }

        /// <summary>Adds a dictionary of parameter names and values to the <see cref="DatabaseCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterNameAndValueDictionary">Dictionary of parameter names and values.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand AddParameters( this DatabaseCommand databaseCommand, IDictionary<string, object> parameterNameAndValueDictionary )
        {
            databaseCommand.DbCommand.AddParameters( parameterNameAndValueDictionary );

            return databaseCommand;
        }

        /// <summary>
        /// Adds the list of parameter values to the <see cref="DatabaseCommand" /> by replacing the given parameterName in the
        /// CommandText with a comma delimited list of generated parameter names such as "parameterName0, parameterName1,
        /// parameterName2", etc.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValues">Parameter values.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterName" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterValues" /> parameter is null.</exception>
        /// <exception cref="Exception">Thrown when the <paramref name="parameterValues" /> list is empty.</exception>
        /// <exception cref="Exception">Thrown when the CommandText has not been set prior to calling this method.</exception>
        /// <exception cref="Exception">Thrown when the CommandText does not contain the <paramref name="parameterName" />.</exception>
        public static DatabaseCommand AddParameters<T>( this DatabaseCommand databaseCommand, string parameterName, List<T> parameterValues )
        {
            databaseCommand.DbCommand.AddParameters( parameterName, parameterValues );

            return databaseCommand;
        }

        /// <summary>
        /// Adds the list of parameter values of the specified <see cref="DbType" /> to the <see cref="DatabaseCommand" /> by
        /// replacing the given parameterName in the CommandText with a comma delimited list of generated parameter names such as
        /// "parameterName0, parameterName1, parameterName2", etc.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValues">Parameter values.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterName" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterValues" /> parameter is null.</exception>
        /// <exception cref="Exception">Thrown when the <paramref name="parameterValues" /> list is empty.</exception>
        /// <exception cref="Exception">Thrown when the CommandText has not been set prior to calling this method.</exception>
        /// <exception cref="Exception">Thrown when the CommandText does not contain the <paramref name="parameterName" />.</exception>
        public static DatabaseCommand AddParameters<T>( this DatabaseCommand databaseCommand, string parameterName, List<T> parameterValues, DbType dbType )
        {
            databaseCommand.DbCommand.AddParameters( parameterName, parameterValues, dbType );

            return databaseCommand;
        }

        /// <summary>Creates a <see cref="DbParameter" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter( this DatabaseCommand databaseCommand, string parameterName, object parameterValue )
        {
            DbParameter parameter = databaseCommand.DbCommand.CreateParameter( parameterName, parameterValue );

            return parameter;
        }

        /// <summary>Creates a <see cref="DbParameter" /> with a given <see cref="DbType" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter( this DatabaseCommand databaseCommand, string parameterName, object parameterValue, DbType dbType )
        {
            DbParameter parameter = databaseCommand.DbCommand.CreateParameter( parameterName, parameterValue, dbType );

            return parameter;
        }

        /// <summary>
        /// Creates a <see cref="DbParameter" /> with a given <see cref="DbType" /> and <see cref="ParameterDirection" />.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <param name="parameterDirection">Parameter direction.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter( this DatabaseCommand databaseCommand, string parameterName, object parameterValue, DbType dbType, ParameterDirection parameterDirection )
        {
            DbParameter parameter = databaseCommand.DbCommand.CreateParameter( parameterName, parameterValue, dbType, parameterDirection );

            return parameter;
        }

        /// <summary>Sets the CommandType.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="commandType">CommandType which specifies how a command string is interpreted.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand SetCommandType( this DatabaseCommand databaseCommand, CommandType commandType )
        {
            databaseCommand.DbCommand.SetCommandType( commandType );

            return databaseCommand;
        }

        /// <summary>
        /// Sets the time in seconds to wait for the command to execute before throwing an exception. The default is 30 seconds.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="commandTimeoutSeconds">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand SetCommandTimeout( this DatabaseCommand databaseCommand, int commandTimeoutSeconds )
        {
            databaseCommand.DbCommand.SetCommandTimeout( commandTimeoutSeconds );

            return databaseCommand;
        }

        /// <summary>Sets the transaction associated with the command.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dbTransaction">The transaction to associate with the command.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand SetTransaction( this DatabaseCommand databaseCommand, DbTransaction dbTransaction )
        {
            databaseCommand.DbCommand.SetTransaction( dbTransaction );

            return databaseCommand;
        }
        
        /// <summary>
        /// Starts a database transaction and associates it with the <see cref="DatabaseCommand"/> instance.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <returns>An object representing the new transaction.</returns>
        public static DbTransaction BeginTransaction( this DatabaseCommand databaseCommand )
        {
            DbTransaction transaction = databaseCommand.DbCommand.BeginTransaction();
            
            return transaction;
        }

        /// <summary>
        /// Starts a database transaction with the specified isolation level and associates it with the <see cref="DatabaseCommand"/> instance.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
        /// <returns>An object representing the new transaction.</returns>
        public static DbTransaction BeginTransaction( this DatabaseCommand databaseCommand, IsolationLevel isolationLevel )
        {
            DbTransaction transaction = databaseCommand.DbCommand.BeginTransaction( isolationLevel );

            return transaction;
        }

        /// <summary>
        /// Tests the connection to a database and returns true if a connection can be successfully opened and closed.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <returns>Returns true if a connection can be successfully opened and closed. </returns>
        public static bool TestConnection( this DatabaseCommand databaseCommand )
        {
            try
            {
                databaseCommand.DbCommand.Connection.Open();

                databaseCommand.DbCommand.Connection.Close();

                return true;
            }
            catch ( Exception )
            {
                return false;
            }
        }

        /// <summary>Returns the underlying <see cref="DbCommand" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <returns><see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="databaseCommand" /> parameter is null.</exception>
        public static DbCommand ToDbCommand( this DatabaseCommand databaseCommand )
        {
            if ( databaseCommand == null )
            {
                throw new ArgumentNullException( "databaseCommand" );
            }

            return databaseCommand.DbCommand;
        }

        /// <summary>Returns a new instance of a <see cref="DatabaseCommand" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <returns><see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbCommand" /> parameter is null.</exception>
        public static DatabaseCommand ToDatabaseCommand( this DbCommand dbCommand )
        {
            if ( dbCommand == null )
            {
                throw new ArgumentNullException( "dbCommand" );
            }

            return new DatabaseCommand( dbCommand );
        }

        /// <summary>
        /// Generates a parameterized MySQL INSERT statement from the given object and adds it to the
        /// <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using MySQL's SELECT LAST_INSERT_ID() function.
        /// </para>
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="obj">Object to generate the SQL INSERT statement from.</param>
        /// <param name="tableName">Optional table name to insert into. If none is supplied, it will use the type name.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DatabaseCommand GenerateInsertForMySql( this DatabaseCommand databaseCommand, object obj, string tableName = null )
        {
            databaseCommand.DbCommand.GenerateInsertForMySql( obj, tableName );

            return databaseCommand;
        }

        /// <summary>
        /// Generates a list of concatenated parameterized MySQL INSERT statements from the given list of objects and adds it to
        /// the <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using MySQL's SELECT LAST_INSERT_ID() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="listOfObjects">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DatabaseCommand GenerateInsertsForMySql<T>( this DatabaseCommand databaseCommand, List<T> listOfObjects, string tableName = null )
        {
            databaseCommand.DbCommand.GenerateInsertsForMySql( listOfObjects, tableName );

            return databaseCommand;
        }

        /// <summary>
        /// Generates a parameterized PostgreSQL INSERT statement from the given object and adds it to the
        /// <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using PostgreSQL's LastVal() function.
        /// </para>
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="obj">Object to generate the SQL INSERT statement from.</param>
        /// <param name="tableName">Optional table name to insert into. If none is supplied, it will use the type name.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DatabaseCommand GenerateInsertForPostgreSQL(this DatabaseCommand databaseCommand, object obj, string tableName = null)
        {
            databaseCommand.DbCommand.GenerateInsertForPostgreSQL(obj, tableName);

            return databaseCommand;
        }

        /// <summary>
        /// Generates a list of concatenated parameterized PostgreSQL INSERT statements from the given list of objects and adds it to
        /// the <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using PostgreSQL's LastVal() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="listOfObjects">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DatabaseCommand GenerateInsertsForPostgreSQL<T>(this DatabaseCommand databaseCommand, List<T> listOfObjects, string tableName = null)
        {
            databaseCommand.DbCommand.GenerateInsertsForPostgreSQL(listOfObjects, tableName);

            return databaseCommand;
        }

        /// <summary>
        /// Generates a parameterized SQLite INSERT statement from the given object and adds it to the
        /// <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQLite's SELECT last_insert_rowid() function.
        /// </para>
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="obj">Object to generate the SQL INSERT statement from.</param>
        /// <param name="tableName">Optional table name to insert into. If none is supplied, it will use the type name.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        public static DatabaseCommand GenerateInsertForSQLite( this DatabaseCommand databaseCommand, object obj, string tableName = null )
        {
            databaseCommand.DbCommand.GenerateInsertForSQLite( obj, tableName );

            return databaseCommand;
        }

        /// <summary>
        /// Generates a list of concatenated parameterized SQLite INSERT statements from the given list of objects and adds it to
        /// the <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQLite's SELECT last_insert_rowid() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="listOfObjects">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        public static DatabaseCommand GenerateInsertsForSQLite<T>( this DatabaseCommand databaseCommand, List<T> listOfObjects, string tableName = null )
        {
            databaseCommand.DbCommand.GenerateInsertsForSQLite( listOfObjects, tableName );

            return databaseCommand;
        }

        /// <summary>
        /// Generates a parameterized SQL Server INSERT statement from the given object and adds it to the
        /// <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQL Server's SELECT SCOPE_IDENTITY() function.
        /// </para>
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="obj">Object to generate the SQL INSERT statement from.</param>
        /// <param name="tableName">Optional table name to insert into. If none is supplied, it will use the type name.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DatabaseCommand GenerateInsertForSqlServer( this DatabaseCommand databaseCommand, object obj, string tableName = null )
        {
            databaseCommand.DbCommand.GenerateInsertForSqlServer( obj, tableName );

            return databaseCommand;
        }

        /// <summary>
        /// Generates a list of concatenated parameterized SQL Server INSERT statements from the given list of objects and adds it
        /// to the <see cref="DatabaseCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQL Server's SELECT SCOPE_IDENTITY() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="listOfObjects">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DatabaseCommand GenerateInsertsForSqlServer<T>( this DatabaseCommand databaseCommand, List<T> listOfObjects, string tableName = null )
        {
            databaseCommand.DbCommand.GenerateInsertsForSqlServer( listOfObjects, tableName );

            return databaseCommand;
        }

        #region Execute Functions

        /// <summary>Executes a statement against the database and returns the number of rows affected.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>The number of rows affected.</returns>
        /// <exception cref="Exception">Unexpected exception.</exception>
        public static int ExecuteNonQuery( this DatabaseCommand databaseCommand, bool keepConnectionOpen = false )
        {
            int numberOfRowsAffected;

            try
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers( databaseCommand );

                databaseCommand.DbCommand.OpenConnection();

                numberOfRowsAffected = databaseCommand.DbCommand.ExecuteNonQuery();

                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers( databaseCommand );
            }
            catch ( Exception exception )
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers( exception, databaseCommand );

                throw;
            }
            finally
            {
                if ( keepConnectionOpen == false )
                {
                    databaseCommand.DbCommand.CloseAndDispose();

                    databaseCommand.DbCommand = null;
                }
            }

            return numberOfRowsAffected;
        }

        /// <summary>Executes a statement against the database asynchronously and returns the number of rows affected.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>A <see cref="Task{TResult}"/> resulting in the number of rows affected.</returns>
        /// <exception cref="Exception">Unexpected exception.</exception>
        public static async Task<int> ExecuteNonQueryAsync(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            int numberOfRowsAffected;

            try
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers(databaseCommand);

                await databaseCommand.DbCommand.OpenConnectionAsync();

                numberOfRowsAffected = await databaseCommand.DbCommand.ExecuteNonQueryAsync();

                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers(databaseCommand);
            }
            catch (Exception exception)
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers(exception, databaseCommand);

                throw;
            }
            finally
            {
                if (keepConnectionOpen == false)
                {
                    databaseCommand.DbCommand.CloseAndDispose();

                    databaseCommand.DbCommand = null;
                }
            }

            return numberOfRowsAffected;
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set returned by the query. All other
        /// columns and rows are ignored.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        public static object ExecuteScalar( this DatabaseCommand databaseCommand, bool keepConnectionOpen = false )
        {
            object returnValue;

            try
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers( databaseCommand );

                databaseCommand.DbCommand.OpenConnection();

                returnValue = databaseCommand.DbCommand.ExecuteScalar();

                if ( returnValue == DBNull.Value )
                    returnValue = null;

                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers( databaseCommand );
            }
            catch ( Exception exception )
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers( exception, databaseCommand );

                throw;
            }
            finally
            {
                if ( keepConnectionOpen == false )
                {
                    databaseCommand.DbCommand.CloseAndDispose();

                    databaseCommand.DbCommand = null;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Executes the query asynchronously and returns the first column of the first row in the result set returned by the query.
        /// All other columns and rows are ignored.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>A <see cref="Task{TResult}"/> resulting in the first column of the first row in the result set.</returns>
        public static async Task<object> ExecuteScalarAsync(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            object returnValue;

            try
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers(databaseCommand);

                await databaseCommand.DbCommand.OpenConnectionAsync();

                returnValue = await databaseCommand.DbCommand.ExecuteScalarAsync();

                if (returnValue == DBNull.Value)
                    returnValue = null;

                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers(databaseCommand);
            }
            catch (Exception exception)
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers(exception, databaseCommand);

                throw;
            }
            finally
            {
                if (keepConnectionOpen == false)
                {
                    databaseCommand.DbCommand.CloseAndDispose();

                    databaseCommand.DbCommand = null;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set returned by the query. All other
        /// columns and rows are ignored.
        /// </summary>
        /// <typeparam name="T">Type to convert the result to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>
        /// The first column of the first row in the result set converted to a type of <typeparamref name="T" />.
        /// </returns>
        /// <exception cref="TypeConverter.TypeConversionException">
        /// Thrown when an error occurs attempting to convert a value to an
        /// enum.
        /// </exception>
        /// <exception cref="TypeConverter.TypeConversionException">
        /// Thrown when an error occurs attempting to convert a value to a
        /// type.
        /// </exception>
        public static T ExecuteScalar<T>( this DatabaseCommand databaseCommand, bool keepConnectionOpen = false )
        {
            object returnValue = databaseCommand.ExecuteScalar( keepConnectionOpen );

            return returnValue.ConvertTo<T>();
        }

        /// <summary>
        /// Executes the query asynchronously and returns the first column of the first row in the result set returned by the query.
        /// All other columns and rows are ignored.
        /// </summary>
        /// <typeparam name="T">Type to convert the result to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> resulting in the first column of the first row in the result set converted to a type
        /// of <typeparamref name="T" />.
        /// </returns>
        /// <exception cref="TypeConverter.TypeConversionException">
        /// Thrown when an error occurs attempting to convert a value to an
        /// enum.
        /// </exception>
        /// <exception cref="TypeConverter.TypeConversionException">
        /// Thrown when an error occurs attempting to convert a value to a
        /// type.
        /// </exception>
        public static async Task<T> ExecuteScalarAsync<T>(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            object returnValue = await databaseCommand.ExecuteScalarAsync(keepConnectionOpen);

            return returnValue.ConvertTo<T>();
        }

        /// <summary>
        /// Executes a statement against the database and calls the <paramref name="dataRecordCallback" /> action for each record
        /// returned.
        /// </summary>
        /// <remarks>
        /// For safety the DbDataReader is returned as an IDataRecord to the callback so that callers cannot modify the current row
        /// being read.
        /// </remarks>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dataRecordCallback">Action called for each record returned.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        public static void ExecuteReader( this DatabaseCommand databaseCommand, Action<IDataRecord> dataRecordCallback, bool keepConnectionOpen = false )
        {
            try
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers( databaseCommand );

                databaseCommand.DbCommand.OpenConnection();

                using ( DbDataReader dbDataReader = databaseCommand.DbCommand.ExecuteReader() )
                {
                    while ( dbDataReader.HasRows )
                    {
                        while ( dbDataReader.Read() )
                        {
                            dataRecordCallback.Invoke( dbDataReader );
                        }

                        dbDataReader.NextResult();
                    }
                }

                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers( databaseCommand );
            }
            catch ( Exception exception )
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers( exception, databaseCommand );

                throw;
            }
            finally
            {
                if ( keepConnectionOpen == false )
                {
                    databaseCommand.DbCommand.CloseAndDispose();

                    databaseCommand.DbCommand = null;
                }
            }
        }

        /// <summary>
        /// Executes a statement against the database and calls the <paramref name="selector" /> function for each record returned.
        /// </summary>
        /// <remarks>
        /// For safety the DbDataReader is returned as an IDataRecord to the callback so that callers cannot modify the current row
        /// being read.
        /// </remarks>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="selector">Function called for each record returned.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        public static IEnumerable<T> ExecuteReader<T>( this DatabaseCommand databaseCommand, Func<IDataRecord, T> selector, bool keepConnectionOpen = false )
        {
            DbDataReader dbDataReader = null;

            try
            {
                try
                {
                    Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers( databaseCommand );

                    databaseCommand.DbCommand.OpenConnection();
                    dbDataReader = databaseCommand.DbCommand.ExecuteReader();
                }
                catch ( Exception exception )
                {
                    Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers( exception, databaseCommand );
                    throw;
                }

                var readerHasRows = false;
                do
                {
                    try
                    {
                        readerHasRows = dbDataReader.HasRows;
                        if ( !readerHasRows )
                        {
                            break;
                        }
                    }
                    catch ( Exception exception )
                    {
                        Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers( exception, databaseCommand );
                        throw;
                    }

                    var hasNextRow = false;
                    do
                    {
                        T projection = default( T );
                        try
                        {
                            hasNextRow = dbDataReader.Read();

                            if (hasNextRow)
                            {
                                projection = selector.Invoke( dbDataReader );
                            }
                        }
                        catch ( Exception exception )
                        {
                            Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers( exception, databaseCommand );
                            throw;
                        }

                        if ( hasNextRow )
                        {
                            yield return projection;
                        }
                    }
                    while ( hasNextRow );

                    try
                    {
                        dbDataReader.NextResult();
                    }
                    catch ( Exception exception )
                    {
                        Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers( exception, databaseCommand );
                        throw;
                    }
                }
                while ( readerHasRows );

                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers( databaseCommand );
            }
            finally
            {
                if ( dbDataReader != null )
                {
                    dbDataReader.Dispose();
                }

                if ( keepConnectionOpen == false )
                {
                    databaseCommand.DbCommand.CloseAndDispose();
                    databaseCommand.DbCommand = null;
                }
            }
        }

        /// <summary>
        /// Executes a statement against the database asynchronously and calls the <paramref name="dataRecordCallback" /> action for each record
        /// returned.
        /// </summary>
        /// <remarks>
        /// For safety the DbDataReader is returned as an IDataRecord to the callback so that callers cannot modify the current row
        /// being read.
        /// </remarks>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="dataRecordCallback">Action called for each record returned.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        public static async Task ExecuteReaderAsync(this DatabaseCommand databaseCommand, Action<IDataRecord> dataRecordCallback, bool keepConnectionOpen = false)
        {
            try
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers(databaseCommand);

                await databaseCommand.DbCommand.OpenConnectionAsync();

                using (DbDataReader dbDataReader = await databaseCommand.DbCommand.ExecuteReaderAsync())
                {
                    while (dbDataReader.HasRows)
                    {
                        while (await dbDataReader.ReadAsync())
                        {
                            dataRecordCallback.Invoke(dbDataReader);
                        }

                        await dbDataReader.NextResultAsync();
                    }
                }

                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers(databaseCommand);
            }
            catch (Exception exception)
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers(exception, databaseCommand);

                throw;
            }
            finally
            {
                if (keepConnectionOpen == false)
                {
                    databaseCommand.DbCommand.CloseAndDispose();

                    databaseCommand.DbCommand = null;
                }
            }
        }

        /// <summary>
        /// Executes a statement against a database and maps the results to a list of type <typeparamref name="T" /> using a given
        /// mapper function supplied to the <paramref name="mapper" /> parameter.
        /// </summary>
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="mapper">
        /// A method that takes an <see cref="IDataRecord" /> as an argument and returns an instance of type
        /// <typeparamref name="T" />.
        /// </param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Results mapped to a list of type <typeparamref name="T" />.</returns>
        public static List<T> ExecuteToMap<T>( this DatabaseCommand databaseCommand, Func<IDataRecord, T> mapper, bool keepConnectionOpen = false )
        {
            var list = new List<T>();

            databaseCommand.ExecuteReader( reader =>
            {
                T mappedObject = mapper.Invoke( reader );

                list.Add( mappedObject );
            }, keepConnectionOpen );

            return list;
        }

        /// <summary>
        /// Executes a statement against a database asynchronously and maps the results to a list of type <typeparamref name="T" />
        /// using a given mapper function supplied to the <paramref name="mapper" /> parameter.
        /// </summary>
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="mapper">
        /// A method that takes an <see cref="IDataRecord" /> as an argument and returns an instance of type
        /// <typeparamref name="T" />.
        /// </param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>A <see cref="Task{TResult}"/> resulting in the results mapped to a list of type <typeparamref name="T" />.</returns>
        public static async Task<List<T>> ExecuteToMapAsync<T>(this DatabaseCommand databaseCommand, Func<IDataRecord, T> mapper, bool keepConnectionOpen = false)
        {
            var list = new List<T>();

            await databaseCommand.ExecuteReaderAsync(reader =>
            {
                T mappedObject = mapper.Invoke(reader);

                list.Add(mappedObject);
            }, keepConnectionOpen);

            return list;
        }

        /// <summary>
        /// Executes a statement against a database and maps matching column names to a list of type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Results mapped to a list of type <typeparamref name="T" />.</returns>
        public static List<T> ExecuteToList<T>( this DatabaseCommand databaseCommand, bool keepConnectionOpen = false )
        {
            return databaseCommand.ExecuteToMap( DataRecordMapper.Map<T>, keepConnectionOpen );
        }

        /// <summary>
        /// Executes a statement against a database asynchronously and maps matching column names to a list of type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>A <see cref="Task{TResult}"/> resulting in the results mapped to a list of type <typeparamref name="T" />.</returns>
        public static Task<List<T>> ExecuteToListAsync<T>(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            return databaseCommand.ExecuteToMapAsync(DataRecordMapper.Map<T>, keepConnectionOpen);
        }

        /// <summary>
        /// Executes a statement against a database and maps matching column names to a type of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Results mapped to a type of <typeparamref name="T" />.</returns>
        public static T ExecuteToObject<T>( this DatabaseCommand databaseCommand, bool keepConnectionOpen = false ) where T : new()
        {
            return databaseCommand.ExecuteToList<T>( keepConnectionOpen ).FirstOrDefault();
        }

        /// <summary>
        /// Executes a statement against a database asynchronously and maps matching column names to a type of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>A <see cref="Task{TResult}"/> resulting in the results mapped to a type of <typeparamref name="T" />.</returns>
        public static async Task<T> ExecuteToObjectAsync<T>(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false) where T : new()
        {
            return (await databaseCommand.ExecuteToListAsync<T>(keepConnectionOpen)).FirstOrDefault();
        }

        /// <summary>Executes a statement against a database and maps the results to a list of type dynamic.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Results mapped to a list of type dynamic.</returns>
        public static List<dynamic> ExecuteToDynamicList( this DatabaseCommand databaseCommand, bool keepConnectionOpen = false )
        {
            return databaseCommand.ExecuteToMap( DataRecordMapper.MapDynamic, keepConnectionOpen );
        }

        /// <summary>Executes a statement against a database asynchronously and maps the results to a list of type dynamic.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>A <see cref="Task{TResult}"/> resulting in the results mapped to a list of type dynamic.</returns>
        public static Task<List<dynamic>> ExecuteToDynamicListAsync(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            return databaseCommand.ExecuteToMapAsync(DataRecordMapper.MapDynamic, keepConnectionOpen);
        }

        /// <summary>Executes a statement against a database and maps the result to a dynamic object.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Result mapped to a dynamic object.</returns>
        public static dynamic ExecuteToDynamicObject( this DatabaseCommand databaseCommand, bool keepConnectionOpen = false )
        {
            return databaseCommand.ExecuteToDynamicList( keepConnectionOpen ).FirstOrDefault();
        }

        /// <summary>Executes a statement against a database asynchronously and maps the result to a dynamic object.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>Result mapped to a dynamic object.</returns>
        public static async Task<dynamic> ExecuteToDynamicObjectAsync(this DatabaseCommand databaseCommand, bool keepConnectionOpen = false)
        {
            return (await databaseCommand.ExecuteToDynamicListAsync(keepConnectionOpen)).FirstOrDefault();
        }

        /// <summary>Executes a statement against a database and populates the results into a <see cref="DataSet" />.</summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>DataSet representing an in-memory cache of the result set.</returns>
        /// <exception cref="Exception">An unexpected null was returned from a call to DbProviderFactory.CreateDataAdapter().</exception>
        public static DataSet ExecuteToDataSet( this DatabaseCommand databaseCommand, bool keepConnectionOpen = false )
        {
            var dataSet = new DataSet();

            try
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPreExecuteEventHandlers( databaseCommand );

                databaseCommand.DbCommand.OpenConnection();



                DbProviderFactory dbProviderFactory = databaseCommand.DbCommand.Connection.GetDbProviderFactory();
                
                DbDataAdapter dataAdapter = dbProviderFactory.CreateDataAdapter();

                if ( dataAdapter == null )
                    throw new Exception( "An unexpected null was returned from a call to DbProviderFactory.CreateDataAdapter()." );

                dataAdapter.SelectCommand = databaseCommand.DbCommand;

                dataAdapter.Fill( dataSet );

                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandPostExecuteEventHandlers( databaseCommand );
            }
            catch ( Exception exception )
            {
                Sequelocity.ConfigurationSettings.EventHandlers.InvokeDatabaseCommandUnhandledExceptionEventHandlers( exception, databaseCommand );

                throw;
            }
            finally
            {
                if ( keepConnectionOpen == false )
                {
                    databaseCommand.DbCommand.CloseAndDispose();

                    databaseCommand.DbCommand = null;
                }
            }

            return dataSet;
        }

        /// <summary>
        /// Executes a statement against a database and returns the first table populated in the <see cref="DataSet" />.
        /// </summary>
        /// <param name="databaseCommand"><see cref="DatabaseCommand" /> instance.</param>
        /// <param name="keepConnectionOpen">Optional parameter indicating whether to keep the connection open. Default is false.</param>
        /// <returns>
        /// DataTable representing an in-memory cache of the first <see cref="DataTable" /> result set from the returned
        /// <see cref="DataSet" />.
        /// </returns>
        public static DataTable ExecuteToDataTable( this DatabaseCommand databaseCommand, bool keepConnectionOpen = false )
        {
            return databaseCommand.ExecuteToDataSet( keepConnectionOpen ).Tables[ 0 ];
        }

        #endregion Execute Functions
    }

    /// <summary>Provides methods for accessing and caching <see cref="Type" /> metadata.</summary>
    public static class TypeCacher
    {
        /// <summary>
        /// Cache that stores types as the key and the type's PropertyInfo and FieldInfo in a <see cref="OrderedDictionary"/> as the value.
        /// </summary>
        private static readonly Dictionary<Type, OrderedDictionary> PropertiesAndFieldsCache = new Dictionary<Type, OrderedDictionary>();

        /// <summary>Gets the types properties and fields and caches the results.</summary>
        /// <param name="type">Type.</param>
        /// <returns><see cref="OrderedDictionary"/> of lowercase member names and PropertyInfo or FieldInfo as the values.</returns>
        public static OrderedDictionary GetPropertiesAndFields( Type type )
        {
            if ( PropertiesAndFieldsCache.ContainsKey( type ) )
            {
                return PropertiesAndFieldsCache[ type ];
            }

            var orderedDictionary = new OrderedDictionary( StringComparer.InvariantCultureIgnoreCase );
            
            PropertyInfo[] properties = type.GetProperties();

            foreach ( PropertyInfo propertyInfo in properties )
            {
                orderedDictionary[ propertyInfo.Name ] = propertyInfo;
            }

            FieldInfo[] fields = type.GetFields();

            foreach ( FieldInfo fieldInfo in fields )
            {
                orderedDictionary[ fieldInfo.Name ] = fieldInfo;
            }

            PropertiesAndFieldsCache.Add( type, orderedDictionary );

            return orderedDictionary;
        }
    }

    /// <summary>
    /// <see cref="DbConnection"/> extensions.
    /// </summary>
    public static class DbConnectionExtensions
    {
        static readonly PropertyInfo ProviderFactoryPropertyInfo = typeof( DbConnection ).GetProperty( "ProviderFactory", BindingFlags.Instance | BindingFlags.NonPublic );

        /// <summary>
        /// Gets the database provider factory for the given <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="dbConnection">The database connection.</param>
        /// <returns><see cref="DbProviderFactory"/>.</returns>
        public static DbProviderFactory GetDbProviderFactory( this DbConnection dbConnection )
        {
            // Note that in .NET v4.5 we could use this new method instead which would avoid the reflection:
            // DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory( databaseCommand.DbCommand.Connection );

            return ( DbProviderFactory ) ProviderFactoryPropertyInfo.GetValue( dbConnection, null );
        }
    }

    /// <summary>
    /// <see cref="Type" /> extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Cache that stores the default value for value types to reduce unnecessary redundant Activator.CreateInstance calls.
        /// </summary>
        public static readonly ConcurrentDictionary<Type, object> GetDefaultValueCache = new ConcurrentDictionary<Type, object>();

        /// <summary>Gets the default value for the given type.</summary>
        /// <param name="type">Type to get the default value for.</param>
        /// <returns>Default value of the given type.</returns>
        public static object GetDefaultValue( this Type type )
        {
            return type.IsValueType ? GetDefaultValueCache.GetOrAdd( type, Activator.CreateInstance ) : null;
        }
    }

    /// <summary>
    /// <see cref="object" /> extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>Converts the given object to a type of <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type to convert to.</typeparam>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Instance of type <typeparamref name="T" />.</returns>
        /// <exception cref="TypeConverter.TypeConversionException">
        /// Thrown when an error occurs attempting to convert a value to an
        /// enum.
        /// </exception>
        /// <exception cref="TypeConverter.TypeConversionException">
        /// Thrown when an error occurs attempting to convert a value to a
        /// type.
        /// </exception>
        public static T ConvertTo<T>( this object obj )
        {
            return ( T ) TypeConverter.ConvertType( obj, typeof ( T ) );
        }

        /// <summary>Converts the given object to an <see cref="int" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Int representation of the object.</returns>
        public static int ToInt( this object obj )
        {
            return ConvertTo<int>( obj );
        }

        /// <summary>Converts the given object to an <see cref="int" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Int representation of the object.</returns>
        public static int? ToNullableInt( this object obj )
        {
            return ConvertTo<int?>( obj );
        }

        /// <summary>Converts the given object to an <see cref="long" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Long representation of the object.</returns>
        public static long ToLong( this object obj )
        {
            return ConvertTo<long>( obj );
        }

        /// <summary>Converts the given object to an <see cref="long" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Long representation of the object.</returns>
        public static long? ToNullableLong( this object obj )
        {
            return ConvertTo<long?>( obj );
        }

        /// <summary>Converts the given object to an <see cref="double" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Double representation of the object.</returns>
        public static double ToDouble( this object obj )
        {
            return ConvertTo<double>( obj );
        }

        /// <summary>Converts the given object to an <see cref="double" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Double representation of the object.</returns>
        public static double? ToNullableDouble( this object obj )
        {
            return ConvertTo<double?>( obj );
        }

        /// <summary>Converts the given object to an <see cref="decimal" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Decimal representation of the object.</returns>
        public static decimal ToDecimal( this object obj )
        {
            return ConvertTo<decimal>( obj );
        }

        /// <summary>Converts the given object to an <see cref="decimal" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Decimal representation of the object.</returns>
        public static decimal? ToNullableDecimal( this object obj )
        {
            return ConvertTo<decimal?>( obj );
        }

        /// <summary>Converts the given object to an <see cref="DateTime" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>DateTime representation of the object.</returns>
        public static DateTime ToDateTime( this object obj )
        {
            return ConvertTo<DateTime>( obj );
        }

        /// <summary>Converts the given object to an <see cref="DateTime" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>DateTime representation of the object.</returns>
        public static DateTime? ToNullableDateTime( this object obj )
        {
            return ConvertTo<DateTime?>( obj );
        }

        /// <summary>Converts the given object to an <see cref="bool" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Bool representation of the object.</returns>
        public static bool ToBool( this object obj )
        {
            return ConvertTo<bool>( obj );
        }

        /// <summary>Converts the given object to an <see cref="bool" />.</summary>
        /// <param name="obj">Object to convert.</param>
        /// <returns>Bool representation of the object.</returns>
        public static bool? ToNullableBool( this object obj )
        {
            return ConvertTo<bool?>( obj );
        }

        /// <summary>Indicates if the object is an anonymous type.</summary>
        /// <param name="obj">Object instance.</param>
        /// <returns>Returns true if the object is an anonymous type.</returns>
        public static bool IsAnonymousType( this object obj )
        {
            if ( obj == null )
                return false;

            return obj.GetType().Namespace == null;
        }
    }

    /// <summary>Provides type conversion helpers.</summary>
    public static class TypeConverter
    {
        /// <summary>Converts the given value to the given type.</summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="type">Type to convert the given value to.</param>
        /// <returns>Converted value.</returns>
        /// <exception cref="TypeConversionException">Thrown when an error occurs attempting to convert a value to an enum.</exception>
        /// <exception cref="TypeConversionException">Thrown when an error occurs attempting to convert a value to a type.</exception>
        public static object ConvertType( object value, Type type )
        {
            // Handle DBNull
            if ( value == DBNull.Value )
                value = null;

            // Handle value type conversion of null to the values types default value
            if ( value == null && type.IsValueType )
                return type.GetDefaultValue(); // Extension method internally handles caching

            Type underlyingType = Nullable.GetUnderlyingType( type ) ?? type;

            // Handle Enums
            if ( underlyingType.IsEnum )
            {
                try
                {
                    // ReSharper disable once PossibleNullReferenceException // Because an enum and a nullable enum are both value types, it's actually not possible to reach the next line of code when the value variable is null
                    value = Enum.Parse( underlyingType, value.ToString(), true );
                }
                catch ( Exception exception )
                {
                    throw new TypeConversionException( String.Format( "An error occurred while attempting to convert the value '{0}' to an enum of type '{1}'", value, underlyingType ), exception );
                }
            }

            try
            {
                // Handle Guids
                if ( underlyingType == typeof ( Guid ) )
                {
                    if ( value is string )
                    {
                        value = new Guid( value as string );
                    }
                    if ( value is byte[] )
                    {
                        value = new Guid( value as byte[] );
                    }
                }

                object result = Convert.ChangeType( value, underlyingType );

                return result;
            }
            catch ( Exception exception )
            {
                throw new TypeConversionException( String.Format( "An error occurred while attempting to convert the value '{0}' to type '{1}'", value, underlyingType ), exception );
            }
        }

        /// <summary>Thrown when an exception occurs while converting a value from one type to another.</summary>
        [Serializable]
        public class TypeConversionException : Exception
        {
            /// <summary>Instantiates a new <see cref="TypeConversionException" /> with a specified error message.</summary>
            /// <param name="message">The message that describes the error.</param>
            /// <param name="innerException">
            /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner
            /// exception is specified.
            /// </param>
            public TypeConversionException( string message, Exception innerException )
                : base( message, innerException )
            {
            }
        }
    }

    /// <summary>Provides methods for mapping an <see cref="IDataRecord" /> to different types.</summary>
    public static class DataRecordMapper
    {
        /// <summary>Maps an <see cref="IDataRecord" /> to a type of <typeparamref name="T" />.</summary>
        /// <remarks>This method internally uses caching to increase performance.</remarks>
        /// <typeparam name="T">The type to map to.</typeparam>
        /// <param name="dataRecord">The <see cref="IDataRecord" /> to map from.</param>
        /// <returns>A mapped instance of <typeparamref name="T" />.</returns>
        /// <exception cref="PropertySetValueException">
        /// Thrown when an error occurs when attempting to assign a value to a
        /// property.
        /// </exception>
        /// <exception cref="FieldSetValueException">Thrown when an error occurs when attempting to assign a value to a field.</exception>
        public static T Map<T>( this IDataRecord dataRecord )
        {
            Type type = typeof ( T );

            int fieldCount = dataRecord.FieldCount;

            // Handle mapping to primitives and strings when there is only a single field in the record
            if ( fieldCount == 1 && ( type.IsPrimitive || type == typeof ( string ) ) )
            {
                object convertedValue = TypeConverter.ConvertType( dataRecord.GetValue( 0 ), type );

                return ( T ) convertedValue;
            }

            object mappedObject = type.GetDefaultValue() ?? Activator.CreateInstance<T>();

            bool didAssignValues = false;

            // OrderedDictionary where the key is a case-insensitive property or field name and the value is the members corresponding PropertyInfo or FieldInfo
            OrderedDictionary orderedDictionary = TypeCacher.GetPropertiesAndFields( type );

            for ( int i = 0; i < fieldCount; i++ )
            {
                string dataRecordFieldName = dataRecord.GetName( i ).ToLower();

                object memberInfo = orderedDictionary[dataRecordFieldName];

                if ( memberInfo != null )
                {
                    if ( memberInfo is PropertyInfo )
                    {
                        var propertyInfo = memberInfo as PropertyInfo;

                        if ( propertyInfo.CanWrite )
                        {
                            object value = dataRecord.GetValue( i );

                            object convertedValue = TypeConverter.ConvertType( value, propertyInfo.PropertyType );

                            try
                            {
                                propertyInfo.SetValue( mappedObject, convertedValue, null );

                                didAssignValues = true;
                            }
                            catch ( Exception exception )
                            {
                                throw new PropertySetValueException( string.Format( "An error occurred while attempting to assign the value '{0}' to property '{1}' of type '{2}' on class type {3}", value, propertyInfo.Name, propertyInfo.PropertyType, type ), exception );
                            }
                        }
                    }
                    else if ( memberInfo is FieldInfo )
                    {
                        var fieldInfo = memberInfo as FieldInfo;

                        object value = dataRecord.GetValue( i );

                        object convertedValue = TypeConverter.ConvertType( value, fieldInfo.FieldType );

                        try
                        {
                            fieldInfo.SetValue( mappedObject, convertedValue );

                            didAssignValues = true;
                        }
                        catch ( Exception exception )
                        {
                            throw new FieldSetValueException( string.Format( "An error occurred while attempting to assign the value '{0}' to field '{1}' of type '{2}' on class type {3}", value, fieldInfo.Name, fieldInfo.FieldType, type ), exception );
                        }
                    }
                }
            }

            // If no values were assigned, attempt to map the value directly to the type
            if ( didAssignValues == false && fieldCount == 1 )
            {
                object convertedValue = TypeConverter.ConvertType( dataRecord.GetValue( 0 ), type );

                mappedObject = ( T ) convertedValue;
            }

            return ( T ) mappedObject;
        }

        /// <summary>Maps an <see cref="IDataRecord" /> to a type of dynamic object.</summary>
        /// <param name="dataRecord">The <see cref="IDataRecord" /> to map from.</param>
        /// <returns>A dynamic object.</returns>
        public static dynamic MapDynamic( this IDataRecord dataRecord )
        {
            dynamic obj = new DynamicDictionary();

            for ( int i = 0; i < dataRecord.FieldCount; i++ )
            {
                string dataRecordFieldName = dataRecord.GetName( i );

                object dataRecordFieldValue = dataRecord.GetValue( i );

                if ( dataRecordFieldValue == DBNull.Value )
                    dataRecordFieldValue = null;

                obj[ dataRecordFieldName ] = dataRecordFieldValue;
            }

            return obj;
        }

        /// <summary>Exception thrown when setting a fields value.</summary>
        [Serializable]
        public class FieldSetValueException : Exception
        {
            /// <summary>Instantiates a new <see cref="FieldSetValueException" /> with a specified error message.</summary>
            /// <param name="message">The message that describes the error.</param>
            /// <param name="innerException">
            /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner
            /// exception is specified.
            /// </param>
            public FieldSetValueException( string message, Exception innerException )
                : base( message, innerException )
            {
            }
        }

        /// <summary>Exception thrown when setting a properties value.</summary>
        [Serializable]
        public class PropertySetValueException : Exception
        {
            /// <summary>Instantiates a new <see cref="PropertySetValueException" /> with a specified error message.</summary>
            /// <param name="message">The message that describes the error.</param>
            /// <param name="innerException">
            /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner
            /// exception is specified.
            /// </param>
            public PropertySetValueException( string message, Exception innerException )
                : base( message, innerException )
            {
            }
        }
    }

    /// <summary>
    /// <see cref="DbCommand" /> extensions.
    /// </summary>
    public static class DbCommandExtensions
    {
        /// <summary>Sets the text command to run against the data source.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="commandText">The text command to run against the data source.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand SetCommandText( this DbCommand dbCommand, string commandText )
        {
            dbCommand.CommandText = commandText;

            return dbCommand;
        }

        /// <summary>Appends to the text command to run against the data source.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="commandText">Text command to append to the text command to run against the data source.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand AppendCommandText( this DbCommand dbCommand, string commandText )
        {
            dbCommand.CommandText += commandText;

            return dbCommand;
        }

        /// <summary>Creates a <see cref="DbParameter" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter( this DbCommand dbCommand, string parameterName, object parameterValue )
        {
            DbParameter parameter = dbCommand.CreateParameter();

            parameter.ParameterName = parameterName;

            parameter.Value = parameterValue;

            return parameter;
        }

        /// <summary>Creates a <see cref="DbParameter" /> with a given <see cref="DbType" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter( this DbCommand dbCommand, string parameterName, object parameterValue, DbType dbType )
        {
            DbParameter parameter = dbCommand.CreateParameter();

            parameter.ParameterName = parameterName;

            parameter.Value = parameterValue;

            parameter.DbType = dbType;

            return parameter;
        }

        /// <summary>
        /// Creates a <see cref="DbParameter" /> with a given <see cref="DbType" /> and <see cref="ParameterDirection" />.
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <param name="parameterDirection">Parameter direction.</param>
        /// <returns><see cref="DbParameter" />.</returns>
        public static DbParameter CreateParameter( this DbCommand dbCommand, string parameterName, object parameterValue, DbType dbType, ParameterDirection parameterDirection )
        {
            DbParameter parameter = dbCommand.CreateParameter();

            parameter.ParameterName = parameterName;

            parameter.Value = parameterValue;

            parameter.DbType = dbType;

            parameter.Direction = parameterDirection;

            return parameter;
        }

        /// <summary>Adds a <see cref="DbParameter" /> to the <see cref="DbCommand" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="dbParameter"><see cref="DbParameter" /> to add.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbParameter" /> parameter is null.</exception>
        public static DbCommand AddParameter( this DbCommand dbCommand, DbParameter dbParameter )
        {
            if ( dbParameter == null )
            {
                throw new ArgumentNullException( "dbParameter" );
            }

            dbCommand.Parameters.Add( dbParameter );

            return dbCommand;
        }

        /// <summary>Adds a parameter to the <see cref="DbCommand" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterName" /> parameter is null.</exception>
        public static DbCommand AddParameter( this DbCommand dbCommand, string parameterName, object parameterValue )
        {
            if ( String.IsNullOrWhiteSpace( parameterName ) )
            {
                throw new ArgumentNullException( "parameterName" );
            }

            DbParameter parameter = dbCommand.CreateParameter( parameterName, parameterValue );

            dbCommand.Parameters.Add( parameter );

            return dbCommand;
        }

        /// <summary>Adds a parameter to the <see cref="DbCommand" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterName" /> parameter is null.</exception>
        public static DbCommand AddParameter( this DbCommand dbCommand, string parameterName, object parameterValue, DbType dbType )
        {
            if ( String.IsNullOrWhiteSpace( parameterName ) )
            {
                throw new ArgumentNullException( "parameterName" );
            }

            DbParameter parameter = dbCommand.CreateParameter( parameterName, parameterValue, dbType );

            dbCommand.Parameters.Add( parameter );

            return dbCommand;
        }

        /// <summary>Adds a parameter whose default value is <see cref="DBNull" /> when unassigned, to the <see cref="DbCommand" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterName" /> parameter is null.</exception>
        public static DbCommand AddNullableParameter( this DbCommand dbCommand, string parameterName, object parameterValue, DbType dbType )
        {
            if ( parameterValue is string 
                 && string.IsNullOrEmpty( parameterValue.ToString() ) )
            {
                parameterValue = null;
            }

            dbCommand = parameterValue == null
                ? dbCommand.AddParameter( parameterName, DBNull.Value )
                : dbCommand.AddParameter( parameterName, parameterValue, dbType );

            return dbCommand;
        }

        /// <summary>Adds a list of <see cref="DbParameter" />s to the <see cref="DbCommand" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="dbParameters">List of database parameters.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand AddParameters( this DbCommand dbCommand, IEnumerable<DbParameter> dbParameters )
        {
            foreach ( DbParameter dbParameter in dbParameters )
            {
                dbCommand.AddParameter( dbParameter );
            }

            return dbCommand;
        }

        /// <summary>Adds a parameter array of <see cref="DbParameter" />s to the <see cref="DbCommand" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="dbParameters">Parameter array of database parameters.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand AddParameters( this DbCommand dbCommand, params DbParameter[] dbParameters )
        {
            foreach ( DbParameter dbParameter in dbParameters )
            {
                dbCommand.AddParameter( dbParameter );
            }

            return dbCommand;
        }

        /// <summary>Adds a dictionary of parameter names and values to the <see cref="DbCommand" />.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="parameterNameAndValueDictionary">Dictionary of parameter names and values.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="parameterNameAndValueDictionary" /> parameter
        /// is null.
        /// </exception>
        public static DbCommand AddParameters( this DbCommand dbCommand, IDictionary<string, object> parameterNameAndValueDictionary )
        {
            if ( parameterNameAndValueDictionary == null )
            {
                throw new ArgumentNullException( "parameterNameAndValueDictionary" );
            }

            foreach ( var parameterNameAndValue in parameterNameAndValueDictionary )
            {
                dbCommand.AddParameter( parameterNameAndValue.Key, parameterNameAndValue.Value );
            }

            return dbCommand;
        }

        /// <summary>
        /// Adds the list of parameter values to the <see cref="DbCommand" /> by replacing the given parameterName in the
        /// <see cref="DbCommand.CommandText" /> with a comma delimited list of generated parameter names such as "parameterName0,
        /// parameterName1, parameterName2", etc.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValues">Parameter values.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterName" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterValues" /> parameter is null.</exception>
        /// <exception cref="Exception">Thrown when the <paramref name="parameterValues" /> list is empty.</exception>
        /// <exception cref="Exception">
        /// Thrown when the <paramref name="dbCommand" /> CommandText has not been set prior to calling this method.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the <paramref name="dbCommand" /> CommandText does not contain the
        /// <paramref name="parameterName" />.
        /// </exception>
        public static DbCommand AddParameters<T>( this DbCommand dbCommand, string parameterName, List<T> parameterValues )
        {
            if ( String.IsNullOrWhiteSpace( parameterName ) )
            {
                throw new ArgumentNullException( "parameterName" );
            }

            if ( parameterValues == null )
            {
                throw new ArgumentNullException( "parameterValues" );
            }

            if ( parameterValues.Count == 0 )
            {
                throw new Exception( "Parameter values list is empty." );
            }

            if ( String.IsNullOrWhiteSpace( dbCommand.CommandText ) )
            {
                throw new Exception( "The CommandText must already be set before calling this method." );
            }

            if ( dbCommand.CommandText.Contains( parameterName ) == false )
            {
                throw new Exception( string.Format( "The CommandText does not contain the parameter name '{0}'", parameterName ) );
            }

            var parameterNames = new List<string>();

            foreach ( T parameterValue in parameterValues )
            {
                // Note that we are appending the ordinal parameter position as a suffix to the parameter name in order to create
                // some uniqueness for each parameter name as well as to aid in debugging.
                string paramName = parameterName + "_p" + dbCommand.Parameters.Count;

                parameterNames.Add( paramName );

                dbCommand.AddParameter( paramName, parameterValue );
            }

            string commaDelimitedString = string.Join( ",", parameterNames );

            dbCommand.CommandText = dbCommand.CommandText.Replace( parameterName, commaDelimitedString );

            return dbCommand;
        }

        /// <summary>
        /// Adds the list of parameter values of the specified <see cref="DbType" /> to the <see cref="DbCommand" /> by replacing
        /// the given parameterName in the <see cref="DbCommand.CommandText" /> with a comma delimited list of generated parameter
        /// names such as "parameterName0, parameterName1, parameterName2", etc.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValues">Parameter values.</param>
        /// <param name="dbType">Parameter type.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterName" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="parameterValues" /> parameter is null.</exception>
        /// <exception cref="Exception">Thrown when the <paramref name="parameterValues" /> list is empty.</exception>
        /// <exception cref="Exception">
        /// Thrown when the <paramref name="dbCommand" /> CommandText has not been set prior to calling this method.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when the <paramref name="dbCommand" /> CommandText does not contain the
        /// <paramref name="parameterName" />.
        /// </exception>
        public static DbCommand AddParameters<T>( this DbCommand dbCommand, string parameterName, List<T> parameterValues, DbType dbType )
        {
            if ( String.IsNullOrWhiteSpace( parameterName ) )
            {
                throw new ArgumentNullException( "parameterName" );
            }

            if ( parameterValues == null )
            {
                throw new ArgumentNullException( "parameterValues" );
            }

            if ( parameterValues.Count == 0 )
            {
                throw new Exception( "Parameter values list is empty." );
            }

            if ( String.IsNullOrWhiteSpace( dbCommand.CommandText ) )
            {
                throw new Exception( "The CommandText must already be set before calling this method." );
            }

            if ( dbCommand.CommandText.Contains( parameterName ) == false )
            {
                throw new Exception( string.Format( "The CommandText does not contain the parameter name '{0}'", parameterName ) );
            }

            var parameterNames = new List<string>();

            foreach ( T parameterValue in parameterValues )
            {
                // Note that we are appending the ordinal parameter position as a suffix to the parameter name in order to create
                // some uniqueness for each parameter name as well as to aid in debugging.
                string paramName = parameterName + "_p" + dbCommand.Parameters.Count;

                parameterNames.Add( paramName );

                dbCommand.AddParameter( paramName, parameterValue, dbType );
            }

            string commaDelimitedString = string.Join( ",", parameterNames );

            dbCommand.CommandText = dbCommand.CommandText.Replace( parameterName, commaDelimitedString );

            return dbCommand;
        }

        /// <summary>Sets the CommandType.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="commandType">CommandType which specifies how a command string is interpreted.</param>
        /// <returns>The given <see cref="DatabaseCommand" /> instance.</returns>
        public static DbCommand SetCommandType( this DbCommand dbCommand, CommandType commandType )
        {
            dbCommand.CommandType = commandType;

            return dbCommand;
        }

        /// <summary>
        /// Sets the time in seconds to wait for the command to execute before throwing an exception. The default is 30 seconds.
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="commandTimeoutSeconds">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand SetCommandTimeout( this DbCommand dbCommand, int commandTimeoutSeconds )
        {
            dbCommand.CommandTimeout = commandTimeoutSeconds;

            return dbCommand;
        }

        /// <summary>Sets the transaction associated with the command.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="dbTransaction">The transaction to associate with the command.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand SetTransaction( this DbCommand dbCommand, DbTransaction dbTransaction )
        {
            dbCommand.Transaction = dbTransaction;

            return dbCommand;
        }

        /// <summary>
        /// Starts a database transaction and associates it with the <see cref="DbCommand"/> instance.
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <returns>An object representing the new transaction.</returns>
        public static DbTransaction BeginTransaction( this DbCommand dbCommand )
        {
            dbCommand.OpenConnection();

            DbTransaction transaction = dbCommand.Connection.BeginTransaction();

            dbCommand.SetTransaction( transaction );

            return transaction;
        }

        /// <summary>
        /// Starts a database transaction with the specified isolation level and associates it with the <see cref="DbCommand"/> instance.
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
        /// <returns>An object representing the new transaction.</returns>
        public static DbTransaction BeginTransaction( this DbCommand dbCommand, IsolationLevel isolationLevel )
        {
            dbCommand.OpenConnection();

            DbTransaction transaction = dbCommand.Connection.BeginTransaction( isolationLevel );

            dbCommand.SetTransaction( transaction );

            return transaction;
        }

        /// <summary>Opens a database connection.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        public static DbCommand OpenConnection( this DbCommand dbCommand )
        {
            if ( dbCommand.Connection.State != ConnectionState.Open )
            {
                dbCommand.Connection.Open();
            }

            return dbCommand;
        }

        /// <summary>Opens a database connection asynchronously.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <returns>A <see cref="Task{TResult}"/> resulting in the given <see cref="DbCommand" /> instance.</returns>
        public static async Task<DbCommand> OpenConnectionAsync(this DbCommand dbCommand)
        {
            if (dbCommand.Connection.State != ConnectionState.Open)
            {
                await dbCommand.Connection.OpenAsync();
            }

            return dbCommand;
        }

        /// <summary>
        /// Closes and disposes the <see cref="DbCommand.Connection" /> and the <see cref="DbCommand" /> itself.
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        public static void CloseAndDispose( this DbCommand dbCommand )
        {
            dbCommand.Connection.Close();

            dbCommand.Connection.Dispose();

            dbCommand.Dispose();
        }

        /// <summary>Gets the command text with the parameters replaced with their values.</summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <returns>Parameter replaced command text.</returns>
        public static string GetParameterReplacedCommandText( this DbCommand dbCommand )
        {
            string commandText = dbCommand.CommandText;

            foreach ( DbParameter parameter in dbCommand.Parameters )
            {
                string replacementText = string.Format( "'{0}' /* {1} */", parameter.Value, parameter.ParameterName );

                commandText = commandText.Replace( parameter.ParameterName, replacementText );
            }

            return commandText;
        }

        /// <summary>
        /// Generates a SQL statement for debugging purposes which includes the parameter replaced command text and command and
        /// connection information in SQL comments.
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <returns>SQL statement for debugging purposes.</returns>
        public static string GetDebugCommandText( this DbCommand dbCommand )
        {
            var sb = new StringBuilder();

            sb.AppendLine( "/******** COMMAND & CONNECTION INFO ********/" );
            sb.AppendLine( "/*" );
            sb.AppendLine( "Application Name: " + AppDomain.CurrentDomain.FriendlyName );
            sb.AppendLine( "Database Server: " + dbCommand.Connection.DataSource );
            sb.AppendLine( "Database Name: " + dbCommand.Connection.Database );
            sb.AppendLine( "Connection String: " + dbCommand.Connection.ConnectionString );
            sb.AppendLine( "Connection State: " + dbCommand.Connection.State );
            sb.AppendLine( "Command Timeout: " + dbCommand.CommandTimeout + " sec." );
            sb.AppendLine( "Command Parameter Count: " + dbCommand.Parameters.Count );
            sb.AppendLine( "Client Machine Name: " + Environment.MachineName );
            sb.AppendLine( "Client DNS Host Name: " + Dns.GetHostEntry( "LocalHost" ).HostName );
            sb.AppendLine( "Current DateTime: " + DateTime.Now );
            sb.AppendLine( "*/" );
            sb.AppendLine();

            string commandText = dbCommand.GetParameterReplacedCommandText();

            sb.AppendLine( "/******** SQL QUERY ********/" );
            sb.AppendLine();
            sb.AppendLine( commandText );
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Generates a parameterized MySQL INSERT statement from the given object and adds it to the <see cref="DbCommand" />
        /// .
        /// <para>
        /// Note that the generated query also selects the last inserted id using MySQL's SELECT LAST_INSERT_ID() function.
        /// </para>
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="obj">Object to generate the SQL INSERT statement from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DbCommand GenerateInsertForMySql( this DbCommand dbCommand, object obj, string tableName = null )
        {
            const string mySqlInsertStatementTemplate = @"
INSERT INTO {0}
({1}
)
VALUES
({2}
);
SELECT LAST_INSERT_ID() AS LastInsertedId;
"; // Intentional line break for readability of multiple inserts

            return dbCommand.GenerateInsertCommand( obj, mySqlInsertStatementTemplate, tableName, KeywordEscapeMethod.Backtick );
        }

        /// <summary>
        /// Generates a list of concatenated parameterized MySQL INSERT statements from the given list of objects and adds it to
        /// the <see cref="DbCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using MySQL's SELECT LAST_INSERT_ID() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="listOfObjects">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DbCommand GenerateInsertsForMySql<T>( this DbCommand dbCommand, List<T> listOfObjects, string tableName = null )
        {
            foreach( T obj in listOfObjects )
            {
                dbCommand.GenerateInsertForMySql( obj, tableName );
            }

            return dbCommand;
        }

        /// <summary>
        /// Generates a parameterized PostgreSQL INSERT statement from the given object and adds it to the <see cref="DbCommand" />
        /// .
        /// <para>
        /// Note that the generated query also selects the last inserted id using PostgreSQL's LastVal() function.
        /// </para>
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="obj">Object to generate the SQL INSERT statement from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DbCommand GenerateInsertForPostgreSQL(this DbCommand dbCommand, object obj, string tableName = null)
        {
            const string postgreSQLInsertStatementTemplate = @"
INSERT INTO {0}
({1}
)
VALUES
({2}
);
select LastVal();
";

            return dbCommand.GenerateInsertCommand(obj, postgreSQLInsertStatementTemplate, tableName, KeywordEscapeMethod.None);
        }

        /// <summary>
        /// Generates a list of concatenated parameterized PostgreSQL INSERT statements from the given list of objects and adds it to
        /// the <see cref="DbCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using PostgreSQL's LastVal() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="listOfObjects">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DbCommand GenerateInsertsForPostgreSQL<T>(this DbCommand dbCommand, List<T> listOfObjects, string tableName = null)
        {
            foreach(T obj in listOfObjects)
            {
                dbCommand.GenerateInsertForPostgreSQL(obj, tableName);
            }

            return dbCommand;
        }

        /// <summary>
        /// Generates a parameterized SQLite INSERT statement from the given object and adds it to the <see cref="DbCommand" />
        /// .
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQLite's SELECT last_insert_rowid() function.
        /// </para>
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="obj">Object to generate the SQL INSERT statement from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        public static DbCommand GenerateInsertForSQLite( this DbCommand dbCommand, object obj, string tableName = null )
        {
            const string sqliteInsertStatementTemplate = @"
INSERT INTO {0}
({1}
)
VALUES
({2}
);
SELECT last_insert_rowid() AS [LastInsertedId];
"; // Intentional line break for readability of multiple inserts

            return dbCommand.GenerateInsertCommand( obj, sqliteInsertStatementTemplate, tableName, KeywordEscapeMethod.SquareBracket );
        }

        /// <summary>
        /// Generates a list of concatenated parameterized SQLite INSERT statements from the given list of objects and adds it to
        /// the <see cref="DbCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQLite's SELECT last_insert_rowid() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="listOfObjects">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        public static DbCommand GenerateInsertsForSQLite<T>( this DbCommand dbCommand, List<T> listOfObjects, string tableName = null )
        {
            foreach ( T obj in listOfObjects )
            {
                dbCommand.GenerateInsertForSQLite( obj, tableName );
            }

            return dbCommand;
        }

        /// <summary>
        /// Generates a parameterized SQL Server INSERT statement from the given object and adds it to the
        /// <see cref="DbCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQL Server's SELECT SCOPE_IDENTITY() function.
        /// </para>
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="obj">Object to generate the SQL INSERT statement from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DbCommand GenerateInsertForSqlServer( this DbCommand dbCommand, object obj, string tableName = null )
        {
            const string sqlServerInsertStatementTemplate = @"
INSERT INTO {0}
({1}
)
VALUES
({2}
);
SELECT SCOPE_IDENTITY() AS [LastInsertedId];
"; // Intentional line break for readability of multiple inserts

            return dbCommand.GenerateInsertCommand( obj, sqlServerInsertStatementTemplate, tableName, KeywordEscapeMethod.SquareBracket );
        }

        /// <summary>
        /// Generates a list of concatenated parameterized SQL Server INSERT statements from the given list of objects and adds it
        /// to the <see cref="DbCommand" />.
        /// <para>
        /// Note that the generated query also selects the last inserted id using SQL Server's SELECT SCOPE_IDENTITY() function.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list.</typeparam>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="listOfObjects">List of objects to generate the SQL INSERT statements from.</param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DbCommand GenerateInsertsForSqlServer<T>( this DbCommand dbCommand, List<T> listOfObjects, string tableName = null )
        {
            foreach ( T obj in listOfObjects )
            {
                dbCommand.GenerateInsertForSqlServer( obj, tableName );
            }

            return dbCommand;
        }

        /// <summary>
        /// The method used for escaping keywords.
        /// </summary>
        public enum KeywordEscapeMethod
        {
            /// <summary>No escape method is used.</summary>
            None = 0,
            /// <summary>Keywords are enclosed in square brackets. Used by SQL Server, SQLite.</summary>
            SquareBracket = 1,
            /// <summary>Keywords are enclosed in double quotes. Used by PostgreSQL, SQLite.</summary>
            DoubleQuote = 2,
            /// <summary>Keywords are enclosed in backticks aka grave accents (ASCII code 96). Used by MySQL, SQLite.</summary>
            Backtick = 3
        }

        /// <summary>
        /// Generates a parameterized SQL Server INSERT statement from the given object and adds it to the
        /// <see cref="DbCommand" />.
        /// </summary>
        /// <param name="dbCommand"><see cref="DbCommand" /> instance.</param>
        /// <param name="obj">Object to generate the SQL INSERT statement from.</param>
        /// <param name="sqlInsertStatementTemplate">
        /// SQL INSERT statement template where argument 0 is the table name, argument 1 is the comma delimited list of columns,
        /// and argument 2 is the comma delimited list of values.
        /// <para>Example: INSERT INTO {0} ({1}) VALUES({2});</para>
        /// </param>
        /// <param name="tableName">
        /// Optional table name to insert into. If none is supplied, it will use the type name. Note that this parameter is
        /// required when passing in an anonymous object or an <see cref="ArgumentNullException" /> will be thrown.
        /// </param>
        /// <param name="keywordEscapeMethod">The method used for escaping keywords.</param>
        /// <returns>The given <see cref="DbCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of 'tableName' cannot be null when the object passed is an anonymous
        /// type.
        /// </exception>
        public static DbCommand GenerateInsertCommand( this DbCommand dbCommand, object obj, string sqlInsertStatementTemplate, string tableName = null, KeywordEscapeMethod keywordEscapeMethod = KeywordEscapeMethod.None )
        {
            if ( obj == null )
            {
                throw new ArgumentNullException( "obj" );
            }

            if ( sqlInsertStatementTemplate == null )
            {
                throw new ArgumentNullException( "sqlInsertStatementTemplate" );
            }

            if ( String.IsNullOrWhiteSpace( sqlInsertStatementTemplate ) )
            {
                throw new ArgumentNullException( "sqlInsertStatementTemplate", "The 'sqlInsertStatementTemplate' parameter must not be null, empty, or whitespace." );
            }

            if ( sqlInsertStatementTemplate.Contains( "{0}" ) == false || sqlInsertStatementTemplate.Contains( "{1}" ) == false || sqlInsertStatementTemplate.Contains( "{2}" ) == false )
            {
                throw new Exception( "The 'sqlInsertStatementTemplate' parameter does not conform to the template requirements of containing three string.Format arguments. A valid example is: INSERT INTO {0} ({1}) VALUES({2});" );
            }

            if ( tableName == null && obj.IsAnonymousType() )
            {
                throw new ArgumentNullException( "tableName", "The 'tableName' parameter must be provided when the object supplied is an anonymous type." );
            }

            string preKeywordEscapeCharacter = "";

            string postKeywordEscapeCharacter = "";

            switch ( keywordEscapeMethod )
            {
                case KeywordEscapeMethod.SquareBracket:
                    preKeywordEscapeCharacter = "[";
                    postKeywordEscapeCharacter = "]";
                    break;
                case KeywordEscapeMethod.DoubleQuote:
                    preKeywordEscapeCharacter = "\"";
                    postKeywordEscapeCharacter = "\"";
                    break;
                case KeywordEscapeMethod.Backtick:
                    preKeywordEscapeCharacter = "`";
                    postKeywordEscapeCharacter = "`";
                    break;
            }

            if ( tableName == null )
            {
                tableName = preKeywordEscapeCharacter + obj.GetType().Name + postKeywordEscapeCharacter;
            }

            string linePrefix = Environment.NewLine + "\t";

            string columns = string.Empty;

            string values = string.Empty;
            
            IDictionary<string, object> namesAndValues = GetPropertyAndFieldNamesAndValues( obj );
            
            foreach( var nameAndValue in namesAndValues )
            {
                if ( nameAndValue.Value == null )
                    continue;

                columns += linePrefix + preKeywordEscapeCharacter + nameAndValue.Key + postKeywordEscapeCharacter + ",";

                // Note that we are appending the ordinal parameter position as a suffix to the parameter name in order to create
                // some uniqueness for each parameter name so that this method can be called repeatedly as well as to aid in debugging.
                string parameterName = "@" + nameAndValue.Key + "_p" + dbCommand.Parameters.Count;

                values += linePrefix + parameterName + ",";

                dbCommand.AddParameter( parameterName, nameAndValue.Value );
            }

            dbCommand.AppendCommandText( string.Format( sqlInsertStatementTemplate, tableName, columns.TrimEnd( ',' ), values.TrimEnd( ',' ) ) );

            return dbCommand;
        }

        /// <summary>Gets a dictionary containing the objects property and field names and values.</summary>
        /// <param name="obj">Object to get names and values from.</param>
        /// <returns>Dictionary containing property and field names and values.</returns>
        private static IDictionary<string, object> GetPropertyAndFieldNamesAndValues( object obj )
        {
            // Support dynamic objects backed by a dictionary of string object.
            var objectAsDictionary = obj as IDictionary<string, object>;

            if ( objectAsDictionary != null )
                return objectAsDictionary;

            Type type = obj.GetType();

            OrderedDictionary orderedDictionary = TypeCacher.GetPropertiesAndFields( type );

            var dictionary = new Dictionary<string, object>();

            foreach ( DictionaryEntry entry in orderedDictionary )
            {
                object value = null;

                if ( entry.Value is FieldInfo )
                {
                    var fieldInfo = entry.Value as FieldInfo;

                    value = fieldInfo.GetValue( obj );
                }
                else if ( entry.Value is PropertyInfo )
                {
                    var propertyInfo = entry.Value as PropertyInfo;

                    value = propertyInfo.GetValue( obj, null );
                }

                dictionary.Add( entry.Key.ToString(), value );
            }

            return dictionary;
        }
    }

#pragma warning disable 1591 // The DynamicDictionary is meant to be used as a dynamic thus XML comments are unnecessary

    /*    
    DynamicDictionary implementation obtained from: https://github.com/randyburden/DynamicDictionary    

    The MIT License (MIT)

    Copyright (c) 2014 Randy Burden ( http://randyburden.com ) All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
    */

    /// <summary>
    /// A dynamic dictionary allowing case-insensitive access and returns null when accessing non-existent properties.
    /// </summary>
    /// <example>
    /// // Non-existent properties will return null dynamic obj = new DynamicDictionary(); var firstName = obj.FirstName;
    /// Assert.Null( firstName ); // Allows case-insensitive property access dynamic obj = new DynamicDictionary();
    /// obj.SuperHeroName = "Superman"; Assert.That( obj.SUPERMAN == "Superman" ); Assert.That( obj.superman == "Superman" );
    /// Assert.That( obj.sUpErMaN == "Superman" );
    /// </example>
    public class DynamicDictionary : DynamicObject, IDictionary<string, object>
    {
        private readonly IDictionary<string, object> _dictionary = new DefaultValueDictionary<string, object>( StringComparer.InvariantCultureIgnoreCase );

        #region DynamicObject Overrides

        public override bool TryGetMember( GetMemberBinder binder, out object result )
        {
            result = _dictionary[ binder.Name ];

            return true;
        }

        public override bool TrySetMember( SetMemberBinder binder, object value )
        {
            if ( _dictionary.ContainsKey( binder.Name ) )
            {
                _dictionary[ binder.Name ] = value;
            }
            else
            {
                _dictionary.Add( binder.Name, value );
            }

            return true;
        }

        public override bool TryInvokeMember( InvokeMemberBinder binder, object[] args, out object result )
        {
            if ( _dictionary.ContainsKey( binder.Name ) && _dictionary[ binder.Name ] is Delegate )
            {
                var delegateValue = _dictionary[ binder.Name ] as Delegate;

                result = delegateValue.DynamicInvoke( args );

                return true;
            }

            return base.TryInvokeMember( binder, args, out result );
        }

        #endregion DynamicObject Overrides

        #region IDictionary<string,object> Members

        public void Add( string key, object value )
        {
            _dictionary.Add( key, value );
        }

        public bool ContainsKey( string key )
        {
            return _dictionary.ContainsKey( key );
        }

        public ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public bool Remove( string key )
        {
            return _dictionary.Remove( key );
        }

        public bool TryGetValue( string key, out object value )
        {
            return _dictionary.TryGetValue( key, out value );
        }

        public ICollection<object> Values
        {
            get { return _dictionary.Values; }
        }

        public object this[ string key ]
        {
            get
            {
                object value;

                _dictionary.TryGetValue( key, out value );

                return value;
            }
            set { _dictionary[ key ] = value; }
        }

        #endregion IDictionary<string,object> Members

        #region ICollection<KeyValuePair<string,object>> Members

        public void Add( KeyValuePair<string, object> item )
        {
            _dictionary.Add( item );
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains( KeyValuePair<string, object> item )
        {
            return _dictionary.Contains( item );
        }

        public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex )
        {
            _dictionary.CopyTo( array, arrayIndex );
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        public bool Remove( KeyValuePair<string, object> item )
        {
            return _dictionary.Remove( item );
        }

        #endregion ICollection<KeyValuePair<string,object>> Members

        #region IEnumerable<KeyValuePair<string,object>> Members

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion IEnumerable<KeyValuePair<string,object>> Members

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region Nested Types

        /// <summary>
        /// A dictionary that returns the default value when accessing keys that do not exist in the dictionary.
        /// </summary>
        public class DefaultValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            private readonly IDictionary<TKey, TValue> _dictionary;

            #region Constructors

            /// <summary>
            /// Initializes a dictionary that returns the default value when accessing keys that do not exist in the dictionary.
            /// </summary>
            public DefaultValueDictionary()
            {
                _dictionary = new Dictionary<TKey, TValue>();
            }

            /// <summary>Initializes with an existing dictionary.</summary>
            /// <param name="dictionary"></param>
            public DefaultValueDictionary( IDictionary<TKey, TValue> dictionary )
            {
                _dictionary = dictionary;
            }

            /// <summary>Initializes using the given equality comparer.</summary>
            /// <param name="comparer"></param>
            public DefaultValueDictionary( IEqualityComparer<TKey> comparer )
            {
                _dictionary = new Dictionary<TKey, TValue>( comparer );
            }

            #endregion Constructors

            #region IDictionary<string,TValue> Members

            public void Add( TKey key, TValue value )
            {
                _dictionary.Add( key, value );
            }

            public bool ContainsKey( TKey key )
            {
                return _dictionary.ContainsKey( key );
            }

            public ICollection<TKey> Keys
            {
                get { return _dictionary.Keys; }
            }

            public bool Remove( TKey key )
            {
                return _dictionary.Remove( key );
            }

            public bool TryGetValue( TKey key, out TValue value )
            {
                return _dictionary.TryGetValue( key, out value );
            }

            public ICollection<TValue> Values
            {
                get { return _dictionary.Values; }
            }

            public TValue this[ TKey key ]
            {
                get
                {
                    TValue value;

                    _dictionary.TryGetValue( key, out value );

                    return value;
                }
                set { _dictionary[ key ] = value; }
            }

            #endregion IDictionary<string,TValue> Members

            #region ICollection<KeyValuePair<string,TValue>> Members

            public void Add( KeyValuePair<TKey, TValue> item )
            {
                _dictionary.Add( item );
            }

            public void Clear()
            {
                _dictionary.Clear();
            }

            public bool Contains( KeyValuePair<TKey, TValue> item )
            {
                return _dictionary.Contains( item );
            }

            public void CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex )
            {
                _dictionary.CopyTo( array, arrayIndex );
            }

            public int Count
            {
                get { return _dictionary.Count; }
            }

            public bool IsReadOnly
            {
                get { return _dictionary.IsReadOnly; }
            }

            public bool Remove( KeyValuePair<TKey, TValue> item )
            {
                return _dictionary.Remove( item );
            }

            #endregion ICollection<KeyValuePair<TKey,TValue>> Members

            #region IEnumerable<KeyValuePair<TKey,TValue>> Members

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            #endregion IEnumerable<KeyValuePair<TKey,TValue>> Members

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            #endregion IEnumerable Members
        }

#pragma warning restore 1591 // The DynamicDictionary is meant to be used as a dynamic thus XML comments are unnecessary

        #endregion Nested Types
    }
}