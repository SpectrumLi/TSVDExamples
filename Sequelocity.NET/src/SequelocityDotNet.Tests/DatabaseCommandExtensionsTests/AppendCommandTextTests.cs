using NUnit.Framework;

namespace SequelocityDotNet.Tests.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class AppendCommandTextTests
    {
        [Test]
        public void Should_Handle_Setting_The_CommandText_Of_The_DbCommand()
        {
            // Arrange
            const string commandText = "SELECT * FROM SuperHero";

            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            databaseCommand = databaseCommand.AppendCommandText( commandText );

            // Assert
            Assert.That( databaseCommand.DbCommand.CommandText == commandText );
        }

        [Test]
        public void Should_Handle_Appending_To_The_CommandText_Of_The_DbCommand()
        {
            // Arrange
            const string commandText1 = "SELECT * FROM SuperHero;";
            const string commandText2 = "SELECT * FROM Monsters;";

            var databaseCommand = TestHelpers.GetDatabaseCommand()
                .SetCommandText( commandText1 );

            // Act
            databaseCommand = databaseCommand.AppendCommandText( commandText2 );

            // Assert
            Assert.That( databaseCommand.DbCommand.CommandText == commandText1 + commandText2 );
        }
    }
}