
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyGenericContext.Models;

namespace MyGenericContext.Utilities
{
public class DatabaseContext : IDatabaseContext, IDisposable
    {
        private readonly ILogger _Logger;
        private readonly DatabaseConnectionSettings _ConnectionSettings;
        private RetryConnectionSettings _RetrySettings;
        private readonly SqlConnection _Connection;

        /// <summary>
        /// Handler object to request DataRetry Calls
        /// </summary>
        private DataRetryHandler _RetryPolicyHandler;

        private readonly RetryPolicy _RetryPolicy;

        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="Logger"></param>
        /// <param name="ConnectionSettings"></param>
        /// <param name="RetrySettings"></param>
        /// <param name="RetryPolicyHandler"></param>
        public DatabaseContext(ILogger<DatabaseContext> Logger,
            IOptions<DatabaseConnectionSettings> ConnectionSettings,
            IOptions<RetryConnectionSettings> RetrySettings,
            DataRetryHandler RetryPolicyHandler)
        {
            _Logger = Logger;
            _ConnectionSettings = ConnectionSettings.Value;
            _RetrySettings = RetrySettings.Value;
            _RetryPolicyHandler = RetryPolicyHandler;
            _Connection = new SqlConnection(_ConnectionSettings.DefaultConnection);

            _RetryPolicy = _RetryPolicyHandler.GenerateRetryPolicy();
        }

        /// <summary>
        /// Non DI Constructor
        /// </summary>
        /// <param name="ConnectionSettings"></param>
        /// <param name="RetrySettings"></param>
        /// <param name="RetryPolicyHandler"></param>
        public DatabaseContext(DatabaseConnectionSettings ConnectionSettings,
            RetryConnectionSettings RetrySettings,
            DataRetryHandler RetryPolicyHandler)
        {
            _Logger = ApplicationLoggerProvider.CreateLogger<DatabaseContext>();
            _ConnectionSettings = ConnectionSettings;
            _RetrySettings = RetrySettings;
            _RetryPolicyHandler = RetryPolicyHandler;
            _Connection = new SqlConnection(_ConnectionSettings.DefaultConnection);

            _RetryPolicy = _RetryPolicyHandler.GenerateRetryPolicy();
        }

        /// <summary>
        /// Additional Constructor
        /// </summary>
        /// <param name="ConnectionSettings"></param>
        /// <param name="RetrySettings"></param>
        public DatabaseContext(DatabaseConnectionSettings ConnectionSettings,
                RetryConnectionSettings RetrySettings)
        {
            _Logger = ApplicationLoggerProvider.CreateLogger<DatabaseContext>();
            _ConnectionSettings = ConnectionSettings;
            _RetrySettings = RetrySettings;
            _RetryPolicyHandler = new DataRetryHandler(_RetrySettings);
            _Connection = new SqlConnection(_ConnectionSettings.DefaultConnection);
            _RetryPolicy = _RetryPolicyHandler.GenerateRetryPolicy();
        }

        /// <summary>
        /// Adds the supplied parameters to the supplied command object
        /// </summary>
        /// <param name="command">The current SqlCommand instance</param>
        /// <param name="parameters">The supplied parameters</param>
        public void AddParameters(SqlCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                return;
            }
            foreach (var parameter in parameters)
            {
                SqlParameter sqlParameter = command.CreateParameter();
                sqlParameter.ParameterName = parameter.Key;
                sqlParameter.Value = parameter.Value ?? DBNull.Value;
                command.Parameters.Add(sqlParameter);
            }
        }

