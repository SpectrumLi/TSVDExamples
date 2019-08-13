using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SqlServer.DbCommandExtensionsTests
{
    [TestFixture]
    public class AddNullableParameterTests
    {
        [Test]
        [TestCase( "VARCHAR(100)", DbType.AnsiString, "" )]
        [TestCase( "SMALLINT", DbType.Int16, null )]
        public void Should_Set_Sql_Parameter_To_Null_When_Null_Or_Empty_Is_Assigned(string sqlDataType, DbType dbType, object parameterValue)
        {
            // Arrange
            const string sql = @"
DECLARE @SuperHeroName {0} = @pSuperHeroName

SELECT @SuperHeroName as SuperHeroName
";
            string formattedSql = string.Format(sql, sqlDataType);

            // Act
            string result = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( formattedSql )
                .AddNullableParameter( "@pSuperHeroName", parameterValue, dbType )
                .ExecuteScalar<string>();

            // Assert
            Assert.That(result == null);
        }
    }
}