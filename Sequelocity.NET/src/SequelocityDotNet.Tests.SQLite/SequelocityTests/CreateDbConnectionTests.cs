using System.Configuration;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SQLite.SequelocityTests
{
    [TestFixture]
    public class CreateDbConnectionTests
    {

        [Test]
        public void Should_Create_A_DbConnection_When_Passed_A_Connection_String_And_Provider_Name()
        {
            // Arrange
            string connectionStringName = ConfigurationManager.ConnectionStrings[ ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString ].ConnectionString;

            const string dbProviderFactoryInvariantName = "System.Data.SQLite";

            // Act
            var dbConnection = Sequelocity.CreateDbConnection( connectionStringName, dbProviderFactoryInvariantName );

            // Assert
            Assert.NotNull( dbConnection );

            // Cleanup
            dbConnection.Dispose();
        }

        [Test]
        public void Should_Create_A_DbConnection_When_Passed_A_Connection_String_Name()
        {
            // Arrange
            string connectionStringName = ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString;
            
            // Act
            var dbConnection = Sequelocity.CreateDbConnection( connectionStringName );

            // Assert
            Assert.NotNull( dbConnection );

            // Cleanup
            dbConnection.Dispose();
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_When_Passed_A_Null_ConnectionString()
        {
            // Arrange
            const string connectionString = null;

            const string dbProviderFactoryInvariantName = "System.Data.SQLite";

            // Act
            TestDelegate action = () => Sequelocity.CreateDbConnection( connectionString, dbProviderFactoryInvariantName );

            // Assert
            Assert.Throws<Sequelocity.ConnectionStringNotFoundException>( action );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_When_Passed_A_Null_DbProviderFactoryInvariantName()
        {
            // Arrange
            string connectionString = ConfigurationManager.ConnectionStrings[ ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString ].ConnectionString;

            const string dbProviderFactoryInvariantName = null;

            TestHelpers.ClearDefaultConfigurationSettings();

            // Act
            TestDelegate action = () => Sequelocity.CreateDbConnection( connectionString, dbProviderFactoryInvariantName );

            // Assert
            Assert.Throws<Sequelocity.DbProviderFactoryNotFoundException>( action );
        }

        [Test]
        public void Should_Null_The_DbCommand_On_Dispose()
        {
            // Arrange
            DatabaseCommand databaseCommand = Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString );

            // Act
            databaseCommand.Dispose();

            // Assert
            Assert.Null( databaseCommand.DbCommand );
        }

        [Test]
        public void Should_Null_The_DbCommand_On_Dispose_In_A_Using_Statement()
        {
            // Arrange
            DatabaseCommand databaseCommand;

            // Act
            using( databaseCommand = Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString ) )
            {

            }

            // Assert
            Assert.Null( databaseCommand.DbCommand );
        }
    }
}