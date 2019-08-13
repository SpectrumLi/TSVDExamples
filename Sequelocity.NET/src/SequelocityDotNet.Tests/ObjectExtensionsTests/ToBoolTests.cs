using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToBoolTests
    {
        [Test]
        public void Can_Convert_To_Bool()
        {
            Assert.True( "true".ToBool() );
            Assert.False( "false".ToBool() );

            Assert.True( 1.ToBool() );
            Assert.False( 0.ToBool() );

            Assert.True( true.ToBool() );
            Assert.False( false.ToBool() );

            Assert.True( new { BooleanProperty = true }.BooleanProperty.ToBool() );
            Assert.False( new { BooleanProperty = false }.BooleanProperty.ToBool() );
        }
    }
}