using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class SetCommandTimeoutTests
    {
        [Test]
        public void Should_Handle_Setting_A_CommandTimeout()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const int commandTimeoutInSeconds = 60;

            // Act
            dbCommand = dbCommand.SetCommandTimeout( commandTimeoutInSeconds );

            // Assert
            Assert.That( dbCommand.CommandTimeout == commandTimeoutInSeconds );
        }
    }
}