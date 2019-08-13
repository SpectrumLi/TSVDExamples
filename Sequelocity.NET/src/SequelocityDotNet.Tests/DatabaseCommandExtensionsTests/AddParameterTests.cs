using System;
using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class AddParameterTests
    {
        [Test]
        public void Should_Handle_Adding_A_DbParameter_To_The_Parameters_Collection_Of_The_DbCommand()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            var dbParameter = databaseCommand.DbCommand.CreateParameter();
            dbParameter.ParameterName = "SuperHeroName";
            dbParameter.Value = "Superman";
            dbParameter.Direction = ParameterDirection.InputOutput;

            // Act
            databaseCommand = databaseCommand.AddParameter( dbParameter );

            // Assert
            Assert.That( databaseCommand.DbCommand.Parameters[dbParameter.ParameterName].Value == dbParameter.Value );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_When_Adding_A_Null_DbParameter()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            TestDelegate action = () => databaseCommand.AddParameter( null );

            // Assert
            Assert.Throws<ArgumentNullException>( action );
        }

        [Test]
        public void Should_Handle_Adding_A_Parameter_To_The_Parameters_Collection_Of_The_DbCommand()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            const string parameterName = "@SuperHeroNames";

            object parameterValue = "Superman";

            // Act
            databaseCommand = databaseCommand.AddParameter( parameterName, parameterValue );

            // Assert
            Assert.That( databaseCommand.DbCommand.Parameters[parameterName].Value == parameterValue );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_When_Adding_A_Parameter_With_A_Null_Parameter_Name()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            const string parameterName = null;

            object parameterValue = "Superman";

            // Act
            TestDelegate action = () => databaseCommand.AddParameter( parameterName, parameterValue );

            // Assert
            Assert.Throws<ArgumentNullException>( action );
        }

        [Test]
        public void Should_Handle_Adding_A_Parameter_With_A_DbType_To_The_Parameters_Collection_Of_The_DbCommand()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            const string parameterName = "@SuperHeroNames";

            object parameterValue = "Superman";

            // Act
            databaseCommand = databaseCommand.AddParameter( parameterName, parameterValue, DbType.AnsiString );

            // Assert
            Assert.That( databaseCommand.DbCommand.Parameters[parameterName].Value == parameterValue );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_When_Adding_A_Parameter_With_A_Null_Parameter_Name_And_A_DbType()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            const string parameterName = null;

            object parameterValue = "Superman";

            // Act
            TestDelegate action = () => databaseCommand.AddParameter( parameterName, parameterValue, DbType.AnsiString );

            // Assert
            Assert.Throws<ArgumentNullException>( action );
        }
    }
}