using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace MySqlHelper
{
    public class MySqlHelperNS
    {
        private readonly string connectionString;

        public MySqlHelperNS(string host, string database, string username, string password)
        {
            connectionString = string.Format(
                "SERVER={0}; DATABASE={1}; UID={2}; PWD={3}; PORT=; Allow User Variables=True;SSL Mode=None;",
                host, database, username, password);
        }

        public MySqlHelperResult Execute(string query, bool readData, Dictionary<string, object> parameters = null)
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    mySqlConnection.Open();
                    MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction();

                    try
                    {
                        using (MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection, mySqlTransaction))
                        {
                            if (parameters != null && parameters.Count > 0)
                            {
                                foreach (KeyValuePair<string, object> parameter in parameters)
                                {
                                    mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                }
                            }
                            if (readData)
                            {
                                DataTable result = null;

                                using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader())
                                {
                                    if (mySqlDataReader.HasRows)
                                    {
                                        result = new DataTable();
                                        result.Load(mySqlDataReader);
                                    }
                                }

                                mySqlTransaction.Commit();
                                return new MySqlHelperResult(data: result?.CreateDataReader());
                            }
                            else
                            {
                                int affected_rows = mySqlCommand.ExecuteNonQuery();
                                mySqlTransaction.Commit();
                                return new MySqlHelperResult(affected_rows: affected_rows);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        mySqlTransaction.Rollback();
                        return new MySqlHelperResult(error: exception);
                    }

                }
                catch (Exception exception)
                {
                    return new MySqlHelperResult(error: exception);
                }
            }
        }
        public static string EscapeString(string value)
        {
            return MySql.Data.MySqlClient.MySqlHelper.EscapeString(value);
        }
    }
}