using NUnit.Framework;

namespace SequelocityDotNet.Tests.PostgreSQL.SequelocityTests
{
    public class GetDatabaseCommandForPostgreSQLTests
    {
        [Test]
        public void Can_Get_A_DatabaseCommand_For_A_PostgreSQL()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionString = ConnectionStringsNames.PostgreSQLConnectionString;

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommandForPostgreSQL(connectionString);

            // Assert
            Assert.NotNull(databaseCommand);
            Assert.That(databaseCommand.DbCommand.Connection.ToString() == "Npgsql.NpgsqlConnection");

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }
    }
}
