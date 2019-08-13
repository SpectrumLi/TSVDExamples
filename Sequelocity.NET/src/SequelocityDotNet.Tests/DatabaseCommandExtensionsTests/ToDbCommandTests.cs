using System;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ToDbCommandTests
    {
        [Test]
        public void Should_Handle_Getting_The_DbCommand()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            var dbCommand = databaseCommand.ToDbCommand();

            // Assert
            Assert.NotNull( dbCommand );
            Assert.That( databaseCommand.DbCommand == dbCommand );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_If_The_DatabaseCommand_Is_Null()
        {
            // Arrange
            DatabaseCommand databaseCommand = null;

            // Act
            TestDelegate action = () => databaseCommand.ToDbCommand();

            // Assert
            Assert.Throws<ArgumentNullException>( action );
        }
    }
}