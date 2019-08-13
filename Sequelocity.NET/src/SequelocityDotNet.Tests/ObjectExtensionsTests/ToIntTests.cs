using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToIntTests
    {
        [Test]
        public void Can_Convert_To_Int()
        {
            Assert.That( "500".ToInt() == 500 );
            Assert.That( long.Parse( "500" ).ToInt() == 500 );
            Assert.That( double.Parse( "500" ).ToInt() == 500 );
            Assert.That( decimal.Parse( "500" ).ToInt() == 500 );
        }
    }
}