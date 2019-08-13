using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class SetCommandTextTests
    {
        [Test]
        public void Should_Handle_Setting_The_CommandText_Of_The_DbCommand()
        {
            // Arrange
            const string commandText = "SELECT * FROM SuperHero";

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.SetCommandText( commandText );

            // Assert
            Assert.That( dbCommand.CommandText == commandText );
        }

        [Test]
        public void Should_Overwrite_Any_Existing_CommandText()
        {
            // Arrange
            const string commandText = "SELECT * FROM SuperHero";

            var dbCommand = TestHelpers.GetDbCommand()
                .SetCommandText( "Hello World!" );

            // Act
            dbCommand = dbCommand.SetCommandText( commandText );

            // Assert
            Assert.That( dbCommand.CommandText == commandText );
        }
    }
}