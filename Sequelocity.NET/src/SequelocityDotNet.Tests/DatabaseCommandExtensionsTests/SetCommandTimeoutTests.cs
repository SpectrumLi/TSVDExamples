using NUnit.Framework;

namespace SequelocityDotNet.Tests.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class SetCommandTimeoutTests
    {
        [Test]
        public void Should_Handle_Setting_A_CommandTimeout()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            const int commandTimeoutInSeconds = 60;

            // Act
            databaseCommand = databaseCommand.SetCommandTimeout( commandTimeoutInSeconds );

            // Assert
            Assert.That( databaseCommand.DbCommand.CommandTimeout == commandTimeoutInSeconds );
        }
    }
}