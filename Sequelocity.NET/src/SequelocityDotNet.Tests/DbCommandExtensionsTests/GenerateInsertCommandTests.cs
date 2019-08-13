using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class GenerateInsertCommandTests
    {
        public class CustomerWithFields
        {
            public int? CustomerId;
            public string FirstName;
            public string LastName;
            public DateTime DateOfBirth;
        }

        [Test]
        public void Should_Generate_An_Insert_Statement_When_Passed_An_Instance_Of_An_Class_With_Fields()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.NotNull( dbCommand.CommandText );
            Assert.That( dbCommand.CommandText.Contains( "INSERT" ) );
        }

        public class CustomerWithProperties
        {
            public int? CustomerId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
        }

        [Test]
        public void Should_Generate_An_Insert_Statement_When_Passed_An_Instance_Of_An_Class_With_Properties()
        {
            // Arrange
            var customer = new CustomerWithProperties { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.NotNull( dbCommand.CommandText );
            Assert.That( dbCommand.CommandText.Contains( "INSERT" ) );
        }

        [Test]
        public void Should_Add_The_Type_Name_When_No_Table_Name_Is_Supplied()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "CustomerWithFields" ) );
        }

        [Test]
        public void Should_Use_The_Supplied_Table_Name_In_The_Insert_Statement()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});", "[Person]" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "[Person]" ) );
        }

        [Test]
        public void Should_Add_Parameters_To_The_DbCommand_And_To_The_Insert_Statement()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});", "[Person]" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            var parameters = dbCommand.Parameters.Cast<DbParameter>().ToList();
            
            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@FirstName" ) ).Value.ToString() == customer.FirstName );
            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@LastName" ) ).Value.ToString() == customer.LastName );
            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@DateOfBirth" ) ).Value.ToString() == customer.DateOfBirth.ToString() );

            Assert.That( dbCommand.CommandText.Contains( "@FirstName" ) );
            Assert.That( dbCommand.CommandText.Contains( "@LastName" ) );
            Assert.That( dbCommand.CommandText.Contains( "@DateOfBirth" ) );
        }

        [Test]
        public void Should_Throw_An_Exception_When_Passing_An_Anonymous_Object_And_Not_Specifying_A_TableName()
        {
            // Arrange
            var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            TestDelegate action = () => dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});" );

            // Assert
            var exception = Assert.Catch<ArgumentNullException>( action );
            Trace.WriteLine( exception.Message );
            Assert.That( exception.Message.Contains( "The 'tableName' parameter must be provided when the object supplied is an anonymous type." ) );
        }

        [Test]
        public void Should_Throw_An_Exception_When_Passing_A_Null_Object()
        {
            // Arrange
            CustomerWithFields customer = null;

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            TestDelegate action = () => dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});" );

            // Assert
            var exception = Assert.Catch<ArgumentNullException>( action );
            Trace.WriteLine( exception.Message );
            Assert.That( exception.Message.Contains( "Value cannot be null.\r\nParameter name: obj" ) );
        }

        [Test]
        public void Should_Throw_An_Exception_When_Passing_A_Null_SqlInsertStatementTemplate()
        {
            // Arrange
            var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            TestDelegate action = () => dbCommand.GenerateInsertCommand( customer, null, "[Customer]" );

            // Assert
            var exception = Assert.Catch<ArgumentNullException>( action );
            Trace.WriteLine( exception.Message );
            Assert.That( exception.Message.Contains( "Value cannot be null.\r\nParameter name: sqlInsertStatementTemplate" ) );
        }

        [Test]
        public void Should_Throw_An_Exception_When_Passing_An_Empty_SqlInsertStatementTemplate()
        {
            // Arrange
            var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            TestDelegate action = () => dbCommand.GenerateInsertCommand( customer, "", "[Customer]" );

            // Assert
            var exception = Assert.Catch<ArgumentNullException>( action );
            Trace.WriteLine( exception.Message );
            Assert.That( exception.Message.Contains( "The 'sqlInsertStatementTemplate' parameter must not be null, empty, or whitespace." ) );
        }

        [Test]
        public void Should_Throw_An_Exception_When_Passing_An_Invalid_SqlInsertStatementTemplate()
        {
            // Arrange
            var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            TestDelegate action = () => dbCommand.GenerateInsertCommand( customer, "An_Invalid_Template{0}", "[Customer]" );

            // Assert
            var exception = Assert.Catch<Exception>( action );
            Trace.WriteLine( exception.Message );
            Assert.That( exception.Message.Contains( "The 'sqlInsertStatementTemplate' parameter does not conform to the template requirements of containing three string.Format arguments." ) );
        }

        [Test]
        public void Should_Generate_An_Insert_Statement_When_Passed_An_Anonymous_Object()
        {
            // Arrange
            var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});", "[Person]" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.NotNull( dbCommand.CommandText );
            Assert.That( dbCommand.CommandText.Contains( "INSERT" ) );
        }

        #region Escaping Tests

        [Test]
        public void Should_Not_Escape_The_Table_Name_By_Default()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "CustomerWithFields" ) );
        }

        [Test]
        public void Can_Escape_The_Table_Name_With_Square_Brackets()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});", null, DbCommandExtensions.KeywordEscapeMethod.SquareBracket );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "[CustomerWithFields]" ) );
        }

        [Test]
        public void Can_Escape_The_Table_Name_With_Backticks()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});", null, DbCommandExtensions.KeywordEscapeMethod.Backtick );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "`CustomerWithFields`" ) );
        }

        [Test]
        public void Can_Escape_The_Table_Name_With_Double_Quotes()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});", null, DbCommandExtensions.KeywordEscapeMethod.DoubleQuote );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "\"CustomerWithFields\"" ) );
        }

        [Test]
        public void Can_Escape_The_Column_Names_With_Square_Brackets()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});", null, DbCommandExtensions.KeywordEscapeMethod.SquareBracket );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "[FirstName]" ) );
            Assert.That( dbCommand.CommandText.Contains( "[LastName]" ) );
            Assert.That( dbCommand.CommandText.Contains( "[DateOfBirth]" ) );
        }

        [Test]
        public void Can_Escape_The_Column_Names_Name_With_Backticks()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});", null, DbCommandExtensions.KeywordEscapeMethod.Backtick );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "`FirstName`" ) );
            Assert.That( dbCommand.CommandText.Contains( "`LastName`" ) );
            Assert.That( dbCommand.CommandText.Contains( "`DateOfBirth`" ) );
        }

        [Test]
        public void Can_Escape_The_Column_Names_With_Double_Quotes()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});", null, DbCommandExtensions.KeywordEscapeMethod.DoubleQuote );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "\"FirstName\"" ) );
            Assert.That( dbCommand.CommandText.Contains( "\"LastName\"" ) );
            Assert.That( dbCommand.CommandText.Contains( "\"DateOfBirth\"" ) );
        }

        #endregion Escaping Tests

        public class ClassWithNoFields
        {
        }

        [Test]
        [Description( "This is just a weird test just for fun." )]
        public void Should_Generate_An_Insert_Statement_When_Passed_A_Class_With_No_Fields()
        {
            // Arrange
            var classWithNoFields = new ClassWithNoFields();

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( classWithNoFields, "INSERT INTO {0} ({1}) VALUES({2});" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.NotNull( dbCommand.CommandText );
            Assert.That( dbCommand.CommandText.Contains( "INSERT" ) );
        }
    }
}