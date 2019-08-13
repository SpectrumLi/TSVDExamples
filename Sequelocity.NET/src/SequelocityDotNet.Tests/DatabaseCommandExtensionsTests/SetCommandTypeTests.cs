using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class SetCommandTypeTests
    {
        [Test]
        public void Should_Handle_Setting_The_CommandType()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            const CommandType commandType = CommandType.StoredProcedure;

            // Act
            databaseCommand = databaseCommand.SetCommandType( commandType );

            // Assert
            Assert.That( databaseCommand.DbCommand.CommandType == commandType );
        }
    }
}