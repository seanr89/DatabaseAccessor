using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MyGenericContext.DataLayer.Interfaces
{
    public interface IDatabaseContext : IDisposable
    {
        
        /// <summary>
        /// Adds the supplied parameters to the created command
        /// </summary>
        /// <param name="command">The current SqlCommand instance</param>
        /// <param name="parameters">The supplied parameters</param>
        void AddParameters(SqlCommand command, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a non-query statement
        /// </summary>
        /// <param name="commandText">The stored procedure to be executed</param>
        /// <param name="parameters">The parameters associated with the stored procedure</param>
        /// <returns>The count of records affected by the statement</returns>
        Task<int> Execute(string commandText, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a query that returns a single result
        /// </summary>
        /// <param name="commandText">The stored procedure to be executed</param>
        /// <param name="parameters">The parameters associated with the stored procedure</param>
        /// <returns>The returned query value</returns>
        Task<object> QueryValue(string commandText, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The stored procedure to be executed</param>
        /// <param name="parameters">Parameters to pass to the query</param>
        /// <returns>A list of a Dictionary of Key, value pairs representing the Column Name and corresponding value </returns>
        Task<List<Dictionary<string, string>>> Query(string commandText, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a parameterless query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The stored procedure to be executed</param>
        /// <returns>A list of a Dictionary of Key, value pairs representing the Column Name and corresponding value</returns>
        Task<List<Dictionary<string, string>>> Query(string commandText);

        /// <summary>
        /// Creates and returns a parameterless SqlCommand object, assigning it to a connection. 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="connection"></param>
        /// <returns>SqlCommand object</returns>
        SqlCommand CreateCommand(string commandText, SqlConnection connection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="connection"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        SqlCommand CreateCommand(string commandText, SqlConnection connection, Dictionary<string, object> parameters);

        /// <summary>
        /// Ensures that the connection is disposed when not used anymore.
        /// </summary>
        void Dispose();
    }
}