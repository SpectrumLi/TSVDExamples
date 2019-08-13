using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DataRecordMapperTests
{
    [TestFixture]
    public class MapTests
    {
        [Test]
        public void Warmup()
        {
            // This test is intentionally left blank
        }

        [TestFixture]
        public class SimpleMappingTests
        {
            public class SuperHeroWithFields
            {
                public int SuperHeroId;
                public string SuperHeroName;
                public string AlterEgoFirstName;
                public string AlterEgoLastName;
            }

            [Test]
            public void Should_Handle_Mapping_To_Types_With_Fields()
            {
                // Arrange
                var superHeroId = new KeyValuePair<string, object>( "SuperHeroId", 0 );
                var superHeroName = new KeyValuePair<string, object>( "SuperHeroName", "Superman" );
                var alterEgoFirstName = new KeyValuePair<string, object>( "AlterEgoFirstName", "Clark" );
                var alterEgoLastName = new KeyValuePair<string, object>( "AlterEgoLastName", "Kent" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { superHeroId, superHeroName, alterEgoFirstName, alterEgoLastName };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var superHero = dataRecord.Map<SuperHeroWithFields>();

                // Assert
                Assert.NotNull( superHero );
                Assert.That( superHero.SuperHeroId == (int)superHeroId.Value );
                Assert.That( superHero.SuperHeroName == superHeroName.Value.ToString() );
                Assert.That( superHero.AlterEgoFirstName == alterEgoFirstName.Value.ToString() );
                Assert.That( superHero.AlterEgoLastName == alterEgoLastName.Value.ToString() );
            }

            public class SuperHeroWithProperties
            {
                public int SuperHeroId { get; set; }
                public string SuperHeroName { get; set; }
                public string AlterEgoFirstName { get; set; }
                public string AlterEgoLastName { get; set; }
            }

            [Test]
            public void Should_Handle_Mapping_To_Types_With_Properties()
            {
                // Arrange
                var superHeroId = new KeyValuePair<string, object>( "SuperHeroId", 0 );
                var superHeroName = new KeyValuePair<string, object>( "SuperHeroName", "Superman" );
                var alterEgoFirstName = new KeyValuePair<string, object>( "AlterEgoFirstName", "Clark" );
                var alterEgoLastName = new KeyValuePair<string, object>( "AlterEgoLastName", "Kent" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { superHeroId, superHeroName, alterEgoFirstName, alterEgoLastName };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var superHero = dataRecord.Map<SuperHeroWithProperties>();

                // Assert
                Assert.NotNull( superHero );
                Assert.That( superHero.SuperHeroId == (int)superHeroId.Value );
                Assert.That( superHero.SuperHeroName == superHeroName.Value.ToString() );
                Assert.That( superHero.AlterEgoFirstName == alterEgoFirstName.Value.ToString() );
                Assert.That( superHero.AlterEgoLastName == alterEgoLastName.Value.ToString() );
            }

            public class SuperHeroWithFieldsAndProperties
            {
                public int SuperHeroId;
                public string SuperHeroName;
                public string AlterEgoFirstName { get; set; }
                public string AlterEgoLastName { get; set; }
            }

            [Test]
            public void Should_Handle_Mapping_To_Types_With_Fields_And_Properties()
            {
                // Arrange
                var superHeroId = new KeyValuePair<string, object>( "SuperHeroId", 0 );
                var superHeroName = new KeyValuePair<string, object>( "SuperHeroName", "Superman" );
                var alterEgoFirstName = new KeyValuePair<string, object>( "AlterEgoFirstName", "Clark" );
                var alterEgoLastName = new KeyValuePair<string, object>( "AlterEgoLastName", "Kent" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { superHeroId, superHeroName, alterEgoFirstName, alterEgoLastName };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var superHero = dataRecord.Map<SuperHeroWithFieldsAndProperties>();

                // Assert
                Assert.NotNull( superHero );
                Assert.That( superHero.SuperHeroId == (int)superHeroId.Value );
                Assert.That( superHero.SuperHeroName == superHeroName.Value.ToString() );
                Assert.That( superHero.AlterEgoFirstName == alterEgoFirstName.Value.ToString() );
                Assert.That( superHero.AlterEgoLastName == alterEgoLastName.Value.ToString() );
            }
        }

        [TestFixture]
        public class NullableMappingTests
        {
            public class SuperHeroWithFields
            {
                public int? SuperHeroId;
                public string SuperHeroName;
                public string AlterEgoFirstName;
                public string AlterEgoLastName;
                public DateTime? DateOfBirth;
            }

            [Test]
            public void Should_Handle_Mapping_NonNullable_Values_To_Nullable_Type_Fields()
            {
                // Arrange
                var superHeroId = new KeyValuePair<string, object>( "SuperHeroId", 500 ); // Assign to a nullable type
                var superHeroName = new KeyValuePair<string, object>( "SuperHeroName", "Superman" );
                var alterEgoFirstName = new KeyValuePair<string, object>( "AlterEgoFirstName", "Clark" );
                var alterEgoLastName = new KeyValuePair<string, object>( "AlterEgoLastName", "Kent" );
                var dateOfBirth = new KeyValuePair<string, object>( "DateOfBirth", "06/18/1938" ); // Assign to a nullable type

                var keyValuePairs = new List<KeyValuePair<string, object>> { superHeroId, superHeroName, alterEgoFirstName, alterEgoLastName, dateOfBirth };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var superHero = dataRecord.Map<SuperHeroWithFields>();

                // Assert
                Assert.NotNull( superHero );
                Assert.That( superHero.SuperHeroId == (int?)superHeroId.Value ); // Under test
                Assert.That( superHero.SuperHeroName == superHeroName.Value.ToString() );
                Assert.That( superHero.AlterEgoFirstName == alterEgoFirstName.Value.ToString() );
                Assert.That( superHero.AlterEgoLastName == alterEgoLastName.Value.ToString() );
                Assert.That( superHero.DateOfBirth == Convert.ToDateTime( dateOfBirth.Value ) ); // Under test
            }

            [Test]
            public void Should_Handle_Mapping_Null_Values_To_Nullable_Type_Fields()
            {
                // Arrange
                var superHeroId = new KeyValuePair<string, object>( "SuperHeroId", null ); // Assign to a nullable type
                var superHeroName = new KeyValuePair<string, object>( "SuperHeroName", "Superman" );
                var alterEgoFirstName = new KeyValuePair<string, object>( "AlterEgoFirstName", "Clark" );
                var alterEgoLastName = new KeyValuePair<string, object>( "AlterEgoLastName", "Kent" );
                var dateOfBirth = new KeyValuePair<string, object>( "DateOfBirth", null ); // Assign to a nullable type

                var keyValuePairs = new List<KeyValuePair<string, object>> { superHeroId, superHeroName, alterEgoFirstName, alterEgoLastName, dateOfBirth };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var superHero = dataRecord.Map<SuperHeroWithFields>();

                // Assert
                Assert.NotNull( superHero );
                Assert.That( superHero.SuperHeroId == (int?)superHeroId.Value ); // Under test
                Assert.That( superHero.SuperHeroName == superHeroName.Value.ToString() );
                Assert.That( superHero.AlterEgoFirstName == alterEgoFirstName.Value.ToString() );
                Assert.That( superHero.AlterEgoLastName == alterEgoLastName.Value.ToString() );
                Assert.That( superHero.DateOfBirth == (DateTime?)dateOfBirth.Value ); // Under test
            }

            public class SuperHeroWithProperties
            {
                public int? SuperHeroId { get; set; }
                public string SuperHeroName { get; set; }
                public string AlterEgoFirstName { get; set; }
                public string AlterEgoLastName { get; set; }
                public DateTime? DateOfBirth { get; set; }
            }

            [Test]
            public void Should_Handle_Mapping_NonNullable_Values_To_Nullable_Type_Properties()
            {
                // Arrange
                var superHeroId = new KeyValuePair<string, object>( "SuperHeroId", 500 ); // Assign to a nullable type
                var superHeroName = new KeyValuePair<string, object>( "SuperHeroName", "Superman" );
                var alterEgoFirstName = new KeyValuePair<string, object>( "AlterEgoFirstName", "Clark" );
                var alterEgoLastName = new KeyValuePair<string, object>( "AlterEgoLastName", "Kent" );
                var dateOfBirth = new KeyValuePair<string, object>( "DateOfBirth", "06/18/1938" ); // Assign to a nullable type

                var keyValuePairs = new List<KeyValuePair<string, object>> { superHeroId, superHeroName, alterEgoFirstName, alterEgoLastName, dateOfBirth };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var superHero = dataRecord.Map<SuperHeroWithProperties>();

                // Assert
                Assert.NotNull( superHero );
                Assert.That( superHero.SuperHeroId == (int?)superHeroId.Value ); // Under test
                Assert.That( superHero.SuperHeroName == superHeroName.Value.ToString() );
                Assert.That( superHero.AlterEgoFirstName == alterEgoFirstName.Value.ToString() );
                Assert.That( superHero.AlterEgoLastName == alterEgoLastName.Value.ToString() );
                Assert.That( superHero.DateOfBirth == Convert.ToDateTime( dateOfBirth.Value ) ); // Under test
            }

            [Test]
            public void Should_Handle_Mapping_Null_Values_To_Nullable_Type_Properties()
            {
                // Arrange
                var superHeroId = new KeyValuePair<string, object>( "SuperHeroId", null ); // Assign to a nullable type
                var superHeroName = new KeyValuePair<string, object>( "SuperHeroName", "Superman" );
                var alterEgoFirstName = new KeyValuePair<string, object>( "AlterEgoFirstName", "Clark" );
                var alterEgoLastName = new KeyValuePair<string, object>( "AlterEgoLastName", "Kent" );
                var dateOfBirth = new KeyValuePair<string, object>( "DateOfBirth", null ); // Assign to a nullable type

                var keyValuePairs = new List<KeyValuePair<string, object>> { superHeroId, superHeroName, alterEgoFirstName, alterEgoLastName, dateOfBirth };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var superHero = dataRecord.Map<SuperHeroWithProperties>();

                // Assert
                Assert.NotNull( superHero );
                Assert.That( superHero.SuperHeroId == (int?)superHeroId.Value ); // Under test
                Assert.That( superHero.SuperHeroName == superHeroName.Value.ToString() );
                Assert.That( superHero.AlterEgoFirstName == alterEgoFirstName.Value.ToString() );
                Assert.That( superHero.AlterEgoLastName == alterEgoLastName.Value.ToString() );
                Assert.That( superHero.DateOfBirth == (DateTime?)dateOfBirth.Value ); // Under test
            }
        }

        [TestFixture]
        public class SimpleTypeConversionTests
        {
            public class CustomerWithFields
            {
                public int CustomerId;
                public string FirstName;
                public string LastName;
                public DateTime DateOfBirth;
            }

            [Test]
            public void Should_Handle_Mapping_Simple_Type_Conversions_On_Fields()
            {
                // Arrange
                var customerId = new KeyValuePair<string, object>( "CustomerId", Convert.ToInt64( 5000 ) ); // Assign to int
                var firstName = new KeyValuePair<string, object>( "FirstName", "Clark" );
                var lastName = new KeyValuePair<string, object>( "LastName", "Kent" );
                var dateOfBirth = new KeyValuePair<string, object>( "DateOfBirth", "06/18/1938" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { customerId, firstName, lastName, dateOfBirth };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var customer = dataRecord.Map<CustomerWithFields>();

                // Assert
                Assert.NotNull( customer );
                Assert.That( customer.CustomerId == Convert.ToInt64( customerId.Value ) ); // Under test
                Assert.That( customer.FirstName == firstName.Value.ToString() );
                Assert.That( customer.LastName == lastName.Value.ToString() );
                Assert.That( customer.DateOfBirth == Convert.ToDateTime( dateOfBirth.Value ) );
            }

            [Test]
            public void Should_Throw_An_Exception_When_Converting_NonStandard_Values_On_Fields()
            {
                // Arrange
                var customerId = new KeyValuePair<string, object>( "CustomerId", Convert.ToInt64( 5000 ) );
                var firstName = new KeyValuePair<string, object>( "FirstName", "Clark" );
                var lastName = new KeyValuePair<string, object>( "LastName", "Kent" );
                var dateOfBirth = new KeyValuePair<string, object>( "DateOfBirth", "June 18th, 1938" ); // Non-standard DateTime value

                var keyValuePairs = new List<KeyValuePair<string, object>> { customerId, firstName, lastName, dateOfBirth };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                TestDelegate action = () => dataRecord.Map<CustomerWithFields>();

                // Assert
                Assert.Throws<TypeConverter.TypeConversionException>( action );
            }

            public class CustomerWithProperties
            {
                public int CustomerId { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public DateTime DateOfBirth { get; set; }
            }

            [Test]
            public void Should_Handle_Mapping_Simple_Type_Conversions_On_Properties()
            {
                // Arrange
                var customerId = new KeyValuePair<string, object>( "CustomerId", Convert.ToInt64( 5000 ) ); // Assign to int
                var firstName = new KeyValuePair<string, object>( "FirstName", "Clark" );
                var lastName = new KeyValuePair<string, object>( "LastName", "Kent" );
                var dateOfBirth = new KeyValuePair<string, object>( "DateOfBirth", "06/18/1938" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { customerId, firstName, lastName, dateOfBirth };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var customer = dataRecord.Map<CustomerWithProperties>();

                // Assert
                Assert.NotNull( customer );
                Assert.That( customer.CustomerId == Convert.ToInt64( customerId.Value ) ); // Under test
                Assert.That( customer.FirstName == firstName.Value.ToString() );
                Assert.That( customer.LastName == lastName.Value.ToString() );
                Assert.That( customer.DateOfBirth == Convert.ToDateTime( dateOfBirth.Value ) );
            }

            [Test]
            public void Should_Throw_An_Exception_When_Converting_NonStandard_Values_On_Properties()
            {
                // Arrange
                var customerId = new KeyValuePair<string, object>( "CustomerId", Convert.ToInt64( 5000 ) );
                var firstName = new KeyValuePair<string, object>( "FirstName", "Clark" );
                var lastName = new KeyValuePair<string, object>( "LastName", "Kent" );
                var dateOfBirth = new KeyValuePair<string, object>( "DateOfBirth", "June 18th, 1938" ); // Non-standard DateTime value

                var keyValuePairs = new List<KeyValuePair<string, object>> { customerId, firstName, lastName, dateOfBirth };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                TestDelegate action = () => dataRecord.Map<CustomerWithProperties>();

                // Assert
                Assert.Throws<TypeConverter.TypeConversionException>( action );
            }
        }

        [TestFixture]
        public class EnumConversionTests
        {
            public class BankAccountWithFields
            {
                public string AccountHolderFullName;

                public BankAccountType AccountType;

                public enum BankAccountType
                {
                    Checking,
                    Savings,
                    MoneyMarket
                }
            }

            [Test]
            public void Should_Handle_Mapping_Strings_To_Enum_Values_On_Fields()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", "Savings" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType.ToString() == accountType.Value.ToString() );
            }

            [Test]
            public void Should_Handle_Mapping_Integers_To_Enum_Values_On_Fields()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", 1 );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType.ToString() == Enum.ToObject( typeof( BankAccountWithFields.BankAccountType ), accountType.Value ).ToString() );
            }

            [Test]
            public void Should_Use_The_Default_Value_When_Mapping_A_Null_To_An_Enum_On_Fields()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", null );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType == BankAccountWithFields.BankAccountType.Checking ); // Checking is the default value
            }

            [Test]
            public void Should_Throw_An_Exception_When_Mapping_An_Invalid_Value_To_An_Enum_On_Fields()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", "asdf;lkj" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                TestDelegate action = () => dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.Throws<TypeConverter.TypeConversionException>( action );
            }

            public class BankAccountWithProperties
            {
                public string AccountHolderFullName { get; set; }

                public BankAccountType AccountType { get; set; }

                public enum BankAccountType
                {
                    Checking,
                    Savings,
                    MoneyMarket
                }
            }

            [Test]
            public void Should_Handle_Mapping_Strings_To_Enum_Values_On_Properties()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", "Savings" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType.ToString() == accountType.Value.ToString() );
            }

            [Test]
            public void Should_Handle_Mapping_Integers_To_Enum_Values_On_Properties()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", 1 );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType.ToString() == Enum.ToObject( typeof( BankAccountWithFields.BankAccountType ), accountType.Value ).ToString() );
            }

            [Test]
            public void Should_Use_The_Default_Value_When_Mapping_A_Null_To_An_Enum_On_Properties()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", null );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType == BankAccountWithFields.BankAccountType.Checking ); // Checking is the default value
            }

            [Test]
            public void Should_Throw_An_Exception_When_Mapping_An_Invalid_Value_To_An_Enum_On_Properties()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", "asdf;lkj" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                TestDelegate action = () => dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.Throws<TypeConverter.TypeConversionException>( action );
            }
        }

        [TestFixture]
        public class NullableEnumConversionTests
        {
            public class BankAccountWithFields
            {
                public string AccountHolderFullName;

                public BankAccountType? AccountType;

                public enum BankAccountType
                {
                    Checking,
                    Savings,
                    MoneyMarket
                }
            }

            [Test]
            public void Should_Handle_Mapping_Strings_To_Enum_Values_On_Fields()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", "Savings" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType.ToString() == accountType.Value.ToString() );
            }

            [Test]
            public void Should_Handle_Mapping_Integers_To_Enum_Values_On_Fields()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", 1 );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType.ToString() == Enum.ToObject( typeof( BankAccountWithFields.BankAccountType ), accountType.Value ).ToString() );
            }

            [Test]
            public void Should_Assign_Null_When_Mapping_A_Null_To_An_Enum_On_Fields()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", null );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType == null ); // Checking if null
            }

            [Test]
            public void Should_Throw_An_Exception_When_Mapping_An_Invalid_Value_To_An_Enum_On_Fields()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", "asdf;lkj" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                TestDelegate action = () => dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.Throws<TypeConverter.TypeConversionException>( action );
            }

            public class BankAccountWithProperties
            {
                public string AccountHolderFullName { get; set; }

                public BankAccountType? AccountType { get; set; }

                public enum BankAccountType
                {
                    Checking,
                    Savings,
                    MoneyMarket
                }
            }

            [Test]
            public void Should_Handle_Mapping_Strings_To_Enum_Values_On_Properties()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", "Savings" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType.ToString() == accountType.Value.ToString() );
            }

            [Test]
            public void Should_Handle_Mapping_Integers_To_Enum_Values_On_Properties()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", 1 );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType.ToString() == Enum.ToObject( typeof( BankAccountWithFields.BankAccountType ), accountType.Value ).ToString() );
            }

            [Test]
            public void Should_Use_The_Default_Value_When_Mapping_A_Null_To_An_Enum_On_Properties()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", null );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var bankAccount = dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.NotNull( bankAccount );
                Assert.That( bankAccount.AccountHolderFullName == accountHolderFullName.Value.ToString() );
                Assert.That( bankAccount.AccountType == null ); // Checking if null
            }

            [Test]
            public void Should_Throw_An_Exception_When_Mapping_An_Invalid_Value_To_An_Enum_On_Properties()
            {
                // Arrange
                var accountHolderFullName = new KeyValuePair<string, object>( "AccountHolderFullName", "Clark Kent" );
                var accountType = new KeyValuePair<string, object>( "AccountType", "asdf;lkj" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { accountHolderFullName, accountType };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                TestDelegate action = () => dataRecord.Map<BankAccountWithFields>();

                // Assert
                Assert.Throws<TypeConverter.TypeConversionException>( action );
            }
        }

        [TestFixture]
        public class GuidConversionTests
        {
            public class MonsterWithFields
            {
                public Guid MonsterId;
                public string MonsterName;
            }

            [Test]
            public void Should_Handle_Mapping_Strings_To_Guid_Values_On_Fields()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", Guid.NewGuid() );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithFields>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId.ToString() == monsterId.Value.ToString() );
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Handle_Mapping_ByteArray_To_Guid_Values_On_Fields()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", Guid.NewGuid().ToByteArray() );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithFields>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId.ToByteArray().ToString() == monsterId.Value.ToString() );
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Convert_A_Null_To_A_Default_Guid_When_To_Guid_Values_On_Fields()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", null );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithFields>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId.ToString() == default( Guid ).ToString() );
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Throw_An_Exception_When_Mapping_An_Invalid_Value_To_A_Guid_On_Fields()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", "asdf;lkj" );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                TestDelegate action = () => dataRecord.Map<MonsterWithFields>();

                // Assert
                Assert.Throws<TypeConverter.TypeConversionException>( action );
            }

            public class MonsterWithProperties
            {
                public Guid MonsterId;
                public string MonsterName;
            }

            [Test]
            public void Should_Handle_Mapping_Strings_To_Guid_Values_On_Properties()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", Guid.NewGuid() );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithProperties>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId.ToString() == monsterId.Value.ToString() );
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Handle_Mapping_ByteArray_To_Guid_Values_On_Properties()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", Guid.NewGuid().ToByteArray() );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithProperties>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId.ToByteArray().ToString() == monsterId.Value.ToString() );
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Convert_A_Null_To_A_Default_Guid_When_To_Guid_Values_On_Properties()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", null );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithProperties>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId.ToString() == default( Guid ).ToString() );
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Throw_An_Exception_When_Mapping_An_Invalid_Value_To_A_Guid_On_Properties()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", "asdf;lkj" );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                TestDelegate action = () => dataRecord.Map<MonsterWithProperties>();

                // Assert
                Assert.Throws<TypeConverter.TypeConversionException>( action );
            }
        }

        [TestFixture]
        public class NullableGuidConversionTests
        {
            public class MonsterWithFields
            {
                public Guid? MonsterId;
                public string MonsterName;
            }

            [Test]
            public void Should_Handle_Mapping_Strings_To_Guid_Values_On_Fields()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", Guid.NewGuid() );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithFields>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId.ToString() == monsterId.Value.ToString() );
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Handle_Mapping_ByteArray_To_Guid_Values_On_Fields()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", Guid.NewGuid().ToByteArray() );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithFields>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId.Value.ToByteArray().ToString() == monsterId.Value.ToString() );
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Convert_A_Null_To_A_Default_Guid_When_To_Guid_Values_On_Fields()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", null );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithFields>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId == null ); // Ensure is null
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Throw_An_Exception_When_Mapping_An_Invalid_Value_To_A_Guid_On_Fields()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", "asdf;lkj" );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                TestDelegate action = () => dataRecord.Map<MonsterWithFields>();

                // Assert
                Assert.Throws<TypeConverter.TypeConversionException>( action );
            }

            public class MonsterWithProperties
            {
                public Guid? MonsterId;
                public string MonsterName;
            }

            [Test]
            public void Should_Handle_Mapping_Strings_To_Guid_Values_On_Properties()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", Guid.NewGuid() );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithProperties>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId.ToString() == monsterId.Value.ToString() );
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Handle_Mapping_ByteArray_To_Guid_Values_On_Properties()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", Guid.NewGuid().ToByteArray() );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithProperties>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId.Value.ToByteArray().ToString() == monsterId.Value.ToString() );
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Convert_A_Null_To_A_Default_Guid_When_To_Guid_Values_On_Properties()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", null );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                var monster = dataRecord.Map<MonsterWithProperties>();

                // Assert
                Assert.NotNull( monster );
                Assert.That( monster.MonsterId == null );  // Ensure is null
                Assert.That( monster.MonsterName == monstername.Value.ToString() );
            }

            [Test]
            public void Should_Throw_An_Exception_When_Mapping_An_Invalid_Value_To_A_Guid_On_Properties()
            {
                // Arrange
                var monsterId = new KeyValuePair<string, object>( "MonsterId", "asdf;lkj" );
                var monstername = new KeyValuePair<string, object>( "MonsterName", "Frankenstein" );

                var keyValuePairs = new List<KeyValuePair<string, object>> { monsterId, monstername };

                var dataRecord = new TestHelpers.DataRecord( keyValuePairs );

                // Act
                TestDelegate action = () => dataRecord.Map<MonsterWithProperties>();

                // Assert
                Assert.Throws<TypeConverter.TypeConversionException>( action );
            }
        }
    }
}