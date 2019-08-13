using System.Configuration;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SqlServer.SequelocityTests
{
    [TestFixture]
    public class GetDatabaseCommandTests
    {
        [Test]
        public void Can_Get_A_DatabaseCommand_From_A_ConnectionString_Name()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionStringName = ConnectionStringsNames.SqlServerConnectionString;

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommand( connectionStringName );

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.TestConnection() );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Can_Get_A_DatabaseCommand_From_A_ConnectionString_And_A_DbProviderFactoryInvariantName()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionString = ConfigurationManager.ConnectionStrings[ ConnectionStringsNames.SqlServerConnectionString ].ConnectionString;

            const string dbProviderFactoryInvariantName = "System.Data.SqlClient";

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommand( connectionString, dbProviderFactoryInvariantName );

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.TestConnection() );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Can_Get_A_DatabaseCommand_By_Setting_A_Default_ConnectionString_Name()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            Sequelocity.ConfigurationSettings.Default.ConnectionStringName = ConnectionStringsNames.SqlServerConnectionString;

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommand();

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.TestConnection() );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Can_Get_A_DatabaseCommand_By_Setting_A_Default_ConnectionString_And_A_DbProviderFactoryInvariantName()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            Sequelocity.ConfigurationSettings.Default.ConnectionString = ConfigurationManager.ConnectionStrings[ ConnectionStringsNames.SqlServerConnectionString ].ConnectionString;
            Sequelocity.ConfigurationSettings.Default.DbProviderFactoryInvariantName = "System.Data.SqlClient";

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommand();

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.TestConnection() );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Throws_An_Exception_When_No_ConnectionString_Could_Be_Found()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            // Act
            TestDelegate action = () => Sequelocity.GetDatabaseCommand();

            // Assert
            Assert.Throws<Sequelocity.ConnectionStringNotFoundException>( action );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Throws_An_Exception_When_No_DbProviderFactory_Could_Be_Found()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringsNames.SqlServerConnectionString].ConnectionString;

            // Act
            TestDelegate action = () => Sequelocity.GetDatabaseCommand( connectionString );

            // Assert
            Assert.Throws<Sequelocity.DbProviderFactoryNotFoundException>( action );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }
    }
}