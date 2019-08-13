using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class GenerateInsertForSQLiteTests
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
            dbCommand = dbCommand.GenerateInsertForSQLite( customer );

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
            dbCommand = dbCommand.GenerateInsertForSQLite( customer );

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
            dbCommand = dbCommand.GenerateInsertForSQLite( customer );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "[CustomerWithFields]" ) );
        }

        [Test]
        public void Should_Use_The_Supplied_Table_Name_In_The_Insert_Statement()
        {
            // Arrange
            var customer = new CustomerWithFields { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertForSQLite( customer, "[Person]" );

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
            dbCommand = dbCommand.GenerateInsertForSQLite( customer, "[Person]" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            var parameters = dbCommand.Parameters.Cast<DbParameter>().ToList();

            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@FirstName" ) ).Value.ToString() == customer.FirstName );
            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@LastName" ) ).Value.ToString() == customer.LastName );
            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@DateOfBirth" ) ).Value.ToString() == customer.DateOfBirth.ToString() );
            
            Assert.That( dbCommand.CommandText.Contains( "@FirstName") );
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
            TestDelegate action = () => dbCommand.GenerateInsertForSQLite( customer );

            // Assert
            var exception = Assert.Catch<ArgumentNullException>( action );
            Assert.That( exception.Message.Contains( "The 'tableName' parameter must be provided when the object supplied is an anonymous type." ) );
        }

        [Test]
        public void Should_Generate_An_Insert_Statement_When_Passed_An_Anonymous_Object()
        {
            // Arrange
            var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertForSQLite( customer, "[Person]" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.NotNull( dbCommand.CommandText );
            Assert.That( dbCommand.CommandText.Contains( "INSERT" ) );
        }

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
            dbCommand = dbCommand.GenerateInsertForSQLite( classWithNoFields );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.NotNull( dbCommand.CommandText );
            Assert.That( dbCommand.CommandText.Contains( "INSERT" ) );
        }
    }
}