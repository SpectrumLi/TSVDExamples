using NUnit.Framework;

namespace SequelocityDotNet.Tests.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class SetCommandTextTests
    {
        [Test]
        public void Should_Handle_Setting_The_CommandText_Of_The_DbCommand()
        {
            // Arrange
            const string commandText = "SELECT * FROM SuperHero";

            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            databaseCommand = databaseCommand.SetCommandText( commandText );

            // Assert
            Assert.That( databaseCommand.DbCommand.CommandText == commandText );
        }

        [Test]
        public void Should_Overwrite_Any_Existing_CommandText()
        {
            // Arrange
            const string commandText = "SELECT * FROM SuperHero";

            var databaseCommand = TestHelpers.GetDatabaseCommand()
                .SetCommandText( "Hello World!" );

            // Act
            databaseCommand = databaseCommand.SetCommandText( commandText );

            // Assert
            Assert.That( databaseCommand.DbCommand.CommandText == commandText );
        }
    }
}