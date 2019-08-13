using System.Data.Common;
using System.Diagnostics;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.Bugs
{
    /// <summary>
    ///     <value>BUG #5: GenerateInsertCommand Can Produce An Extra Trailing Comma</value>
    ///     <value>URL: https://github.com/AmbitEnergyLabs/Sequelocity.NET/issues/5 </value>
    ///     <value>
    ///     There is currently a bug in the DbCommandExtensions.GenerateInsertCommand method where under certain circumstances
    ///     it can produce an extra trailing comma in the generated SQL statement when the type has nullable properties or
    ///     fields with null values. The bug is definitely a bit weird to reproduce because internally the reflected properties
    ///     and fields are stored in a hashtable which when iterated over returns results in no predictable order so it
    ///     actually takes a while to create an example class that can cause the bug to surface.
    ///     </value>
    /// </summary>
    [TestFixture]
    public class Bug5
    {
        public class ClassWithTheLastFieldNullable
        {
            public int Ant;
            public string Baboon;
            public int? Cat;
        }

        [Test]
        public void Should_Not_Produce_A_Trailing_Comma_After_The_Last_Parameter_In_The_Parameter_List()
        {
            // Arrange
            var customer = new ClassWithTheLastFieldNullable { Ant = 1, Baboon = "broom" };

            DbCommand dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.NotNull( dbCommand.CommandText );
            Assert.False( dbCommand.CommandText.Contains( ",) VALUES" ) );
        }

        [Test]
        public void Should_Not_Produce_A_Trailing_Comma_After_The_Last_Parameter_In_The_Values_List()
        {
            // Arrange
            var customer = new ClassWithTheLastFieldNullable { Ant = 1, Baboon = "broom" };

            DbCommand dbCommand = TestHelpers.GetDbCommand();

            // Act
            dbCommand = dbCommand.GenerateInsertCommand( customer, "INSERT INTO {0} ({1}) VALUES({2});" );

            // Visual Assertion
            Trace.WriteLine( dbCommand.CommandText );

            // Assert
            Assert.NotNull( dbCommand.CommandText );
            Assert.False( dbCommand.CommandText.Contains( ",);" ) );
        }
    }
}