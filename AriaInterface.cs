using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

public enum AriaDatabase
{
    [AriaInterface.ConnectionString("Server=SERVER_CLINICAL; Database=VARIAN; Integrated Security=true;")]
    Clinical,
    [AriaInterface.ConnectionString("Server=SERVER_LEARNING; Database=VARIAN; Integrated Security=true;")]
    Learning,
    [AriaInterface.ConnectionString("Server=SERVER_RESEARCH; Database=VARIAN; Integrated Security=true;")]
    Research,
    [AriaInterface.ConnectionString("Server=SERVER_TBOX; Database=VARIAN; Integrated Security=true;")]
    TBox
}


/// <summary>
/// Contains methods for connecting to and quering Aria databases
/// <code>
/// (DataTable dataTable, Exception exception) = AriaInterface.Query(AriaDatabase.Clinical, "SELECT * FROM Patient WHERE PatientId LIKE @patId", ("patId", "test%"));
/// </code>
/// </summary>
internal static class AriaInterface
{
    /// <summary>
    /// Connect to selected Aria database and run an SQL query
    /// </summary>
    /// <param name="database">The target database</param>
    /// <param name="query">The SQL query</param>
    /// <param name="parameters">Parameter/value pairs e.g. ("@patSer", 1)</param>
    /// <returns>The result of the query (empty if exceptions are thrown) / exceptions (null if none)</returns>
    internal static (DataTable dataTable, Exception exception) Query(AriaDatabase database, string query, params (string name, object value)[] parameters)
    {
        DataTable dataTable = new DataTable();

        try
        {
            string connectionString = ((ConnectionStringAttribute[])typeof(AriaDatabase).GetField(database.ToString()).GetCustomAttributes(typeof(ConnectionStringAttribute), false))[0].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    foreach ((string parameterName,object value) in parameters)
                        command.Parameters.AddWithValue(parameterName, value);

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                        return (dataTable, null);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            return (dataTable, exception);
        }
    }

    /// <summary>
    /// Connect to selected Aria database and run an SQL query. Exceptions will be shown by a MessageBox.
    /// </summary>
    /// <param name="database">The target database</param>
    /// <param name="query">The SQL query</param>
    /// <param name="parameters">Parameter/value pairs e.g. ("@patSer", 1)</param>
    /// <returns>The result of the query (null if exceptions are thrown)</returns>
    internal static DataTable QueryWithErrorMessage(AriaDatabase database, string query, params (string name, object value)[] parameters)
    {
        (DataTable dataTable, Exception exception) = Query(database, query, parameters);

        if (exception != null)
        {
            MessageBox.Show(exception.Message, $"SQL Query Error ({database})", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }

        return dataTable;
    }

    /// <summary>
    /// This attribute is used to represent a connection string value
    /// for a value in an enum.
    /// </summary>
    internal class ConnectionStringAttribute : Attribute
    {
        /// <summary>
        /// Holds the connection string value for a value in an enum.
        /// </summary>
        public string ConnectionString { get; protected set; }

        /// <summary>
        /// Constructor used to init a ConnectionStringAttribute Attribute
        /// </summary>
        /// <param name="value"></param>
        public ConnectionStringAttribute(string value)
        {
            ConnectionString = value;
        }
    }
}