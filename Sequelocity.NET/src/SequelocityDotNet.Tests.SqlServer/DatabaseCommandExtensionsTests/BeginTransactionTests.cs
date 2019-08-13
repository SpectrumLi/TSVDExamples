using System.Transactions;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SqlServer.DatabaseCommandExtensionsTests
{
	[TestFixture]
	public class BeginTransactionTests
	{
		[Test]
		public void Should_Return_A_New_DbTransaction()
		{
			// Arrange
            var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString );

			// Act
			var transaction = databaseCommand.BeginTransaction();
			
			// Assert
			Assert.NotNull( transaction );
			Assert.That( databaseCommand.DbCommand.Connection == transaction.Connection );
		}

		[Test]
		public void Should_Associate_The_DbTransaction_With_The_DatabaseCommand()
		{
			// Arrange
			var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString );

			// Act
			var transaction = databaseCommand.BeginTransaction();

			// Assert
			Assert.NotNull( databaseCommand.DbCommand.Transaction == transaction );
		}

		[Test]
		public void Can_Rollback_Transaction()
		{
			const string createTableSchema = @"
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
		        .SetCommandText( createTableSchema )
		        .ExecuteNonQuery();

            var rowCount = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                .ExecuteScalar<int>();

            Assert.That( rowCount == 0 );

			const string sqlCommand1 = @"
INSERT INTO Customer VALUES ( 'Clark', 'Kent', '06/18/1938' );
INSERT INTO Customer VALUES ( 'Bruce', 'Wayne', '05/27/1939' );
";

			const string sqlCommand2 = @"
INSERT INTO Customer VALUES ( 'Peter', 'Parker', '08/18/1962' );
";

			using( var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString ) )
			{
				using( var transaction = databaseCommand.BeginTransaction() )
				{
					var rowsUpdated = databaseCommand
						.SetCommandText( sqlCommand1 )
						.ExecuteNonQuery( keepConnectionOpen: true );

					var nextRowsUpdated = databaseCommand
						.SetCommandText( sqlCommand2 )
						.ExecuteNonQuery( keepConnectionOpen: true );

					Assert.That( rowsUpdated == 2 && nextRowsUpdated == 1 );

                    rowCount = databaseCommand
                        .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                        .ExecuteScalar<int>( keepConnectionOpen: true );

                    Assert.That( rowCount == 3 );

					if( rowsUpdated == 2 && nextRowsUpdated == 1 )
						transaction.Rollback();
				}
			}

            rowCount = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                .ExecuteScalar<int>();

            Assert.That( rowCount == 0 );
		}

        [Test]
        public void Can_Commit_Transactions()
        {
            const string createTableSchema = @"
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
                .SetCommandText( createTableSchema )
                .ExecuteNonQuery();

            var rowCount = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                .ExecuteScalar<int>();

            Assert.That( rowCount == 0 );

            const string sqlCommand1 = @"
INSERT INTO Customer VALUES ( 'Clark', 'Kent', '06/18/1938' );
INSERT INTO Customer VALUES ( 'Bruce', 'Wayne', '05/27/1939' );
";

            const string sqlCommand2 = @"
INSERT INTO Customer VALUES ( 'Peter', 'Parker', '08/18/1962' );
";

            using( var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString ) )
            {
                using( var transaction = databaseCommand.BeginTransaction() )
                {
                    var rowsUpdated = databaseCommand
                        .SetCommandText( sqlCommand1 )
                        .ExecuteNonQuery( keepConnectionOpen: true );

                    var nextRowsUpdated = databaseCommand
                        .SetCommandText( sqlCommand2 )
                        .ExecuteNonQuery( keepConnectionOpen: true );

                    Assert.That( rowsUpdated == 2 && nextRowsUpdated == 1 );

                    rowCount = databaseCommand
                        .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                        .ExecuteScalar<int>( keepConnectionOpen: true );

                    Assert.That( rowCount == 3 );

                    if( rowsUpdated == 2 && nextRowsUpdated == 1 )
                        transaction.Commit();
                }
            }

            rowCount = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                .ExecuteScalar<int>();

            Assert.That( rowCount == 3 );
        }

        [Test]
        public void Can_Rollback_Transaction_Using_TransactionScope()
        {
            const string createTableSchema = @"
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
                .SetCommandText( createTableSchema )
                .ExecuteNonQuery();

            var rowCount = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                .ExecuteScalar<int>();

            Assert.That( rowCount == 0 );

            const string sqlCommand1 = @"
INSERT INTO Customer VALUES ( 'Clark', 'Kent', '06/18/1938' );
INSERT INTO Customer VALUES ( 'Bruce', 'Wayne', '05/27/1939' );
";

            const string sqlCommand2 = @"
INSERT INTO Customer VALUES ( 'Peter', 'Parker', '08/18/1962' );
";

            using( var transaction = new TransactionScope() )
            {
                var rowsUpdated = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                        .SetCommandText( sqlCommand1 )
                        .ExecuteNonQuery();

                var nextRowsUpdated = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                    .SetCommandText( sqlCommand2 )
                    .ExecuteNonQuery();

                Assert.That( rowsUpdated == 2 && nextRowsUpdated == 1 );

                rowCount = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                        .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                        .ExecuteScalar<int>();

                Assert.That( rowCount == 3 );

                if ( rowsUpdated == 2 && nextRowsUpdated == 1 )
                    transaction.Dispose();
            }

            rowCount = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                .ExecuteScalar<int>();

            Assert.That( rowCount == 0 );
        }

        [Test]
        public void Can_Commit_Transactions_Using_TransactionScope()
        {
            const string createTableSchema = @"
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
                .SetCommandText( createTableSchema )
                .ExecuteNonQuery();

            var rowCount = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                .ExecuteScalar<int>();

            Assert.That( rowCount == 0 );

            const string sqlCommand1 = @"
INSERT INTO Customer VALUES ( 'Clark', 'Kent', '06/18/1938' );
INSERT INTO Customer VALUES ( 'Bruce', 'Wayne', '05/27/1939' );
";

            const string sqlCommand2 = @"
INSERT INTO Customer VALUES ( 'Peter', 'Parker', '08/18/1962' );
";

            using( var transaction = new TransactionScope() )
            {
                var rowsUpdated = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                        .SetCommandText( sqlCommand1 )
                        .ExecuteNonQuery();

                var nextRowsUpdated = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                    .SetCommandText( sqlCommand2 )
                    .ExecuteNonQuery();

                Assert.That( rowsUpdated == 2 && nextRowsUpdated == 1 );

                rowCount = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                        .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                        .ExecuteScalar<int>();

                Assert.That( rowCount == 3 );

                if( rowsUpdated == 2 && nextRowsUpdated == 1 )
                    transaction.Complete();
            }

            rowCount = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( "SELECT COUNT(*) FROM Customer" )
                .ExecuteScalar<int>();

            Assert.That( rowCount == 3 );
        }
	}
}