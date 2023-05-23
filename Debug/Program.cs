using MySql.Data.MySqlClient;
using MySqlHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Debug
{
    class Program
    {
        private static readonly MySqlHelperNS mySql = new("localhost", "telegram", "root", "root");

        static void Main(string[] args)
        {
            //GetUser
            MySqlHelperData result = GetUser("123456789");

            if(result.error == null)
            {
                DatabaseUser user = (DatabaseUser) result.data;

                Console.WriteLine(user.telegramId);
                Console.WriteLine(user.expireAt);
            }
            else
            {
                Console.WriteLine(result.error);
            }

            //GetUsers
            MySqlHelperData result2 = GetUsers();

            if (result2.error == null)
            {
                List<DatabaseUser> user = (List<DatabaseUser>)result2.data;

                Console.WriteLine(user.Count);
            }
            else
            {
                Console.WriteLine(result2.error);
            }

            //AddUser
            MySqlHelperData result3 = AddUser("123456789", DateTimeOffset.Now);

            if (result3.error == null)
            {
                if ((bool)result3.data)
                    Console.WriteLine("user added");
                else
                    Console.WriteLine("error adding user");
            }
            else
            {
                Console.WriteLine(result3.error);
            }

            //AddUser
            MySqlHelperData result4 = DeleteUser("123456789");

            if (result4.error == null)
            {
                if ((bool)result4.data)
                    Console.WriteLine("user deleted");
                else
                    Console.WriteLine("error deleting user");
            }
            else
            {
                Console.WriteLine(result4.error);
            }

            //prevent to console closes
            Console.ReadKey();
        }

        public static MySqlHelperData GetUser(string telegram_id)
        {
            string query =
                "SELECT JSON_OBJECT('telegramId', telegram_id, 'expireAt', expire_at) AS USER " +
                "FROM users WHERE telegram_id = @telegram_id;";

            Dictionary<string, object> queryParameters = new();
            queryParameters.Add("@telegram_id", telegram_id);

            MySqlHelperResult result = mySql.Execute(query, true, queryParameters);

            if (result.error == null)
            {
                IDataReader data = (IDataReader)result.data;

                if (data != null)
                {
                    DatabaseUser user = null;

                    while (data.Read())
                    {
                        user = JsonConvert.DeserializeObject<DatabaseUser>((string)data["USER"]);
                    }

                    return new MySqlHelperData(data: user);
                }
                else
                {
                    return new MySqlHelperData(data: null);
                }
            }
            else
            {
                return new MySqlHelperData(error: result.error);
            }
        }        
        public static MySqlHelperData GetUsers()
        {
            string query = "SELECT JSON_OBJECT('telegramId', telegram_id, 'expireAt', expire_at) AS USER FROM users;";

            MySqlHelperResult result = mySql.Execute(query, true);

            if (result.error == null)
            {
                IDataReader data = (IDataReader)result.data;

                List<DatabaseUser> users = new();

                if (data != null)
                {
                    while (data.Read())
                    {
                        users.Add(
                            JsonConvert.DeserializeObject<DatabaseUser>((string)data["USER"])
                            );
                    }

                    return new MySqlHelperData(data: users);
                }
                else
                {
                    return new MySqlHelperData(data: users);
                }
            }
            else
            {
                return new MySqlHelperData(error: result.error);
            }
        }
        public static MySqlHelperData AddUser(string telegram_id, DateTimeOffset expire)
        {
            string query = "INSERT INTO users (telegram_id, expire_at) VALUES (@telegram_id, @expire_at);";

            Dictionary<string, object> queryParameters = new();
            queryParameters.Add("@telegram_id", telegram_id);
            queryParameters.Add("@expire_at", expire.ToString("yyyy-MM-dd HH':'mm':'ss"));

            MySqlHelperResult result = mySql.Execute(query, false, queryParameters);

            if (result.error == null)
            {
                return new MySqlHelperData(data: result.affected_rows > 0);
            }
            else
            {
                if (result.error is MySqlException &&
                    ((MySqlException)result.error).Number == 1062 //already exists telegram_id in database
                    )
                {
                    return new MySqlHelperData(data: false);
                }
                else
                {
                    return new MySqlHelperData(error: result.error);
                }
            }
        }
        public static MySqlHelperData DeleteUser(string telegram_id)
        {
            string query = "DELETE FROM users WHERE telegram_id = @telegram_id;";

            Dictionary<string, object> queryParameters = new();
            queryParameters.Add("@telegram_id", telegram_id);

            MySqlHelperResult result = mySql.Execute(query, false, queryParameters);

            if (result.error == null)
            {
                return new MySqlHelperData(data: result.affected_rows > 0);
            }
            else
            {
                return new MySqlHelperData(error: result.error);
            }
        }
    }
}