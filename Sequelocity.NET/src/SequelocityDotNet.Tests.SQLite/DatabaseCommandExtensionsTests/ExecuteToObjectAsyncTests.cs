using System.Data;
using NUnit.Framework;
using System.Threading.Tasks;

namespace SequelocityDotNet.Tests.SQLite.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ExecuteToObjectAsyncTests
    {
        public class SuperHero
        {
            public long SuperHeroId;
            public string SuperHeroName;
        }

        [Test]
        public void Should_Return_A_Task_Resulting_In_Type_Of_T()
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

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalar */
        SuperHeroName
FROM    SuperHero;
";

            // Act
            var superHeroTask = Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( sql )
                .ExecuteToObjectAsync<SuperHero>();

            // Assert
            Assert.IsInstanceOf<Task<SuperHero>>(superHeroTask);
            Assert.NotNull(superHeroTask.Result);
            Assert.That(superHeroTask.Result.SuperHeroId == 1);
            Assert.That(superHeroTask.Result.SuperHeroName == "Superman");
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

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalar */
        SuperHeroName
FROM    SuperHero;
";
            var databaseCommand = Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( sql );

            // Act
            databaseCommand.ExecuteToObjectAsync<SuperHero>()
                .Wait(); // Block until the task completes.

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

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalar */
        SuperHeroName
FROM    SuperHero;
";
            var databaseCommand = Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( sql );

            // Act
            databaseCommand.ExecuteToObjectAsync<SuperHero>( true )
                .Wait(); // Block until the task completes.

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
                .SetCommandText( "SELECT 1 as SuperHeroId, 'Superman' as SuperHeroName" )
                .ExecuteToObjectAsync<SuperHero>()
                .Wait(); // Block until the task completes.

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
                .SetCommandText( "SELECT 1 as SuperHeroId, 'Superman' as SuperHeroName" )
                .ExecuteToObjectAsync<SuperHero>()
                .Wait(); // Block until the task completes.

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
            TestDelegate action = async () => await Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( "asdf;lkj" )
                .ExecuteToObjectAsync<SuperHero>();

            // Assert
            Assert.Throws<System.Data.SQLite.SQLiteException>( action );
            Assert.IsTrue( wasUnhandledExceptionEventHandlerCalled );
        }
    }
}