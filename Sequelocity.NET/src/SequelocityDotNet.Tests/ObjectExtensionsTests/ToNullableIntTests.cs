using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToNullableIntTests
    {
        class SomeObject
        {
            public int? IAmAnInt = 500;
            public int? IAmNull = null;
        }
        
        [Test]
        public void Can_Convert_To_Nullable_Int()
        {
            Assert.That( "500".ToNullableInt() == 500 );
            Assert.That( long.Parse( "500" ).ToNullableInt() == 500 );
            Assert.That( double.Parse( "500" ).ToNullableInt() == 500 );
            Assert.That( decimal.Parse( "500" ).ToNullableInt() == 500 );

            Assert.That( new SomeObject().IAmAnInt.ToNullableInt() == 500 );
            Assert.IsNull( new SomeObject().IAmNull.ToNullableInt() );
        }
    }
}