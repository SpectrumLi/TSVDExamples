using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class GenerateInsertForSqLiteTests
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
            var customer = new DbCommandExtensionsTests.GenerateInsertForSqlServerTests.CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            databaseCommand = databaseCommand.GenerateInsertForSQLite( customer );

            // Visual Assertion
            Trace.WriteLine( databaseCommand.DbCommand.CommandText );

            // Assert
            Assert.NotNull( databaseCommand.DbCommand.CommandText );
            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "INSERT" ) );
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
            var customer = new DbCommandExtensionsTests.GenerateInsertForSqlServerTests.CustomerWithProperties { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            databaseCommand = databaseCommand.GenerateInsertForSQLite( customer );

            // Visual Assertion
            Trace.WriteLine( databaseCommand.DbCommand.CommandText );

            // Assert
            Assert.NotNull( databaseCommand.DbCommand.CommandText );
            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "INSERT" ) );
        }

        [Test]
        public void Should_Add_The_Type_Name_When_No_Table_Name_Is_Supplied()
        {
            // Arrange
            var customer = new DbCommandExtensionsTests.GenerateInsertForSqlServerTests.CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            databaseCommand = databaseCommand.GenerateInsertForSQLite( customer );

            // Visual Assertion
            Trace.WriteLine( databaseCommand.DbCommand.CommandText );

            // Assert
            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "[CustomerWithFields]" ) );
        }

        [Test]
        public void Should_Use_The_Supplied_Table_Name_In_The_Insert_Statement()
        {
            // Arrange
            var customer = new DbCommandExtensionsTests.GenerateInsertForSqlServerTests.CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            databaseCommand = databaseCommand.GenerateInsertForSQLite( customer, "[Person]" );

            // Visual Assertion
            Trace.WriteLine( databaseCommand.DbCommand.CommandText );

            // Assert
            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "[Person]" ) );
        }

        [Test]
        public void Should_Add_Parameters_To_The_DbCommand_And_To_The_Insert_Statement()
        {
            // Arrange
            var customer = new DbCommandExtensionsTests.GenerateInsertForSqlServerTests.CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            databaseCommand = databaseCommand.GenerateInsertForSQLite( customer, "[Person]" );

            // Visual Assertion
            Trace.WriteLine( databaseCommand.DbCommand.CommandText );

            // Assert
            var parameters = databaseCommand.DbCommand.Parameters.Cast<DbParameter>().ToList();

            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@FirstName" ) ).Value.ToString() == customer.FirstName );
            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@LastName" ) ).Value.ToString() == customer.LastName );
            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@DateOfBirth" ) ).Value.ToString() == customer.DateOfBirth.ToString() );
            
            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "@FirstName" ) );
            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "@LastName" ) );
            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "@DateOfBirth" ) );
        }

        [Test]
        public void Should_Throw_An_Exception_When_Passing_An_Anonymous_Object_And_Not_Specifying_A_TableName()
        {
            // Arrange
            var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            TestDelegate action = () => databaseCommand.GenerateInsertForSQLite( customer );

            // Assert
            var exception = Assert.Catch<ArgumentNullException>( action );
            Assert.That( exception.Message.Contains( "The 'tableName' parameter must be provided when the object supplied is an anonymous type." ) );
        }

        [Test]
        public void Should_Generate_An_Insert_Statement_When_Passed_An_Anonymous_Object()
        {
            // Arrange
            var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            databaseCommand = databaseCommand.GenerateInsertForSQLite( customer, "[Person]" );

            // Visual Assertion
            Trace.WriteLine( databaseCommand.DbCommand.CommandText );

            // Assert
            Assert.NotNull( databaseCommand.DbCommand.CommandText );
            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "INSERT" ) );
        }

        public class ClassWithNoFields
        {
        }

        [Test]
        [Description( "This is just a weird test just for fun." )]
        public void Should_Generate_An_Insert_Statement_When_Passed_A_Class_With_No_Fields()
        {
            // Arrange
            var classWithNoFields = new DbCommandExtensionsTests.GenerateInsertForSqlServerTests.ClassWithNoFields();

            var databaseCommand = TestHelpers.GetDatabaseCommand();

            // Act
            databaseCommand = databaseCommand.GenerateInsertForSQLite( classWithNoFields );

            // Visual Assertion
            Trace.WriteLine( databaseCommand.DbCommand.CommandText );

            // Assert
            Assert.NotNull( databaseCommand.DbCommand.CommandText );
            Assert.That( databaseCommand.DbCommand.CommandText.Contains( "INSERT" ) );
        }
    }
}