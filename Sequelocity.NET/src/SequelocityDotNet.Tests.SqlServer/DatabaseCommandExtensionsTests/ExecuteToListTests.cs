using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SqlServer.DatabaseCommandExtensionsTests
{
	[TestFixture]
	public class ExecuteToListTests
	{
		public class SuperHero
		{
			public long SuperHeroId;
			public string SuperHeroName;
		}

		[Test]
		public void Should_Map_The_Results_Back_To_A_List_Of_Type_T()
		{
			// Arrange
			const string sql = @"
CREATE TABLE #SuperHero
(
	SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
	SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
		SuperHeroName
FROM    #SuperHero;
";

			// Act
			var superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.SetCommandText( sql )
				.ExecuteToList<SuperHero>();

			// Assert
			Assert.That( superHeroes.Count == 2 );
			Assert.That( superHeroes[0].SuperHeroId == 1 );
			Assert.That( superHeroes[0].SuperHeroName == "Superman" );
			Assert.That( superHeroes[1].SuperHeroId == 2 );
			Assert.That( superHeroes[1].SuperHeroName == "Batman" );
		}

		[Test]
		public void Should_Null_The_DbCommand_By_Default()
		{
			// Arrange
			const string sql = @"
CREATE TABLE #SuperHero
(
	SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
	SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
		SuperHeroName
FROM    #SuperHero;
";
			var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.SetCommandText( sql );

			// Act
			var superHeroes = databaseCommand.ExecuteToList<SuperHero>();

			// Assert
			Assert.IsNull( databaseCommand.DbCommand );
		}

		[Test]
		public void Should_Keep_The_Database_Connection_Open_If_keepConnectionOpen_Parameter_Was_True()
		{
			// Arrange
			const string sql = @"
CREATE TABLE #SuperHero
(
	SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
	SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
		SuperHeroName
FROM    #SuperHero;
";
			var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.SetCommandText( sql );

			// Act
			var superHeroes = databaseCommand.ExecuteToList<SuperHero>( true );

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
			var superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.SetCommandText( "SELECT 1 as SuperHeroId, 'Superman' as SuperHeroName" )
				.ExecuteToList<SuperHero>();

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
			var superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.SetCommandText( "SELECT 1 as SuperHeroId, 'Superman' as SuperHeroName" )
				.ExecuteToList<SuperHero>();

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
			TestDelegate action = () => Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
				.SetCommandText( "asdf;lkj" )
				.ExecuteToList<SuperHero>();

			// Assert
			Assert.Throws<System.Data.SqlClient.SqlException>( action );
			Assert.IsTrue( wasUnhandledExceptionEventHandlerCalled );
		}
	}
}