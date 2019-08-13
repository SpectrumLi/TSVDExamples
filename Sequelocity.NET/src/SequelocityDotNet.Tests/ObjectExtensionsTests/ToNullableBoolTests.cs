using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToNullableBoolTests
    {
        class SomeObject
        {
            public bool? IAmTrue = true;
            public bool? IAmFalse = false;
            public bool? IAmNull = null;
        }

        [Test]
        public void Can_Convert_To_Nullable_Bool()
        {
            Assert.That( "true".ToNullableBool() == true );
            Assert.That( "false".ToNullableBool() == false );

            Assert.That( 1.ToNullableBool() == true );
            Assert.That( 0.ToNullableBool() == false );

            Assert.That( true.ToNullableBool() == true );
            Assert.That( false.ToNullableBool() == false );

            Assert.That( new { BooleanProperty = true }.BooleanProperty.ToNullableBool() == true );
            Assert.That( new { BooleanProperty = false }.BooleanProperty.ToNullableBool() == false );

            Assert.That( new SomeObject().IAmTrue.ToNullableBool() == true );
            Assert.That( new SomeObject().IAmFalse.ToNullableBool() == false );
            Assert.That( new SomeObject().IAmNull.ToNullableBool() == null );
        }
    }
}