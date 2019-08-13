using System;
using System.Configuration;
using System.Reflection;

namespace SequelocityDotNet.Tests.PostgreSQL
{
    /// <summary>
    /// Connection String Names.
    /// </summary>
    public class ConnectionStringsNames
    {
        /// <summary>
        /// PostgreSQL connection string name.
        /// </summary>
        public static string PostgreSQLConnectionString = "PostgreSQLConnectionString";

        static ConnectionStringsNames()
        {
            AddConnectionStringFromEnvironmentVariableAtRuntime("Sequelocity_PostgreSQLConnectionString", PostgreSQLConnectionString, "Npgsql");
        }

        /// <summary>
        /// Adds a connection string from an environment variable at runtime.
        /// </summary>
        /// <remarks>
        /// Note that this method is re-run safe and will return early if a connection string with the given name already exists.
        /// </remarks>
        /// <param name="environmentVariableName">Environment variable name.</param>
        /// <param name="connectionStringName">Connection string name.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <exception cref="System.Exception">Exception thrown when both the connection string and environment variable do not exist.</exception>
        public static void AddConnectionStringFromEnvironmentVariableAtRuntime(string environmentVariableName, string connectionStringName, string providerName)
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connectionStringSettings == null)
            {
                var environmentVariableValue = Environment.GetEnvironmentVariable(environmentVariableName, EnvironmentVariableTarget.Process)
                                          ?? Environment.GetEnvironmentVariable(environmentVariableName, EnvironmentVariableTarget.User)
                                          ?? Environment.GetEnvironmentVariable(environmentVariableName, EnvironmentVariableTarget.Machine);

                if (environmentVariableValue != null)
                {
                    // Enables adding a ConnectionString at runtime
                    typeof(ConfigurationElementCollection)
                        .GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic)
                        .SetValue(ConfigurationManager.ConnectionStrings, false);

                    connectionStringSettings = new ConnectionStringSettings(connectionStringName, environmentVariableValue, providerName);

                    ConfigurationManager.ConnectionStrings.Add(connectionStringSettings);

                    return;
                }

                var message = string.Format("A ConnectionString named '{0}' was not found, please add one or add an environment variable named '{1}' with the connection string as the value.", connectionStringName, environmentVariableName);

                throw new Exception(message);
            }
        }
    }
}
