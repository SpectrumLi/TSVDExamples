using System.Configuration;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SQLite.SequelocityTests
{
    [TestFixture]
    public class GetDatabaseCommandForSqLiteTests
    {
        [Test]
        public void Can_Get_A_DatabaseCommand_For_Sqlite()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString].ConnectionString;

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommandForSQLite( connectionString );

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.DbCommand.Connection.ToString() == "System.Data.SQLite.SQLiteConnection" );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }
    }
}