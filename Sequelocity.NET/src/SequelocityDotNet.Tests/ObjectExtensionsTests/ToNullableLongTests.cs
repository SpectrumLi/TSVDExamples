using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToNullableLongTests
    {
        class SomeObject
        {
            public long? IAmALong = 50000000L;
            public long? IAmNull = null;
        }

        [Test]
        public void Can_Convert_To_Nullable_Long()
        {
            Assert.That( "500".ToNullableLong() == 500L );
            Assert.That( int.Parse( "500" ).ToNullableLong() == 500L );
            Assert.That( long.Parse( "500" ).ToNullableLong() == 500L );
            Assert.That( double.Parse( "500" ).ToNullableLong() == 500L );
            Assert.That( decimal.Parse( "500" ).ToNullableLong() == 500L );

            Assert.That( new SomeObject().IAmALong.ToNullableLong() == 50000000L );
            Assert.IsNull( new SomeObject().IAmNull.ToNullableLong() );
        }
    }
}