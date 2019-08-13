using NUnit.Framework;

namespace SequelocityDotNet.Tests.MySql.SequelocityTests
{
    [TestFixture]
    public class GetDatabaseCommandForMySqlTests
    {
        [Test]
        public void Can_Get_A_DatabaseCommand_For_A_MySql()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionString = ConnectionStringsNames.MySqlConnectionString;

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommandForMySql( connectionString );

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.DbCommand.Connection.ToString() == "MySql.Data.MySqlClient.MySqlConnection" );
            
            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }
    }
}