        /// <summary>
        /// Handle SQL StoredProcedure generation command generation without any additional parameters
        /// </summary>
        /// <param name="commandText">The provided command text. For stored procedures, this is the name of the stored procedure.</param>
        /// <param name="connection">The ReliableSqlConnection instance to use</param>
        /// <returns>A SQLCommand object configured for stored procedures</returns>
        public SqlCommand CreateCommand(string commandText, SqlConnection connection)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = commandText;
            return command;
        }

        /// <summary>
        /// Handle SQL StoredProcedure generation command generation with additional parameters
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="connection">The </param>
        /// <param name="parameters">The parameters associated with the stored procedure </param>
        /// <returns>A SQLCommand object configured for stored procedures</returns>
        public SqlCommand CreateCommand(string commandText, SqlConnection connection, Dictionary<string, object> parameters)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = commandText;
            AddParameters(command, parameters);
            return command;
        }

        /// <summary>
        /// Executes a non-query statement
        /// </summary>
        /// <param name="commandText">The stored procedure to be executed</param>
        /// <param name="paramaters">The parameters associated with the stored procedure (default null)</param>
        /// <returns>The count of records affected by the statement</returns>
        public async Task<int> Execute(string commandText, Dictionary<string, object> parameters)
        {
            int result = 0;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty");
            }

            try
            {
                await _RetryPolicy.ExecuteAsync(async () =>
                {
                    SqlCommand command = null;
                    if (parameters != null)
                    {
                        command = CreateCommand(commandText, _Connection);
                    }
                    else
                    {
                        command = CreateCommand(commandText, _Connection, parameters);
                    }
                    await _Connection.OpenAsync();
                    result = await command.ExecuteNonQueryAsync();
                });

            }
            catch (Exception e)
            {
                _Logger.LogError(LoggingEvents.GENERAL_ERROR, "Execution failed with exception: " + e.Message);
                result = 0;
            }
            finally
            {
                _Connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Executes a parameterless query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The stored procedure to be executed</param>
        /// <returns>A list of a Dictionary of Key, value pairs representing the Column Name and corresponding value</returns>
        public async Task<List<Dictionary<string, string>>> Query(string commandText)
        {
            List<Dictionary<string, string>> rows = null;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty");
            }

            try
            {
                await _RetryPolicy.ExecuteAsync(async () =>
                {
                    SqlCommand command = null;
                    command = CreateCommand(commandText, _Connection);
                    await _Connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        rows = new List<Dictionary<string, string>>();
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, string>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var columnName = reader.GetName(i);
                                var columnValue = reader.IsDBNull(i) ? null : reader[i].ToString(); ;
                                row.Add(columnName, columnValue);
                            }
                            rows.Add(row);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                _Logger.LogError(LoggingEvents.GENERIC_ERROR, "Execution failed with exception: " + e.Message);
            }
            finally
            {
                _Connection.Close();
            }

            return rows;
        }


        /// <summary>
        /// Executes a query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The stored procedure to be executed</param>
        /// <param name="parameters">Parameters to pass to the query</param>
        /// <returns>A list of a Dictionary of Key, value pairs representing the Column Name and corresponding value </returns>
        public async Task<List<Dictionary<string, string>>> Query(string commandText, Dictionary<string, object> parameters)
        {
            List<Dictionary<string, string>> rows = null;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty");
            }

            try
            {
                await _RetryPolicy.ExecuteAsync(async () =>
                {
                    SqlCommand command = null;
                    if (parameters != null)
                    {
                        command = CreateCommand(commandText, _Connection);
                    }
                    else
                    {
                        command = CreateCommand(commandText, _Connection, parameters);
                    }
                    await _Connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        int rowCount = 0;
                        rows = new List<Dictionary<string, string>>();
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, string>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var columnName = reader.GetName(i);
                                var columnValue = reader.IsDBNull(i) ? null : reader[i].ToString();
                                row.Add(columnName, columnValue);
                            }
                            rows.Add(row);
                            rowCount++;
                        }
                    }
                });
            }
            catch (Exception e)
            {
                //TODO
                _Logger.LogError(LoggingEvents.GENERIC_ERROR, "Execution failed with exception: " + e.Message);
            }
            finally
            {
                _Connection.Close();
            }

            return rows;
        }

        /// <summary>
        /// Executes a query that returns a single result
        /// </summary>
        /// <param name="commandText">The stored procedure to be executed</param>
        /// <param name="parameters">The parameters associated with the stored procedure</param>
        /// <returns>The returned query value</returns>
        public async Task<object> QueryValue(string commandText, Dictionary<string, object> parameters)
        {
            object queryResult = null;

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty");
            }

            try
            {
                await _RetryPolicy.ExecuteAsync(async () =>
                {
                    SqlCommand command = null;
                    if (parameters != null)
                    {
                        command = CreateCommand(commandText, _Connection);
                    }
                    else
                    {
                        command = CreateCommand(commandText, _Connection, parameters);
                    }
                    await _Connection.OpenAsync();
                    queryResult = await command.ExecuteScalarAsync();
                });
            }
            catch(Exception e)
            {
                //TODO
                _Logger.LogError(LoggingEvents.GENERIC_ERROR, "Execution failed with exception: " + e.Message);
            }
            finally
            {
                _Connection.Close();
            }

            return queryResult;
        }

        /// <summary>
        /// Helper method to return the string value of a query
        /// </summary>
        /// <param name="commandText">The stored procedure to be executed</param>
        /// <param name="parameters">Parameters to pass to the query</param>
        /// <returns>The string value resulting from the query</returns>
        public async Task<string> GetStringValue(string commandText, Dictionary<string, object> parameters)
        {
            string value = await QueryValue(commandText, parameters) as string;
            return value;
        }

        /// <summary>
        /// Inherited IDisposable method
        /// ensure closure of any connections and garbage collections
        /// </summary>
        public void Dispose()
        {
            if (_Connection != null)
            {
                //Ensure that if connection is still present, it is closed and disposed off
                _Connection.Close();
                _Connection.Dispose();
            }
        }
}