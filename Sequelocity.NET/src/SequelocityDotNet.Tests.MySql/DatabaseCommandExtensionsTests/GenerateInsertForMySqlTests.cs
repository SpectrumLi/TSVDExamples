using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.MySql.DatabaseCommandExtensionsTests
{
	[TestFixture]
	public class GenerateInsertForMySqlTests
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
            const string createSchemaSql = @"
DROP TABLE IF EXISTS Customer;

CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INT             NOT NULL    AUTO_INCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL,
    PRIMARY KEY ( CustomerId )
);
";
			Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( createSchemaSql )
				.ExecuteNonQuery();

			var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

			// Act
            var customerId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .GenerateInsertForMySql( customer )
				.ExecuteScalar()
				.ToInt();

			// Assert
			Assert.That( customerId == 1 );
		}

		[Test]
		public void Should_Handle_Generating_Inserts_For_A_Strongly_Typed_Object()
		{
			// Arrange
            const string createSchemaSql = @"
DROP TABLE IF EXISTS Customer;

CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INT             NOT NULL    AUTO_INCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL,
    PRIMARY KEY ( CustomerId )
);
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.SetCommandText( createSchemaSql )
				.ExecuteNonQuery();

			var newCustomer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

			// Act
            var customerId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.GenerateInsertForMySql( newCustomer )
				.ExecuteScalar()
				.ToInt();

			const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer;
";

            var customer = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
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
            const string createSchemaSql = @"
DROP TABLE IF EXISTS Person;

CREATE TABLE IF NOT EXISTS Person
(
    CustomerId      INT             NOT NULL    AUTO_INCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL,
    PRIMARY KEY ( CustomerId )
);
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( createSchemaSql )
				.ExecuteNonQuery();

			var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

			// Act
		    var customerId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
		        .GenerateInsertForMySql( customer, "Person" ) // Specifying a table name of Person
		        .ExecuteScalar<int>();

			// Assert
			Assert.That( customerId == 1 );
		}

		[Test]
		public void Should_Throw_An_Exception_When_Passing_An_Anonymous_Object_And_Not_Specifying_A_TableName()
		{
			// Arrange
            const string createSchemaSql = @"
DROP TABLE IF EXISTS Customer;

CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INT             NOT NULL    AUTO_INCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL,
    PRIMARY KEY ( CustomerId )
);
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( createSchemaSql )
				.ExecuteNonQuery();

			var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

			// Act
		    TestDelegate action = () => Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
		        .GenerateInsertForMySql( customer )
		        .ExecuteScalar<int>();

			// Assert
			var exception = Assert.Catch<ArgumentNullException>( action );
			Assert.That( exception.Message.Contains( "The 'tableName' parameter must be provided when the object supplied is an anonymous type." ) );
		}

		[Test]
		public void Should_Handle_Generating_Inserts_For_An_Anonymous_Object()
		{
			// Arrange
            const string createSchemaSql = @"
DROP TABLE IF EXISTS Customer;

CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INT             NOT NULL    AUTO_INCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL,
    PRIMARY KEY ( CustomerId )
);
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.SetCommandText( createSchemaSql )
				.ExecuteNonQuery();

			var newCustomer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

			// Act
            var customerId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.GenerateInsertForMySql( newCustomer, "Customer" )
                .ExecuteScalar<int>();

			const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer;
";

            var customer = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
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
		public void Should_Handle_Generating_Inserts_For_A_Dynamic_ExpandoObject()
		{
			// Arrange
            const string createSchemaSql = @"
DROP TABLE IF EXISTS Customer;

CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INT             NOT NULL    AUTO_INCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL,
    PRIMARY KEY ( CustomerId )
);
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.SetCommandText( createSchemaSql )
				.ExecuteNonQuery();

			dynamic newCustomer = new ExpandoObject();
			newCustomer.FirstName = "Clark";
			newCustomer.LastName = "Kent";
			newCustomer.DateOfBirth = DateTime.Parse( "06/18/1938" );

			// Act
            var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString );
			databaseCommand = DatabaseCommandExtensions.GenerateInsertForMySql( databaseCommand, newCustomer, "Customer" );
			var customerId = databaseCommand
                .ExecuteScalar<int>();

			const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer;
";

            var customer = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
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
        public void Should_Handle_Generating_Inserts_For_A_Dictionary_Of_String_Object()
        {
            // Arrange
            const string createSchemaSql = @"
DROP TABLE IF EXISTS Customer;

CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INT             NOT NULL    AUTO_INCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL,
    PRIMARY KEY ( CustomerId )
);
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( createSchemaSql )
                .ExecuteNonQuery();

            dynamic newCustomer = new ExpandoObject();
            newCustomer.FirstName = "Clark";
            newCustomer.LastName = "Kent";
            newCustomer.DateOfBirth = DateTime.Parse( "06/18/1938" );

            // Act
            var customerId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .GenerateInsertForMySql( ( IDictionary<string, object> ) newCustomer, "Customer" )
                .ExecuteScalar<int>();

            const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer;
";

            var customer = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
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
        public void Should_Handle_Generating_Inserts_For_An_Anonymous_Object_Converted_Into_A_Dynamic()
        {
            // Arrange
            const string createSchemaSql = @"
DROP TABLE IF EXISTS Customer;

CREATE TABLE IF NOT EXISTS Customer
(
    CustomerId      INT             NOT NULL    AUTO_INCREMENT,
    FirstName       NVARCHAR(120)   NOT NULL,
    LastName        NVARCHAR(120)   NOT NULL,
    DateOfBirth     DATETIME        NOT NULL,
    PRIMARY KEY ( CustomerId )
);
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( createSchemaSql )
                .ExecuteNonQuery();

            dynamic newCustomer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            // Act
            var customerId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .GenerateInsertForMySql( (object) newCustomer, "Customer" )
                .ExecuteScalar<int>();

            const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer;
";

            var customer = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
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