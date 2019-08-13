using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SQLite.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ExecuteToDataTableTests
    {
        [Test]
        public void Should_Return_A_DataSet()
        {
            // Arrange
            const string sql = @"
CREATE TABLE IF NOT EXISTS SuperHero
(
    SuperHeroId     INTEGER         NOT NULL    PRIMARY KEY     AUTOINCREMENT,
    SuperHeroName	NVARCHAR(120)   NOT NULL,
    UNIQUE(SuperHeroName)
);

INSERT OR IGNORE INTO SuperHero VALUES ( NULL, 'Superman' );
INSERT OR IGNORE INTO SuperHero VALUES ( NULL, 'Batman' );

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalar */
        SuperHeroName
FROM    SuperHero;
";

            // Act
            var dataTable = Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( sql )
                .ExecuteToDataTable();

            // Assert
            Assert.That( dataTable.Rows.Count == 2 );
            Assert.That( dataTable.Rows[0][0].ToString() == "1" );
            Assert.That( dataTable.Rows[0][1].ToString() == "Superman" );
            Assert.That( dataTable.Rows[1][0].ToString() == "2" );
            Assert.That( dataTable.Rows[1][1].ToString() == "Batman" );
        }
        
        [Test]
        public void Should_Null_The_DbCommand_By_Default()
        {
            // Arrange
            const string sql = @"
CREATE TABLE IF NOT EXISTS SuperHero
(
    SuperHeroId     INTEGER         NOT NULL    PRIMARY KEY     AUTOINCREMENT,
    SuperHeroName	NVARCHAR(120)   NOT NULL,
    UNIQUE(SuperHeroName)
);

INSERT OR IGNORE INTO SuperHero VALUES ( NULL, 'Superman' );
INSERT OR IGNORE INTO SuperHero VALUES ( NULL, 'Batman' );

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalar */
        SuperHeroName
FROM    SuperHero;
";
            var databaseCommand = Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( sql );

            // Act
            databaseCommand.ExecuteToDataTable();

            // Assert
            Assert.IsNull( databaseCommand.DbCommand );
        }

        [Test]
        public void Should_Keep_The_Database_Connection_Open_If_keepConnectionOpen_Parameter_Was_True()
        {
            // Arrange
            const string sql = @"
CREATE TABLE IF NOT EXISTS SuperHero
(
    SuperHeroId     INTEGER         NOT NULL    PRIMARY KEY     AUTOINCREMENT,
    SuperHeroName	NVARCHAR(120)   NOT NULL,
    UNIQUE(SuperHeroName)
);

INSERT OR IGNORE INTO SuperHero VALUES ( NULL, 'Superman' );
INSERT OR IGNORE INTO SuperHero VALUES ( NULL, 'Batman' );

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalar */
        SuperHeroName
FROM    SuperHero;
";
            var databaseCommand = Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( sql );

            // Act
            databaseCommand.ExecuteToDataTable( true );

            // Assert
            Assert.That( databaseCommand.DbCommand.Connection.State == ConnectionState.Open );

            // Cleanup
            databaseCommand.Dispose();
        }

        [Test]
        public void Should_Call_The_DatabaseCommandPreExecuteEventHandler()
        {
            // Arrange
            bool wasPreExecuteEventHandlerCalled = false;

            Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandPreExecuteEventHandlers.Add( command => wasPreExecuteEventHandlerCalled = true );

            // Act
            Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( "SELECT 1" )
                .ExecuteToDataTable();

            // Assert
            Assert.IsTrue( wasPreExecuteEventHandlerCalled );
        }

        [Test]
        public void Should_Call_The_DatabaseCommandPostExecuteEventHandler()
        {
            // Arrange
            bool wasPostExecuteEventHandlerCalled = false;

            Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandPostExecuteEventHandlers.Add( command => wasPostExecuteEventHandlerCalled = true );

            // Act
            Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( "SELECT 1" )
                .ExecuteToDataTable();

            // Assert
            Assert.IsTrue( wasPostExecuteEventHandlerCalled );
        }

        [Test]
        public void Should_Call_The_DatabaseCommandUnhandledExceptionEventHandler()
        {
            // Arrange
            bool wasUnhandledExceptionEventHandlerCalled = false;

            Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandUnhandledExceptionEventHandlers.Add( ( exception, command ) =>
            {
                wasUnhandledExceptionEventHandlerCalled = true;
            } );

            // Act
            TestDelegate action = () => Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( "asdf;lkj" )
                .ExecuteToDataTable();

            // Assert
            Assert.Throws<System.Data.SQLite.SQLiteException>( action );
            Assert.IsTrue( wasUnhandledExceptionEventHandlerCalled );
        }
    }
}