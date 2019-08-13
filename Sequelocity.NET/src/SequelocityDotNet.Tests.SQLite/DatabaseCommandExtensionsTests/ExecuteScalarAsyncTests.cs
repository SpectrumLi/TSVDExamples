using System.Data;
using NUnit.Framework;
using System.Threading.Tasks;

namespace SequelocityDotNet.Tests.SQLite.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ExecuteScalarAsyncTests
    {
        [Test]
        public void Should_Return_A_Task_Resulting_In_The_First_Column_Of_The_First_Row_In_The_Result_Set()
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

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalarAsync */
        SuperHeroName
FROM    SuperHero;
";

            // Act
            var superHeroIdTask = Sequelocity.GetDatabaseCommandForSQLite(ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString)
                .SetCommandText(sql)
                .ExecuteScalarAsync();

            // Assert
            Assert.IsInstanceOf<Task<object>>(superHeroIdTask);
            Assert.That( superHeroIdTask.Result.ToLong() == 1 );
        }

        [Test]
        [Description("This tests the generic version of the ExecuteScalerAsync method.")]
        public void Should_Return_A_Task_Resulting_In_The_First_Column_Of_The_First_Row_In_The_Result_Set_And_Convert_It_To_The_Type_Specified()
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

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalarAsync */
        SuperHeroName
FROM    SuperHero;
";

            // Act
            var superHeroIdTask = Sequelocity.GetDatabaseCommand(ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString)
                .SetCommandText(sql)
                .ExecuteScalarAsync<long>(); // Generic version of the ExecuteScalarAsync method

            // Assert
            Assert.IsInstanceOf<Task<long>>(superHeroIdTask);
            Assert.That(superHeroIdTask.Result == 1);
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

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalarAsync */
        SuperHeroName
FROM    SuperHero;
";
            var databaseCommand = Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( sql );

            // Act
            databaseCommand.ExecuteScalarAsync()
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

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalarAsync */
        SuperHeroName
FROM    SuperHero;
";
            var databaseCommand = Sequelocity.GetDatabaseCommandForSQLite( ConnectionStringsNames.SqliteInMemoryDatabaseConnectionString )
                .SetCommandText( sql );

            // Act
            databaseCommand.ExecuteScalarAsync( true )
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
                .SetCommandText( "SELECT 1" )
                .ExecuteScalarAsync()
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
                .SetCommandText( "SELECT 1" )
                .ExecuteScalarAsync()
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
                .ExecuteScalarAsync();

            // Assert
            Assert.Throws<System.Data.SQLite.SQLiteException>( action );
            Assert.IsTrue( wasUnhandledExceptionEventHandlerCalled );
        }
    }
}