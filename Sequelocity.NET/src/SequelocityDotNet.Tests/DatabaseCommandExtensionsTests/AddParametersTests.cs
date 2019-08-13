using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class AddParametersTests
    {
        [Test]
        public void Should_Handle_Adding_A_Parameter_Array_Of_DbParameters_To_The_Parameters_Collection_Of_The_DbCommand()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            var superHeroNameParameter = databaseCommand.CreateParameter( "@SuperHeroName", "Superman" );
            var alterEgoFirstNameParameter = databaseCommand.CreateParameter( "@AlterEgoFirstName", "Clark" );
            var alterEgoLastNameParameter = databaseCommand.CreateParameter( "@AlterEgoLastName", "Kent" );

            // Act
            databaseCommand = databaseCommand.AddParameters( superHeroNameParameter, alterEgoFirstNameParameter, alterEgoLastNameParameter );

            // Assert
            Assert.That( databaseCommand.DbCommand.Parameters[superHeroNameParameter.ParameterName].Value == superHeroNameParameter.Value );
            Assert.That( databaseCommand.DbCommand.Parameters[alterEgoFirstNameParameter.ParameterName].Value == alterEgoFirstNameParameter.Value );
            Assert.That( databaseCommand.DbCommand.Parameters[alterEgoLastNameParameter.ParameterName].Value == alterEgoLastNameParameter.Value );
        }
        [Test]
        public void Should_Handle_Adding_A_Dictionary_Of_Parameters_To_The_Parameters_Collection_Of_The_DbCommand()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            var superHeroName = new KeyValuePair<string, object>( "@SuperHeroName", "Superman" );
            var alterEgoFirstName = new KeyValuePair<string, object>( "@AlterEgoFirstName", "Clark" );
            var alterEgoLastName = new KeyValuePair<string, object>( "@AlterEgoLastName", "Kent" );

            IDictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add( superHeroName );
            dictionary.Add( alterEgoFirstName );
            dictionary.Add( alterEgoLastName );

            // Act
            databaseCommand = databaseCommand.AddParameters( dictionary );

            // Assert
            Assert.That( databaseCommand.DbCommand.Parameters[superHeroName.Key].Value == superHeroName.Value );
            Assert.That( databaseCommand.DbCommand.Parameters[alterEgoFirstName.Key].Value == alterEgoFirstName.Value );
            Assert.That( databaseCommand.DbCommand.Parameters[alterEgoLastName.Key].Value == alterEgoLastName.Value );
        }

        [Test]
        public void Should_Handle_Adding_A_List_Of_Parameters_To_The_Parameters_Collection_Of_The_DbCommand()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand()
                .SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName IN ( @SuperHeroNames )" );

            const string parameterName = "@SuperHeroNames";

            const string superman = "Superman";
            const string batman = "Batman";
            const string spiderman = "Spider-Man";

            var parameterList = new List<string> { superman, batman, spiderman };

            // Act
            databaseCommand = databaseCommand.AddParameters( parameterName, parameterList );

            // Assert
            Assert.That( databaseCommand.DbCommand.Parameters[parameterName + "_p0"].Value.ToString() == superman );
            Assert.That( databaseCommand.DbCommand.Parameters[parameterName + "_p1"].Value.ToString() == batman );
            Assert.That( databaseCommand.DbCommand.Parameters[parameterName + "_p2"].Value.ToString() == spiderman );

            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "SELECT * FROM SuperHero WHERE SuperHeroName IN ( @SuperHeroNames_p0,@SuperHeroNames_p1,@SuperHeroNames_p2 )" ) );
        }

        [Test]
        public void Should_Handle_Adding_A_List_Of_Parameters_With_A_Specified_DbType_To_The_Parameters_Collection_Of_The_DbCommand()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand()
                .SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName IN ( @SuperHeroNames )" );

            const string parameterName = "@SuperHeroNames";

            const string superman = "Superman";
            const string batman = "Batman";
            const string spiderman = "Spider-Man";

            var parameterList = new List<string> { superman, batman, spiderman };

            // Act
            databaseCommand = databaseCommand.AddParameters( parameterName, parameterList, DbType.AnsiString );

            // Assert

            Assert.That( databaseCommand.DbCommand.Parameters[0].ParameterName.Contains( parameterName ) );
            Assert.That( databaseCommand.DbCommand.Parameters[0].Value.ToString() == superman );
            Assert.That( databaseCommand.DbCommand.Parameters[0].DbType == DbType.AnsiString );

            Assert.That( databaseCommand.DbCommand.Parameters[1].ParameterName.Contains( parameterName ) );
            Assert.That( databaseCommand.DbCommand.Parameters[1].Value.ToString() == batman );
            Assert.That( databaseCommand.DbCommand.Parameters[1].DbType == DbType.AnsiString );

            Assert.That( databaseCommand.DbCommand.Parameters[2].ParameterName.Contains( parameterName ) );
            Assert.That( databaseCommand.DbCommand.Parameters[2].Value.ToString() == spiderman );
            Assert.That( databaseCommand.DbCommand.Parameters[2].DbType == DbType.AnsiString );

            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "SELECT * FROM SuperHero WHERE SuperHeroName IN ( @SuperHeroNames_p0,@SuperHeroNames_p1,@SuperHeroNames_p2 )" ) );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_When_Adding_A_List_Of_Parameters_With_A_Null_Parameter_Name()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand()
                .SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName IN ( @SuperHeroNames )" );

            const string parameterName = null;

            const string superman = "Superman";
            const string batman = "Batman";
            const string spiderman = "Spider-Man";

            var parameterList = new List<string> { superman, batman, spiderman };

            // Act
            TestDelegate action = () => databaseCommand.AddParameters( parameterName, parameterList );

            // Assert
            Assert.Throws<ArgumentNullException>( action );
        }

        [Test]
        public void Should_Throw_An_ArgumentNullException_When_Adding_A_Null_List_Of_Parameters()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand()
                .SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName IN ( @SuperHeroNames )" );

            const string parameterName = "@SuperHeroNames";

            List<string> parameterList = null;

            // Act
            TestDelegate action = () => databaseCommand.AddParameters( parameterName, parameterList );

            // Assert
            Assert.Throws<ArgumentNullException>( action );
        }

        [Test]
        public void Should_Throw_An_Exception_When_Adding_An_Empty_List_Of_Parameters()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand()
                .SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName IN ( @SuperHeroNames )" );

            const string parameterName = "@SuperHeroNames";

            var parameterList = new List<string>();

            // Act
            TestDelegate action = () => databaseCommand.AddParameters( parameterName, parameterList );

            // Assert
            Assert.Throws<Exception>( action );
        }

        [Test]
        public void Should_Throw_An_Exception_When_The_CommandText_Has_Not_Been_Set()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand();

            const string parameterName = "@SuperHeroNames";

            const string superman = "Superman";
            const string batman = "Batman";
            const string spiderman = "Spider-Man";

            var parameterList = new List<string> { superman, batman, spiderman };

            // Act
            TestDelegate action = () => databaseCommand.AddParameters( parameterName, parameterList );

            // Assert
            var exception = Assert.Catch<Exception>( action );
            Assert.That( exception.Message.Contains( "The CommandText must already be set before calling this method." ) );
        }

        [Test]
        public void Should_Throw_An_Exception_When_The_CommandText_Does_Not_Contain_The_Parameter_Name()
        {
            // Arrange
            var databaseCommand = TestHelpers.GetDatabaseCommand()
                .SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName" );

            databaseCommand.SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName" );

            const string parameterName = "@SuperHeroNames";

            const string superman = "Superman";
            const string batman = "Batman";
            const string spiderman = "Spider-Man";

            var parameterList = new List<string> { superman, batman, spiderman };

            // Act
            TestDelegate action = () => databaseCommand.AddParameters( parameterName, parameterList );

            // Assert
            var exception = Assert.Catch<Exception>( action );
            Assert.That( exception.Message.Contains( string.Format( "The CommandText does not contain the parameter name '{0}'", parameterName ) ) );
        }
    }
}