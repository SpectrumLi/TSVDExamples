using System.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SequelocityDotNet.Tests.MySql.DatabaseCommandExtensionsTests
{
	[TestFixture]
	public class ExecuteToListAsyncTests
    {
		public class SuperHero
		{
			public long SuperHeroId;
			public string SuperHeroName;
		}

		[Test]
		public void Should_Return_A_Task_Resulting_In_A_Map_Of_The_Results_To_A_List_Of_Type_T()
		{
			// Arrange
            const string sql = @"
DROP TEMPORARY TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     INT             NOT NULL    AUTO_INCREMENT,
    SuperHeroName	VARCHAR(120)    NOT NULL,
    PRIMARY KEY ( SuperHeroId )
);

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    SuperHero;
";

			// Act
			var superHeroesTask = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.SetCommandText( sql )
				.ExecuteToListAsync<SuperHero>();

            // Assert
            Assert.IsInstanceOf<Task<List<SuperHero>>>(superHeroesTask);
            Assert.That(superHeroesTask.Result.Count == 2);
            Assert.That(superHeroesTask.Result[0].SuperHeroId == 1);
            Assert.That(superHeroesTask.Result[0].SuperHeroName == "Superman");
            Assert.That(superHeroesTask.Result[1].SuperHeroId == 2);
            Assert.That(superHeroesTask.Result[1].SuperHeroName == "Batman");
        }

        [Test]
		public void Should_Null_The_DbCommand_By_Default()
		{
			// Arrange
            const string sql = @"
DROP TEMPORARY TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     INT             NOT NULL    AUTO_INCREMENT,
    SuperHeroName	VARCHAR(120)    NOT NULL,
    PRIMARY KEY ( SuperHeroId )
);

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    SuperHero;
";
			var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.SetCommandText( sql );

			// Act
			databaseCommand.ExecuteToListAsync<SuperHero>()
                .Wait(); // Block until the task completes.

            // Assert
            Assert.IsNull( databaseCommand.DbCommand );
		}

		[Test]
		public void Should_Keep_The_Database_Connection_Open_If_keepConnectionOpen_Parameter_Was_True()
		{
			// Arrange
            const string sql = @"
DROP TEMPORARY TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     INT             NOT NULL    AUTO_INCREMENT,
    SuperHeroName	VARCHAR(120)    NOT NULL,
    PRIMARY KEY ( SuperHeroId )
);

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    SuperHero;
";
			var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.SetCommandText( sql );

			// Act
			databaseCommand.ExecuteToListAsync<SuperHero>( true )
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
			Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.SetCommandText( "SELECT 1 as SuperHeroId, 'Superman' as SuperHeroName" )
				.ExecuteToListAsync<SuperHero>()
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
			Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.SetCommandText( "SELECT 1 as SuperHeroId, 'Superman' as SuperHeroName" )
				.ExecuteToListAsync<SuperHero>()
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
			TestDelegate action = async () => await Sequelocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
				.SetCommandText( "asdf;lkj" )
				.ExecuteToListAsync<SuperHero>();

			// Assert
            Assert.Throws<global::MySql.Data.MySqlClient.MySqlException>( action );
			Assert.IsTrue( wasUnhandledExceptionEventHandlerCalled );
		}
	}
}