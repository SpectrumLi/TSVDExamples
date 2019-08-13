using NUnit.Framework;

namespace SequelocityDotNet.Tests.PostgreSQL.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class BeginTransactionTests
    {
        [Test]
        public void Should_Return_A_New_DbTransaction()
        {
            // Arrange
            var databaseCommand = Sequelocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString);

            // Act
            var transaction = databaseCommand.BeginTransaction();

            // Assert
            Assert.NotNull(transaction);
            Assert.That(databaseCommand.DbCommand.Connection == transaction.Connection);
        }

        [Test]
        public void Should_Associate_The_DbTransaction_With_The_DatabaseCommand()
        {
            // Arrange
            var databaseCommand = Sequelocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString);

            // Act
            var transaction = databaseCommand.BeginTransaction();

            // Assert
            Assert.NotNull(databaseCommand.DbCommand.Transaction == transaction);
        }
    }
}