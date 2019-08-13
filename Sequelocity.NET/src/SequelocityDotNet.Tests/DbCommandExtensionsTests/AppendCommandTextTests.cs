using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class AppendCommandTextTests
    {
        [Test]
        public void Should_Handle_Setting_The_CommandText_Of_The_DbCommand()
        {
            // Arrange
            const string commandText = "SELECT * FROM SuperHero";

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.AppendCommandText( commandText );

            // Assert
            Assert.That( dbCommand.CommandText == commandText );
        }

        [Test]
        public void Should_Handle_Appending_To_The_CommandText_Of_The_DbCommand()
        {
            // Arrange
            const string commandText1 = "SELECT * FROM SuperHero;";
            const string commandText2 = "SELECT * FROM Monsters;";

            var dbCommand = TestHelpers.GetDbCommand()
                .SetCommandText( commandText1 );

            // Act
            dbCommand = dbCommand.AppendCommandText( commandText2 );

            // Assert
            Assert.That( dbCommand.CommandText == commandText1 + commandText2 );
        }
    }
}