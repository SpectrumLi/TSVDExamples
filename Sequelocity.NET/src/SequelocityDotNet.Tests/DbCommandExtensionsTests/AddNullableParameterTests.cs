using System;
using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class AddNullableParameterTests
    {
        [Test]
        [TestCase( null, DbType.Int16 )]
        [TestCase( null, DbType.AnsiString )]
        [TestCase( "", DbType.AnsiString )]
        public void Should_Set_Parameter_Value_To_DBNull_Given_That_Parameter_Is_Null_Or_Empty(object parameterValue, DbType dbType)
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = "@SuperHeroName";

            // Act
            dbCommand = dbCommand.AddNullableParameter( parameterName, parameterValue, dbType );

            // Assert
            Assert.That( dbCommand.Parameters[parameterName].Value == DBNull.Value );
        }

        [Test]
        [TestCase( 1234, DbType.Int16 )]
        [TestCase( "Green Lantern", DbType.AnsiString )]
        [TestCase( " ", DbType.AnsiString )]
        public void Should_Set_Parameter_To_The_Given_Value_When_Value_Is_Not_Null( object parameterValue, DbType dbType )
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = "@SuperHeroName";

            // Act
            dbCommand = dbCommand.AddNullableParameter( parameterName, parameterValue, dbType );

            // Assert
            Assert.That( dbCommand.Parameters[parameterName].Value == parameterValue );
        }
    }
}