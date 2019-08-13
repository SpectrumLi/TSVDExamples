using System;
using System.Data.Common;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ToDatabaseCommandTests
    {
        [Test]
        public void Should_Handle_Getting_A_DatabaseCommand()
        {
            // Arrange
            var dbProviderFactory = DbProviderFactories.GetFactory( "System.Data.SqlClient" );

            var connection = dbProviderFactory.CreateConnection();

            var dbCommand = connection.CreateCommand();

            // Act
            var databaseCommand = dbCommand.ToDatabaseCommand();

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.DbCommand == dbCommand );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_If_The_DatabaseCommand_Is_Null()
        {
            // Arrange
            DbCommand dbCommand = null;

            // Act
            TestDelegate action = () => dbCommand.ToDatabaseCommand();

            // Assert
            Assert.Throws<ArgumentNullException>( action );
        }
    }
}