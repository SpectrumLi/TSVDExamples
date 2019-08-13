using NUnit.Framework;

namespace SequelocityDotNet.Tests.ObjectExtensionsTests
{
    [TestFixture]
    public class ToDecimalTests
    {
        [Test]
        public void Can_Convert_To_Decimal()
        {
            Assert.That( "500".ToDecimal() == 500m );
            Assert.That( long.Parse( "500" ).ToDecimal() == 500m );
            Assert.That( double.Parse( "500" ).ToDecimal() == 500m );
            Assert.That( decimal.Parse( "500" ).ToDecimal() == 500m );
        }
    }
}