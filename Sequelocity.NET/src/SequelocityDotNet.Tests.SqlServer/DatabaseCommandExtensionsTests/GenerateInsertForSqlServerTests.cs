using System;
using System.Dynamic;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SqlServer.DatabaseCommandExtensionsTests
{
	[TestFixture]
	public class GenerateInsertForSqlServerTests
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
IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'Customer' ) )
BEGIN

	DROP TABLE Customer

END

IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'Customer') )
BEGIN

	CREATE TABLE Customer
	(
		CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
		FirstName       NVARCHAR(120)   NOT NULL,
		LastName        NVARCHAR(120)   NOT NULL,
		DateOfBirth     DATETIME        NOT NULL
	);

END
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.SetCommandText( sql )
				.ExecuteNonQuery();

			var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

			// Act
            var customerId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.GenerateInsertForSqlServer( customer )
                .ExecuteScalar<int>();

			// Assert
			Assert.That( customerId == 1 );
		}

		[Test]
		public void Should_Handle_Generating_Inserts_For_A_Strongly_Typed_Object()
		{
			// Arrange
			const string createSchemaSql = @"
IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'Customer' ) )
BEGIN

	DROP TABLE Customer

END

IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'Customer') )
BEGIN

	CREATE TABLE Customer
	(
		CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
		FirstName       NVARCHAR(120)   NOT NULL,
		LastName        NVARCHAR(120)   NOT NULL,
		DateOfBirth     DATETIME        NOT NULL
	);

END
";

			Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.SetCommandText( createSchemaSql )
				.ExecuteNonQuery();

			var newCustomer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

			// Act
            var customerId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.GenerateInsertForSqlServer( newCustomer )
                .ExecuteScalar<int>();

			const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer;
";

            var customer = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
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
IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'Person' ) )
BEGIN

	DROP TABLE Person

END

IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'Person') )
BEGIN

	CREATE TABLE Person
	(
		CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
		FirstName       NVARCHAR(120)   NOT NULL,
		LastName        NVARCHAR(120)   NOT NULL,
		DateOfBirth     DATETIME        NOT NULL
	);

END
";

            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.SetCommandText( sql )
				.ExecuteNonQuery();

			var customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

			// Act
            var customerId = Sequelocity.GetDatabaseCommandForSqlServer( ConnectionStringsNames.SqlServerConnectionString )
				.GenerateInsertForSqlServer( customer, "[Person]" ) // Specifying a table name of Person
				.ExecuteScalar<int>();

			// Assert
			Assert.That( customerId == 1 );
		}

		[Test]
		public void Should_Throw_An_Exception_When_Passing_An_Anonymous_Object_And_Not_Specifying_A_TableName()
		{
			// Arrange
			const string sql = @"
IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'Person' ) )
BEGIN

	DROP TABLE Person

END

IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'Person') )
BEGIN

	CREATE TABLE Person
	(
		CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
		FirstName       NVARCHAR(120)   NOT NULL,
		LastName        NVARCHAR(120)   NOT NULL,
		DateOfBirth     DATETIME        NOT NULL
	);

END
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( sql )
                .ExecuteNonQuery();

			var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

			// Act
            TestDelegate action = () => Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.GenerateInsertForSqlServer( customer )
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
IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'Customer' ) )
BEGIN

	DROP TABLE Customer

END

IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'Customer') )
BEGIN

	CREATE TABLE Customer
	(
		CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
		FirstName       NVARCHAR(120)   NOT NULL,
		LastName        NVARCHAR(120)   NOT NULL,
		DateOfBirth     DATETIME        NOT NULL
	);

END
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( createSchemaSql )
                .ExecuteNonQuery();

			var newCustomer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

			// Act
            var customerId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.GenerateInsertForSqlServer( newCustomer, "[Customer]" )
                .ExecuteScalar<int>();

			const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer;
";

            var customer = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
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
IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'Customer' ) )
BEGIN

	DROP TABLE Customer

END

IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'Customer') )
BEGIN

	CREATE TABLE Customer
	(
		CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
		FirstName       NVARCHAR(120)   NOT NULL,
		LastName        NVARCHAR(120)   NOT NULL,
		DateOfBirth     DATETIME        NOT NULL
	);

END
";
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( createSchemaSql )
                .ExecuteNonQuery();

			dynamic newCustomer = new ExpandoObject();
			newCustomer.FirstName = "Clark";
			newCustomer.LastName = "Kent";
			newCustomer.DateOfBirth = DateTime.Parse( "06/18/1938" );

			// Act
		    var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString );
			databaseCommand = DatabaseCommandExtensions.GenerateInsertForSqlServer( databaseCommand, newCustomer, "[Customer]" );
			var customerId = databaseCommand
                .ExecuteScalar<int>();

			const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer;
";

			var customer = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
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