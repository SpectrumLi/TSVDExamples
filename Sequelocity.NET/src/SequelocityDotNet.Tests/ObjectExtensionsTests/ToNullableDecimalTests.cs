using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToNullableDecimalTests
    {
        class SomeObject
        {
            public decimal? IAmADecimal = 500m;
            public decimal? IAmNull = null;
        }

        [Test]
        public void Can_Convert_To_Nullable_Decimal()
        {
            Assert.That( "500".ToNullableDecimal() == 500m );
            Assert.That( long.Parse( "500" ).ToNullableDecimal() == 500m );
            Assert.That( double.Parse( "500" ).ToNullableDecimal() == 500m );
            Assert.That( decimal.Parse( "500" ).ToNullableDecimal() == 500m );

            Assert.That( new SomeObject().IAmADecimal.ToNullableDecimal() == 500m );
            Assert.IsNull( new SomeObject().IAmNull.ToNullableDecimal() );
        }
    }
}