using System.Data;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SequelocityDotNet.Tests.MySql.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ExecuteToMapAsyncTests
    {
        public class SuperHero
        {
            public long SuperHeroId;
            public string SuperHeroName;
        }

        [Test]
        public void Should_Call_The_DataRecordCall_Action_For_Each_Record_In_The_Result_Set()
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
                .ExecuteToMapAsync( record =>
                {
                    var obj = new SuperHero
                    {
                        SuperHeroId = record.GetValue( 0 ).ToLong(),
                        SuperHeroName = record.GetValue( 1 ).ToString()
                    };

                    return obj;
                } );

            // Assert
            Assert.IsInstanceOf<Task<List<SuperHero>>>(superHeroesTask);
            Assert.That(superHeroesTask.Result.Count == 2);
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
            databaseCommand.ExecuteToMapAsync( record =>
            {
                var obj = new SuperHero
                {
                    SuperHeroId = record.GetValue( 0 ).ToLong(),
                    SuperHeroName = record.GetValue( 1 ).ToString()
                };

                return obj;
            } )
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
            databaseCommand.ExecuteToMapAsync( record =>
            {
                var obj = new SuperHero
                {
                    SuperHeroId = record.GetValue( 0 ).ToLong(),
                    SuperHeroName = record.GetValue( 1 ).ToString()
                };

                return obj;
            }, true )
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
                .ExecuteToMapAsync( record =>
                {
                    var obj = new SuperHero
                    {
                        SuperHeroId = record.GetValue( 0 ).ToLong(),
                        SuperHeroName = record.GetValue( 1 ).ToString()
                    };

                    return obj;
                } )
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
                .ExecuteToMapAsync( record =>
                {
                    var obj = new SuperHero
                    {
                        SuperHeroId = record.GetValue( 0 ).ToLong(),
                        SuperHeroName = record.GetValue( 1 ).ToString()
                    };

                    return obj;
                } )
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
                .ExecuteToMapAsync( record =>
                {
                    var obj = new SuperHero
                    {
                        SuperHeroId = record.GetValue( 0 ).ToLong(),
                        SuperHeroName = record.GetValue( 1 ).ToString()
                    };

                    return obj;
                } );

            // Assert
            Assert.Throws<global::MySql.Data.MySqlClient.MySqlException>( action );
            Assert.IsTrue( wasUnhandledExceptionEventHandlerCalled );
        }
    }
}