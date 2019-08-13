using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class CreateParameterTests
    {
        [Test]
        public void Should_Handle_Creating_A_DbParameter_With_A_Parameter_Name_And_Value()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = "@SuperHeroName";

            object parameterValue = "Superman";

            // Act
            var superHeroNameParameter = dbCommand.CreateParameter( parameterName, parameterValue );

            // Assert
            Assert.That( superHeroNameParameter.ParameterName == parameterName );
            Assert.That( superHeroNameParameter.Value == parameterValue );
        }

        [Test]
        public void Should_Handle_Creating_A_DbParameter_With_A_Parameter_Name_And_Value_And_DbType()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = "@SuperHeroName";

            object parameterValue = "Superman";

            const DbType dbType = DbType.AnsiString;
            
            // Act
            var superHeroNameParameter = dbCommand.CreateParameter( parameterName, parameterValue, dbType );

            // Assert
            Assert.That( superHeroNameParameter.ParameterName == parameterName );
            Assert.That( superHeroNameParameter.Value == parameterValue );
            Assert.That( superHeroNameParameter.DbType == dbType );
        }

        [Test]
        public void Should_Handle_Creating_A_DbParameter_With_A_Parameter_Name_And_Value_And_DbType_And_Parameter_Direction()
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = "@SuperHeroName";

            object parameterValue = "Superman";

            const DbType dbType = DbType.AnsiString;

            const ParameterDirection parameterDirection = ParameterDirection.Output;
            
            // Act
            var superHeroNameParameter = dbCommand.CreateParameter( parameterName, parameterValue, dbType, parameterDirection );

            // Assert
            Assert.That( superHeroNameParameter.ParameterName == parameterName );
            Assert.That( superHeroNameParameter.Value == parameterValue );
            Assert.That( superHeroNameParameter.DbType == dbType );
            Assert.That( superHeroNameParameter.Direction == parameterDirection );
        }
    }
}