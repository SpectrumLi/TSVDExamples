using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SqlServer.DatabaseCommandExtensionsTests
{
	[TestFixture]
	public class GenerateInsertsForSqlServerTests
	{
		public struct Customer
		{
			public int? CustomerId;
			public string FirstName;
			public string LastName;
			public DateTime DateOfBirth;
		}

		[Test]
		public void Should_Return_The_Last_Inserted_Ids()
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

			var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqlServerConnectionString );

			new DatabaseCommand( dbConnection )
				.SetCommandText( sql )
				.ExecuteNonQuery( true );

			var customer1 = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
			var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
			var customer3 = new Customer { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
			var list = new List<Customer> { customer1, customer2, customer3 };
			
			// Act
			var customerIds = new DatabaseCommand( dbConnection )
				.GenerateInsertsForSqlServer( list )
				.ExecuteToList<long>();

			// Assert
			Assert.That( customerIds.Count == 3 );
			Assert.That( customerIds[0] == 1 );
			Assert.That( customerIds[1] == 2 );
			Assert.That( customerIds[2] == 3 );
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

			var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqlServerConnectionString );

			new DatabaseCommand( dbConnection )
				.SetCommandText( createSchemaSql )
				.ExecuteNonQuery( true );

			var customer1 = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
			var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
			var customer3 = new Customer { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
			var list = new List<Customer> { customer1, customer2, customer3 };

			// Act
			var customerIds = new DatabaseCommand( dbConnection )
				.GenerateInsertsForSqlServer( list )
				.ExecuteToList<long>( true );

			const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer
WHERE   CustomerId IN ( @CustomerIds );
";

			var customers = new DatabaseCommand( dbConnection )
				.SetCommandText( selectCustomerQuery )
				.AddParameters( "@CustomerIds", customerIds, DbType.Int64 )
				.ExecuteToList<Customer>()
				.OrderBy( x => x.CustomerId )
				.ToList();

			// Assert
			Assert.That( customers.Count == 3 );

			Assert.That( customers[0].CustomerId == 1 );
			Assert.That( customers[0].FirstName == customer1.FirstName );
			Assert.That( customers[0].LastName == customer1.LastName );
			Assert.That( customers[0].DateOfBirth == customer1.DateOfBirth );

			Assert.That( customers[1].CustomerId == 2 );
			Assert.That( customers[1].FirstName == customer2.FirstName );
			Assert.That( customers[1].LastName == customer2.LastName );
			Assert.That( customers[1].DateOfBirth == customer2.DateOfBirth );

			Assert.That( customers[2].CustomerId == 3 );
			Assert.That( customers[2].FirstName == customer3.FirstName );
			Assert.That( customers[2].LastName == customer3.LastName );
			Assert.That( customers[2].DateOfBirth == customer3.DateOfBirth );
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

			var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqlServerConnectionString );

			new DatabaseCommand( dbConnection )
				.SetCommandText( sql )
				.ExecuteNonQuery( true );

			var customer1 = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
			var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
			var customer3 = new Customer { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
			var list = new List<Customer> { customer1, customer2, customer3 };

			// Act
			var numberOfAffectedRecords = new DatabaseCommand( dbConnection )
				.GenerateInsertsForSqlServer( list, "[Person]" ) // Specifying a table name of Person
				.ExecuteNonQuery( true );

			// Assert
			Assert.That( numberOfAffectedRecords == list.Count );
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

			var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqlServerConnectionString );

			new DatabaseCommand( dbConnection )
				.SetCommandText( sql )
				.ExecuteNonQuery( true );

			var customer1 = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
			var customer2 = new { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
			var customer3 = new { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
			var list = new List<object> { customer1, customer2, customer3 };

			// Act
			TestDelegate action = () => new DatabaseCommand( dbConnection )
                .GenerateInsertsForSqlServer( list )
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

			var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqlServerConnectionString );

			new DatabaseCommand( dbConnection )
				.SetCommandText( createSchemaSql )
				.ExecuteNonQuery( true );

			var customer1 = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
			var customer2 = new { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
			var customer3 = new { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
			var list = new List<object> { customer1, customer2, customer3 };

			// Act
			var customerIds = new DatabaseCommand( dbConnection )
				.GenerateInsertsForSqlServer( list, "Customer" )
				.ExecuteToList<long>( true );

			const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer
WHERE   CustomerId IN ( @CustomerIds );
";

			var customers = new DatabaseCommand( dbConnection )
				.SetCommandText( selectCustomerQuery )
				.AddParameters( "@CustomerIds", customerIds, DbType.Int64 )
				.ExecuteToList<Customer>()
				.OrderBy( x => x.CustomerId )
				.ToList();

			// Assert
			Assert.That( customers.Count == 3 );

			Assert.That( customers[0].CustomerId == 1 );
			Assert.That( customers[0].FirstName == customer1.FirstName );
			Assert.That( customers[0].LastName == customer1.LastName );
			Assert.That( customers[0].DateOfBirth == customer1.DateOfBirth );

			Assert.That( customers[1].CustomerId == 2 );
			Assert.That( customers[1].FirstName == customer2.FirstName );
			Assert.That( customers[1].LastName == customer2.LastName );
			Assert.That( customers[1].DateOfBirth == customer2.DateOfBirth );

			Assert.That( customers[2].CustomerId == 3 );
			Assert.That( customers[2].FirstName == customer3.FirstName );
			Assert.That( customers[2].LastName == customer3.LastName );
			Assert.That( customers[2].DateOfBirth == customer3.DateOfBirth );
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

			var dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqlServerConnectionString );

			new DatabaseCommand( dbConnection )
				.SetCommandText( createSchemaSql )
				.ExecuteNonQuery( true );

			dynamic customer1 = new ExpandoObject();
			customer1.FirstName = "Clark";
			customer1.LastName = "Kent";
			customer1.DateOfBirth = DateTime.Parse( "06/18/1938" );

			dynamic customer2 = new ExpandoObject();
			customer2.FirstName = "Bruce";
			customer2.LastName = "Wayne";
			customer2.DateOfBirth = DateTime.Parse( "05/27/1939" );

			dynamic customer3 = new ExpandoObject();
			customer3.FirstName = "Peter";
			customer3.LastName = "Parker";
			customer3.DateOfBirth = DateTime.Parse( "08/18/1962" );

			var list = new List<dynamic> { customer1, customer2, customer3 };

			// Act
			var customerIds = new DatabaseCommand( dbConnection )
				.GenerateInsertsForSqlServer( list, "Customer" )
				.ExecuteToList<long>( true );

			const string selectCustomerQuery = @"
SELECT  CustomerId,
		FirstName,
		LastName,
		DateOfBirth
FROM    Customer
WHERE   CustomerId IN ( @CustomerIds );
";

			var customers = new DatabaseCommand( dbConnection )
				.SetCommandText( selectCustomerQuery )
				.AddParameters( "@CustomerIds", customerIds, DbType.Int64 )
				.ExecuteToList<Customer>()
				.OrderBy( x => x.CustomerId )
				.ToList();

			// Assert
			Assert.That( customers.Count == 3 );

			Assert.That( customers[0].CustomerId == 1 );
			Assert.That( customers[0].FirstName == customer1.FirstName );
			Assert.That( customers[0].LastName == customer1.LastName );
			Assert.That( customers[0].DateOfBirth == customer1.DateOfBirth );

			Assert.That( customers[1].CustomerId == 2 );
			Assert.That( customers[1].FirstName == customer2.FirstName );
			Assert.That( customers[1].LastName == customer2.LastName );
			Assert.That( customers[1].DateOfBirth == customer2.DateOfBirth );

			Assert.That( customers[2].CustomerId == 3 );
			Assert.That( customers[2].FirstName == customer3.FirstName );
			Assert.That( customers[2].LastName == customer3.LastName );
			Assert.That( customers[2].DateOfBirth == customer3.DateOfBirth );
		}
	}
}