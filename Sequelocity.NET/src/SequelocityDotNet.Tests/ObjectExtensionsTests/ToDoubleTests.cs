using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToDoubleTests
    {
        [Test]
        public void Can_Convert_To_Double()
        {
            Assert.That( "500".ToDouble() == 500D );
            Assert.That( long.Parse( "500" ).ToDouble() == 500D );
            Assert.That( double.Parse( "500" ).ToDouble() == 500D );
            Assert.That( decimal.Parse( "500" ).ToDouble() == 500D );
        }
    }
}