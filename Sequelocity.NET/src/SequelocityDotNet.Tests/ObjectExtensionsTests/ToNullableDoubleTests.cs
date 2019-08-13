using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToNullableDoubleTests
    {
        class SomeObject
        {
            public double? IAmADouble = 500D;
            public double? IAmNull = null;
        }

        [Test]
        public void Can_Convert_To_Nullable_Double()
        {
            Assert.That( "500".ToNullableDouble() == 500D );
            Assert.That( long.Parse( "500" ).ToNullableDouble() == 500D );
            Assert.That( double.Parse( "500" ).ToNullableDouble() == 500D );
            Assert.That( decimal.Parse( "500" ).ToNullableDouble() == 500D );

            Assert.That( new SomeObject().IAmADouble.ToNullableDouble() == 500D );
            Assert.IsNull( new SomeObject().IAmNull.ToNullableDouble() );
        }
    }
}