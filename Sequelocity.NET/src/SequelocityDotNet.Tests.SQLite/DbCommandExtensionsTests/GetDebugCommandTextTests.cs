using System;
using System.Configuration;
using System.Diagnostics;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SQLite.DbCommandExtensionsTests
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
CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INTEGER         NOT NULL    PRIMARY KEY     AUTOINCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL
);";
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString].ConnectionString;

            var dbConnection = Sequelocity.CreateDbConnection( connectionString, "System.Data.SQLite" );

            new DatabaseCommand( dbConnection )
                .SetCommandText( sql )
                .ExecuteNonQuery( true );

            var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
            var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };

            var databaseCommand = new DatabaseCommand( dbConnection )
                .GenerateInsertForSQLite( customer )
                .GenerateInsertForSQLite( customer2 );

            // Act
            var debugCommandText = databaseCommand.DbCommand.GetDebugCommandText();

            // Visual Assertion
            Trace.WriteLine( debugCommandText );
            
            // Assert
            Assert.That( debugCommandText.Contains( connectionString ) );
        }

        [Test]
        public void Should_Contain_Parameter_Replaced_CommandText()
        {
            // Arrange
            const string sql = @"
CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INTEGER         NOT NULL    PRIMARY KEY     AUTOINCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL
);";
            string connectionStringName = ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString;

            var dbConnection = Sequelocity.CreateDbConnection( connectionStringName, "System.Data.SQLite" );

            new DatabaseCommand( dbConnection )
                .SetCommandText( sql )
                .ExecuteNonQuery( true );

            var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
            var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };

            var databaseCommand = new DatabaseCommand( dbConnection )
                .GenerateInsertForSQLite( customer )
                .GenerateInsertForSQLite( customer2 );

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