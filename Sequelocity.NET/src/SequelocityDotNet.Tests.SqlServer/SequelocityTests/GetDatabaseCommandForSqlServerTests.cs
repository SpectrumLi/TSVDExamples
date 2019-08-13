using System.Configuration;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SqlServer.SequelocityTests
{
    [TestFixture]
    public class GetDatabaseCommandForSqlServerTests
    {
        [Test]
        public void Can_Get_A_DatabaseCommand_For_A_SqlServer()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringsNames.SqlServerConnectionString].ConnectionString;

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommandForSqlServer( connectionString );

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.DbCommand.Connection.ToString() == "System.Data.SqlClient.SqlConnection" );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }
    }
}