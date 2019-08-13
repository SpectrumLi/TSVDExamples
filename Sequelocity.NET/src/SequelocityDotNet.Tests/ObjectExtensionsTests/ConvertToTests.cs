using System;
using System.Diagnostics;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ConvertToTests
    {
        [Test]
        public void Can_Convert_To_Int()
        {
            Assert.That( "500".ConvertTo<int>() == 500 );
            Assert.That( long.Parse( "500" ).ConvertTo<int>() == 500 );
            Assert.That( double.Parse( "500" ).ConvertTo<int>() == 500 );
            Assert.That( decimal.Parse( "500" ).ConvertTo<int>() == 500 );
        }

        [Test]
        public void Can_Convert_To_Long()
        {
            Assert.That( "500".ConvertTo<long>() == 500L );
            Assert.That( long.Parse( "500" ).ConvertTo<long>() == 500L );
            Assert.That( double.Parse( "500" ).ConvertTo<long>() == 500L );
            Assert.That( decimal.Parse( "500" ).ConvertTo<long>() == 500L );
        }

        [Test]
        public void Can_Convert_To_Double()
        {
            Assert.That( "500".ConvertTo<double>() == 500D );
            Assert.That( long.Parse( "500" ).ConvertTo<double>() == 500D );
            Assert.That( double.Parse( "500" ).ConvertTo<double>() == 500D );
            Assert.That( decimal.Parse( "500" ).ConvertTo<double>() == 500D );
        }

        [Test]
        public void Can_Convert_To_Decimal()
        {
            Assert.That( "500".ConvertTo<decimal>() == 500m );
            Assert.That( long.Parse( "500" ).ConvertTo<decimal>() == 500m );
            Assert.That( double.Parse( "500" ).ConvertTo<decimal>() == 500m );
            Assert.That( decimal.Parse( "500" ).ConvertTo<decimal>() == 500m );
        }

        [Test]
        public void Can_Convert_To_DateTime()
        {
            Assert.That( "01/01/2014".ConvertTo<DateTime>() == DateTime.Parse( "01/01/2014" ) );
            Assert.That( "01.01.2014".ConvertTo<DateTime>() == DateTime.Parse( "01.01.2014" ) );
            Assert.That( "01-01-2014".ConvertTo<DateTime>() == DateTime.Parse( "01-01-2014" ) );
            Assert.That( "01 01 2014".ConvertTo<DateTime>() == DateTime.Parse( "01 01 2014" ) );
        }

        [Test]
        public void Can_Convert_To_Bool()
        {
            Assert.That( "true".ConvertTo<bool>() == true );
            Assert.That( "false".ConvertTo<bool>() == false );
            Assert.That( 1.ConvertTo<bool>() == true );
            Assert.That( 0.ConvertTo<bool>() == false );
        }

        [Test]
        public void Can_Convert_To_String()
        {
            Assert.That( true.ConvertTo<string>() == "True" ); // Boolean true to String
            Assert.That( false.ConvertTo<string>() == "False" ); // Boolean false to String
            
            Assert.That( 500.ConvertTo<string>() == "500" ); // Int to String
            Assert.That( 500L.ConvertTo<string>() == "500" ); // Long to String
            Assert.That( 500D.ConvertTo<string>() == "500" ); // Double to String
            Assert.That( 500m.ConvertTo<string>() == "500" ); // Decimal to String

            Assert.That( DateTime.Parse( "01/01/2014" ).ConvertTo<string>() == "1/1/2014 12:00:00 AM" ); // DateTime to String
        }
    }
}