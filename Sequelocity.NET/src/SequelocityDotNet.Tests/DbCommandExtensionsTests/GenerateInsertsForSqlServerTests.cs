using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class GenerateInsertsForSqlServerTests
    {
        public class Customer
        {
            public int? CustomerId;
            public string FirstName;
            public string LastName;
            public DateTime DateOfBirth;
        }

        [Test]
        public void Should_Generate_Insert_Statements_When_Passed_An_List_Of_Instantiated_Objects()
        {
            // Arrange
            var customer1 = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
            var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
            var customer3 = new Customer { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
            var list = new List<Customer> { customer1, customer2, customer3 };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertsForSqlServer( list );

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
            var customer1 = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
            var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
            var customer3 = new Customer { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
            var list = new List<Customer> { customer1, customer2, customer3 };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertsForSqlServer( list );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "[Customer]" ) );
        }

        [Test]
        public void Should_Use_The_Supplied_Table_Name_In_The_Insert_Statements()
        {
            // Arrange
            var customer1 = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
            var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
            var customer3 = new Customer { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
            var list = new List<Customer> { customer1, customer2, customer3 };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertsForSqlServer( list, "[Person]" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.That( dbCommand.CommandText.Contains( "[Person]" ) );
        }

        [Test]
        public void Should_Add_Parameters_To_The_DbCommand_And_To_The_Insert_Statements()
        {
            // Arrange
            var customer1 = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
            var customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
            var customer3 = new Customer { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
            var list = new List<Customer> { customer1, customer2, customer3 };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertsForSqlServer( list );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            var parameters = dbCommand.Parameters.Cast<DbParameter>().ToList();

            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@FirstName" ) ).Value.ToString() == customer1.FirstName );
            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@LastName" ) ).Value.ToString() == customer1.LastName );
            Assert.That( parameters.FirstOrDefault( x => x.ParameterName.Contains( "@DateOfBirth" ) ).Value.ToString() == customer1.DateOfBirth.ToString() );
            
            Assert.That( dbCommand.CommandText.Contains( "@FirstName") );
            Assert.That( dbCommand.CommandText.Contains( "@LastName" ) );
            Assert.That( dbCommand.CommandText.Contains( "@DateOfBirth" ) );
        }

        [Test]
        public void Should_Throw_An_Exception_When_Passing_An_Anonymous_Object_And_Not_Specifying_A_TableName()
        {
            // Arrange
            var customer1 = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
            var customer2 = new { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
            var customer3 = new { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
            var list = new List<object> { customer1, customer2, customer3 };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            TestDelegate action = () => dbCommand.GenerateInsertsForSqlServer( list );

            // Assert
            var exception = Assert.Catch<ArgumentNullException>( action );
            Assert.That( exception.Message.Contains( "The 'tableName' parameter must be provided when the object supplied is an anonymous type." ) );
        }

        [Test]
        public void Should_Generate_Insert_Statements_When_Passed_A_List_Of_Anonymous_Objects()
        {
            // Arrange
            var customer1 = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
            var customer2 = new { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
            var customer3 = new { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
            var list = new List<object> { customer1, customer2, customer3 };

            var dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertsForSqlServer( list, "[Person]" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.NotNull( dbCommand.CommandText );
            Assert.That( dbCommand.CommandText.Contains( "INSERT" ) );
        }
    }
}