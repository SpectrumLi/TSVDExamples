using System.Collections.Generic;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DataRecordMapperTests
{
    [TestFixture]
    public class MapDynamicTests
    {
        [Test]
        public void Should_Handle_Mapping_To_A_Dynamic_Object()
        {
            // Arrange
            var superHeroId = new KeyValuePair<string, object>( "SuperHeroId", "500" );
            var superHeroName = new KeyValuePair<string, object>( "SuperHeroName", "Superman" );
            var alterEgoFirstName = new KeyValuePair<string, object>( "AlterEgoFirstName", "Clark" );
            var alterEgoLastName = new KeyValuePair<string, object>( "AlterEgoLastName", "Kent" );

            var keyValuePairs = new List<KeyValuePair<string, object>> { superHeroId, superHeroName, alterEgoFirstName, alterEgoLastName };

            var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

            // Act
            var obj = dataRecord.MapDynamic();

            // Assert
            Assert.That( obj.SuperHeroId == superHeroId.Value );
            Assert.That( obj.SuperHeroName == superHeroName.Value );
            Assert.That( obj.AlterEgoFirstName == alterEgoFirstName.Value );
            Assert.That( obj.AlterEgoLastName == alterEgoLastName.Value );
        }

        [Test]
        public void Should_Handle_Mapping_Nulls_To_A_Dynamic_Object()
        {
            // Arrange
            var superHeroId = new KeyValuePair<string, object>( "SuperHeroId", "500" );
            var superHeroName = new KeyValuePair<string, object>( "SuperHeroName", "Superman" );
            var alterEgoFirstName = new KeyValuePair<string, object>( "AlterEgoFirstName", null );
            var alterEgoLastName = new KeyValuePair<string, object>( "AlterEgoLastName", null );

            var keyValuePairs = new List<KeyValuePair<string, object>> { superHeroId, superHeroName, alterEgoFirstName, alterEgoLastName };

            var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

            // Act
            var obj = dataRecord.MapDynamic();

            // Assert
            Assert.That( obj.SuperHeroId == superHeroId.Value );
            Assert.That( obj.SuperHeroName == superHeroName.Value );
            Assert.That( obj.AlterEgoFirstName == alterEgoFirstName.Value );
            Assert.That( obj.AlterEgoLastName == alterEgoLastName.Value );
        }

        [Test]
        public void Should_Handle_Mapping_Duplicate_Fields_In_A_Dynamic_Object()
        {
            // Arrange
            var superHeroId = new KeyValuePair<string, object>( "SuperHeroId", "500" );
            var superHeroName = new KeyValuePair<string, object>( "SuperHeroName", "Superman" );
            var alterEgoFirstName = new KeyValuePair<string, object>( "AlterEgoFirstName", null );
            var alterEgoLastName = new KeyValuePair<string, object>( "AlterEgoLastName", null );
            var superHeroIdNumberTwo = new KeyValuePair<string, object>( "SuperHeroId", null ); // The second occurrence has a null value

            var keyValuePairs = new List<KeyValuePair<string, object>> { superHeroId, superHeroName, alterEgoFirstName, alterEgoLastName, superHeroIdNumberTwo };

            var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

            // Act
            var obj = dataRecord.MapDynamic();

            // Assert
            Assert.That( obj.SuperHeroId != superHeroId.Value ); // Under test
            Assert.That( obj.SuperHeroId == superHeroIdNumberTwo.Value );  // Under test
            Assert.That( obj.SuperHeroName == superHeroName.Value );
            Assert.That( obj.AlterEgoFirstName == alterEgoFirstName.Value );
            Assert.That( obj.AlterEgoLastName == alterEgoLastName.Value );
        }
    }
}