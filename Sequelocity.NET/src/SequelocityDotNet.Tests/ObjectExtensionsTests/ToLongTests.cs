using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToLongTests
    {
        [Test]
        public void Can_Convert_To_Long()
        {
            Assert.That( "500".ToLong() == 500L );
            Assert.That( int.Parse( "500" ).ToLong() == 500L );
            Assert.That( long.Parse( "500" ).ToLong() == 500L );
            Assert.That( double.Parse( "500" ).ToLong() == 500L );
            Assert.That( decimal.Parse( "500" ).ToLong() == 500L );
        }
    }
}