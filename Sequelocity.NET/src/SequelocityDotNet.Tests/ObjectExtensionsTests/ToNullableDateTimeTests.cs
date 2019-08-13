using System;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToNullableDateTimeTests
    {
        class SomeObject
        {
            public DateTime? IAmADateTime = DateTime.Parse( "01/01/2014" );
            public DateTime? IAmNull = null;
        }

        [Test]
        public void Can_Convert_To_Nullable_DateTime()
        {
            Assert.That( "01/01/2014".ToNullableDateTime() == DateTime.Parse( "01/01/2014" ) );
            Assert.That( "01.01.2014".ToNullableDateTime() == DateTime.Parse( "01.01.2014" ) );
            Assert.That( "01-01-2014".ToNullableDateTime() == DateTime.Parse( "01-01-2014" ) );
            Assert.That( "01 01 2014".ToNullableDateTime() == DateTime.Parse( "01 01 2014" ) );

            Assert.That( new SomeObject().IAmADateTime.ToNullableDateTime() == DateTime.Parse( "01 01 2014" ) );
            Assert.IsNull( new SomeObject().IAmNull.ToNullableDateTime() );
        }
    }
}