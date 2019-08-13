using System;
using System.Configuration;
using System.Diagnostics;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SqlServer.DbCommandExtensionsTests
{
    [TestFixture]
    public class GetDebugCommandTextTests
    {
        public class Customer
        {
            public int? CustomerId;
            public string FirstName;
            public string LastName;
            public DateTime DateOfBirth;
        }

        [Test]
        public void Should_Contain_The_Connection_String()
        {
            // Arrange
            const string sql = @"
CREATE TABLE #Customer
(
    CustomerId      INT         NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL
);
";
            string connectionString = ConfigurationManager.ConnectionStrings[ ConnectionStringsNames.SqlServerConnectionString ].ConnectionString;

            var dbConnection = Sequelocity.CreateDbConnection( connectionString, "System.Data.SqlClient" );

            new DatabaseCommand( dbConnection )
                .SetCommandText( sql )
                .ExecuteNonQuery( true );

            var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
            var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };

            var databaseCommand = new DatabaseCommand( dbConnection )
                .GenerateInsertForSqlServer( customer )
                .GenerateInsertForSqlServer( customer2 );

            // Act
            var debugCommandText = databaseCommand.DbCommand.GetDebugCommandText();

            // Visual Assertion
            Trace.WriteLine( debugCommandText );

            // Assert
            Assert.That( debugCommandText.Contains( connectionString.Substring( 0, 10 ) ) ); // Using a substring as the framework will remove the password so we can't anticipate the entire connection string will be shown
        }

        [Test]
        public void Should_Contain_Parameter_Replaced_CommandText()
        {
            // Arrange
            const string sql = @"
CREATE TABLE #Customer
(
    CustomerId      INT         NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL
);
";
            string connectionString = ConfigurationManager.ConnectionStrings[ ConnectionStringsNames.SqlServerConnectionString ].ConnectionString;

            var dbConnection = Sequelocity.CreateDbConnection( connectionString, "System.Data.SqlClient" );

            new DatabaseCommand( dbConnection )
                .SetCommandText( sql )
                .ExecuteNonQuery( true );

            var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
            var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };

            var databaseCommand = new DatabaseCommand( dbConnection )
                .GenerateInsertForSqlServer( customer )
                .GenerateInsertForSqlServer( customer2 );

            // Act

            var debugCommandText = databaseCommand.DbCommand.GetDebugCommandText();

            // Visual Assertion
            Trace.WriteLine( debugCommandText );

            // Assert
            Assert.That( debugCommandText.Contains( customer.FirstName ) );
            Assert.That( debugCommandText.Contains( customer.LastName ) );
        }
    }
}