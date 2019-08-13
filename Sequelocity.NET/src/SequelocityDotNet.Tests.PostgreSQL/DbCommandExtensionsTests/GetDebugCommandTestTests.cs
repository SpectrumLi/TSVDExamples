using System;
using System.Configuration;
using System.Diagnostics;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.PostgreSQL.DbCommandExtensionsTests
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
DROP TABLE IF EXISTS Customer;

CREATE TEMPORARY TABLE Customer
(
    CustomerId      serial not null,
    FirstName       VARCHAR(120)   NOT NULL,
    LastName        VARCHAR(120)   NOT NULL,
    DateOfBirth     timestamp        NOT NULL,
    PRIMARY KEY ( CustomerId )
);
";
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringsNames.PostgreSQLConnectionString].ConnectionString;

            var dbConnection = Sequelocity.CreateDbConnection(connectionString, "Npgsql");
            connectionString = dbConnection.ConnectionString;

            new DatabaseCommand(dbConnection)
                .SetCommandText(sql)
                .ExecuteNonQuery(true);

            var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse("06/18/1938") };
            var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse("05/27/1939") };

            var databaseCommand = new DatabaseCommand(dbConnection)
                .GenerateInsertForPostgreSQL(customer)
                .GenerateInsertForPostgreSQL(customer2);

            // Act
            var debugCommandText = databaseCommand.DbCommand.GetDebugCommandText();

            dbConnection.Close();

            // Visual Assertion
            Trace.WriteLine(debugCommandText);

            // Assert
            Assert.That(debugCommandText.Contains(connectionString.Substring(0, 10))); // Using a substring as the framework will remove the password so we can't anticipate the entire connection string will be shown
            //Note: the connection string changes slightly to default all keys to uppercase and some names to different names (i.e. USERNAME to USER ID). Connection string provided may need to be altered to accomodate.
        }

        [Test]
        public void Should_Contain_Parameter_Replaced_CommandText()
        {
            // Arrange
            const string sql = @"
DROP TABLE IF EXISTS Customer;

CREATE TEMPORARY TABLE Customer
(
    CustomerId      serial not null,
    FirstName       VARCHAR(120)   NOT NULL,
    LastName        VARCHAR(120)   NOT NULL,
    DateOfBirth     timestamp        NOT NULL,
    PRIMARY KEY ( CustomerId )
);
";
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringsNames.PostgreSQLConnectionString].ConnectionString;

            var dbConnection = Sequelocity.CreateDbConnection(connectionString, "Npgsql");

            new DatabaseCommand(dbConnection)
                .SetCommandText(sql)
                .ExecuteNonQuery(true);

            var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse("06/18/1938") };
            var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse("05/27/1939") };

            var databaseCommand = new DatabaseCommand(dbConnection)
                .GenerateInsertForPostgreSQL(customer)
                .GenerateInsertForPostgreSQL(customer2);

            // Act

            var debugCommandText = databaseCommand.DbCommand.GetDebugCommandText();

            dbConnection.Close();

            // Visual Assertion
            Trace.WriteLine(debugCommandText);

            // Assert
            Assert.That(debugCommandText.Contains(customer.FirstName));
            Assert.That(debugCommandText.Contains(customer.LastName));
        }
    }
}