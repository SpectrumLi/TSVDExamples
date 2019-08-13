using NUnit.Framework;

namespace SequelocityDotNet.Tests.SqlServer.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class SetTransactionTests
    {
        [Test]
        public void Should_Associate_A_Transaction_With_The_DatabaseCommand()
        {
            // Arrange
            var connection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqlServerConnectionString );
            connection.Open();
            var transaction = connection.BeginTransaction();
            var databaseCommand = Sequelocity.GetDatabaseCommand( connection );

            // Act
            databaseCommand.SetTransaction( transaction );

            // Assert
            Assert.That( databaseCommand.DbCommand.Transaction == transaction );

            // Cleanup
            connection.Close();
        }
    }
}