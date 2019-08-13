using System;
using System.Dynamic;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SQLite.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class GenerateInsertForSqLiteTests
    {
        public class Customer
        {
            public int? CustomerId;
            public string FirstName;
            public string LastName;
            public DateTime DateOfBirth;
        }

        [Test]
        public void Should_Return_The_Last_Inserted_Id()
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
            var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString );

            new DatabaseCommand( dbConnection )
                .SetCommandText( sql )
                .ExecuteNonQuery( true );

            var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            // Act
            var customerId = new DatabaseCommand( dbConnection )
                .GenerateInsertForSQLite( customer )
                .ExecuteScalar( true )
                .ToInt();

            // Assert
            Assert.That( customerId == 1 );
        }

        [Test]
        public void Should_Handle_Generating_Inserts_For_A_Strongly_Typed_Object()
        {
            // Arrange
            const string createSchemaSql = @"
CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INTEGER         NOT NULL    PRIMARY KEY     AUTOINCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL
);";
            var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString );

            new DatabaseCommand( dbConnection )
                .SetCommandText( createSchemaSql )
                .ExecuteNonQuery( true );

            var newCustomer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            // Act
            var customerId = new DatabaseCommand( dbConnection )
                .GenerateInsertForSQLite( newCustomer )
                .ExecuteScalar( true )
                .ToInt();

            const string selectCustomerQuery = @"
SELECT  CustomerId,
        FirstName,
        LastName,
        DateOfBirth
FROM    Customer;
";

            var customer = new DatabaseCommand( dbConnection )
                .SetCommandText( selectCustomerQuery )
                .ExecuteToObject<Customer>();

            // Assert
            Assert.That( customerId == 1 );
            Assert.That( customer.CustomerId == 1 );
            Assert.That( customer.FirstName == newCustomer.FirstName );
            Assert.That( customer.LastName == newCustomer.LastName );
            Assert.That( customer.DateOfBirth == newCustomer.DateOfBirth );
        }

        [Test]
        public void Should_Be_Able_To_Specify_The_Table_Name()
        {
            // Arrange
            const string sql = @"
CREATE TABLE IF NOT EXISTS Person
(
    CustomerId      INTEGER         NOT NULL    PRIMARY KEY     AUTOINCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL
);";
            var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString );

            new DatabaseCommand( dbConnection )
                .SetCommandText( sql )
                .ExecuteNonQuery( true );

            var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            // Act
            var customerId = new DatabaseCommand( dbConnection )
                .GenerateInsertForSQLite( customer, "[Person]" ) // Specifying a table name of Person
                .ExecuteScalar( true )
                .ToInt();

            // Assert
            Assert.That( customerId == 1 );
        }

        [Test]
        public void Should_Throw_An_Exception_When_Passing_An_Anonymous_Object_And_Not_Specifying_A_TableName()
        {
            // Arrange
            const string sql = @"
CREATE TABLE IF NOT EXISTS Person
(
    CustomerId      INTEGER         NOT NULL    PRIMARY KEY     AUTOINCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL
);";
            var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString, "System.Data.SQLite" );

            new DatabaseCommand( dbConnection )
                .SetCommandText( sql )
                .ExecuteNonQuery( true );

            var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            // Act
            TestDelegate action = () => new DatabaseCommand( dbConnection )
                .GenerateInsertForSQLite( customer )
                .ExecuteScalar( true )
                .ToInt();

            // Assert
            var exception = Assert.Catch<ArgumentNullException>( action );
            Assert.That( exception.Message.Contains( "The 'tableName' parameter must be provided when the object supplied is an anonymous type." ) );
        }

        [Test]
        public void Should_Handle_Generating_Inserts_For_An_Anonymous_Object()
        {
            // Arrange
            const string createSchemaSql = @"
CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INTEGER         NOT NULL    PRIMARY KEY     AUTOINCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL
);";
            var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString );

            new DatabaseCommand( dbConnection )
                .SetCommandText( createSchemaSql )
                .ExecuteNonQuery( true );

            var newCustomer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            // Act
            var customerId = new DatabaseCommand( dbConnection )
                .GenerateInsertForSQLite( newCustomer, "[Customer]" )
                .ExecuteScalar( true )
                .ToInt();

            const string selectCustomerQuery = @"
SELECT  CustomerId,
        FirstName,
        LastName,
        DateOfBirth
FROM    Customer;
";

            var customer = new DatabaseCommand( dbConnection )
                .SetCommandText( selectCustomerQuery )
                .ExecuteToObject<Customer>();

            // Assert
            Assert.That( customerId == 1 );
            Assert.That( customer.CustomerId == 1 );
            Assert.That( customer.FirstName == newCustomer.FirstName );
            Assert.That( customer.LastName == newCustomer.LastName );
            Assert.That( customer.DateOfBirth == newCustomer.DateOfBirth );
        }

        [Test]
        public void Should_Handle_Generating_Inserts_For_A_Dynamic_Object()
        {
            // Arrange
            const string createSchemaSql = @"
CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INTEGER         NOT NULL    PRIMARY KEY     AUTOINCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL
);";
            var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString );

            new DatabaseCommand( dbConnection )
                .SetCommandText( createSchemaSql )
                .ExecuteNonQuery( true );

            dynamic newCustomer = new ExpandoObject();
            newCustomer.FirstName = "Clark";
            newCustomer.LastName = "Kent";
            newCustomer.DateOfBirth = DateTime.Parse( "06/18/1938" );

            // Act
            var databaseCommand = new DatabaseCommand( dbConnection );
            databaseCommand = DatabaseCommandExtensions.GenerateInsertForSQLite( databaseCommand, newCustomer, "[Customer]" );
            var customerId = databaseCommand
                .ExecuteScalar( true )
                .ToInt();

            const string selectCustomerQuery = @"
SELECT  CustomerId,
        FirstName,
        LastName,
        DateOfBirth
FROM    Customer;
";

            var customer = new DatabaseCommand( dbConnection )
                .SetCommandText( selectCustomerQuery )
                .ExecuteToObject<Customer>();

            // Assert
            Assert.That( customerId == 1 );
            Assert.That( customer.CustomerId == 1 );
            Assert.That( customer.FirstName == newCustomer.FirstName );
            Assert.That( customer.LastName == newCustomer.LastName );
            Assert.That( customer.DateOfBirth == newCustomer.DateOfBirth );
        }
    }
}