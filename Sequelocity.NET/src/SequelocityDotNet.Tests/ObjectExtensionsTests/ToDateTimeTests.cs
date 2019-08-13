using System;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToDateTimeTests
    {
        [Test]
        public void Can_Convert_To_DateTime()
        {
            Assert.That( "01/01/2014".ToDateTime() == DateTime.Parse( "01/01/2014" ) );
            Assert.That( "01.01.2014".ToDateTime() == DateTime.Parse( "01.01.2014" ) );
            Assert.That( "01-01-2014".ToDateTime() == DateTime.Parse( "01-01-2014" ) );
            Assert.That( "01 01 2014".ToDateTime() == DateTime.Parse( "01 01 2014" ) );
        }
    }
}