using System;
using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class AddParameterTests
    {
        [Test]
        public void Should_Handle_Adding_A_DbParameter_To_The_Parameters_Collection_Of_The_DbCommand()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            var dbParameter = dbCommand.CreateParameter();
            dbParameter.ParameterName = "@SuperHeroName";
            dbParameter.Value = "Superman";
            dbParameter.Direction = ParameterDirection.InputOutput;

            // Act
            dbCommand = dbCommand.AddParameter( dbParameter );

            // Assert
            Assert.That( dbCommand.Parameters[dbParameter.ParameterName].Value == dbParameter.Value );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_When_Adding_A_Null_DbParameter()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            TestDelegate action = () => dbCommand.AddParameter( null );

            // Assert
            Assert.Throws<ArgumentNullException>( action );
        }

        [Test]
        public void Should_Handle_Adding_A_Parameter_To_The_Parameters_Collection_Of_The_DbCommand()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = "@SuperHeroName";

            object parameterValue = "Superman";

            // Act
            dbCommand = dbCommand.AddParameter( parameterName, parameterValue );

            // Assert
            Assert.That( dbCommand.Parameters[parameterName].Value == parameterValue );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_When_Adding_A_Parameter_With_A_Null_Parameter_Name()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = null;

            object parameterValue = "Superman";

            // Act
            TestDelegate action = () => dbCommand.AddParameter( parameterName, parameterValue );

            // Assert
            Assert.Throws<ArgumentNullException>( action );
        }

        [Test]
        public void Should_Handle_Adding_A_Parameter_With_A_DbType_To_The_Parameters_Collection_Of_The_DbCommand()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = "@SuperHeroName";

            object parameterValue = "Superman";

            // Act
            dbCommand = dbCommand.AddParameter( parameterName, parameterValue, DbType.AnsiString );

            // Assert
            Assert.That( dbCommand.Parameters[parameterName].Value == parameterValue );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_When_Adding_A_Parameter_With_A_Null_Parameter_Name_And_A_DbType()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = null;

            object parameterValue = "Superman";

            // Act
            TestDelegate action = () => dbCommand.AddParameter( parameterName, parameterValue, DbType.AnsiString );

            // Assert
            Assert.Throws<ArgumentNullException>( action );
        }
    }
}