using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class SetCommandTypeTests
    {
        [Test]
        public void Should_Handle_Setting_The_CommandType()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const CommandType commandType = CommandType.StoredProcedure;

            // Act
            dbCommand = dbCommand.SetCommandType( commandType );

            // Assert
            Assert.That( dbCommand.CommandType == commandType );
        }
    }
}