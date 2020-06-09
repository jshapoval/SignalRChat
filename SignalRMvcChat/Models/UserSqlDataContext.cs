using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SignalRMvcChat.Data;

namespace SignalRMvcChat.Models
{
    public class UserSqlDataContext : SqlDataContextBase<int, User>
    {
        public static UserSqlDataContext Instance = new UserSqlDataContext();

        public UserSqlDataContext() : base(
            new DbEntityInfo(
                new[] { new KeyColumn { Name = "Id", Type = DbType.Int32 } },
                true))
        {
        }
        public string CheckAndInsertUserInDb(string userName,string password)
        {
            string resultMessage = null;
            using (Microsoft.Data.SqlClient.SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                var queryForCheck = string.Format(
                    @"SELECT * FROM Users WHERE Email = '{0}'
                     set @ReturnValue = @@rowCount;", userName);

                var command = new SqlCommand(queryForCheck, connection);

                var returnValue = command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                returnValue.Direction = ParameterDirection.InputOutput;
                returnValue.Value = -1;
                var result = (int)command.Parameters["@ReturnValue"].Value;
                if (result > 0)
                {
                    resultMessage = "please, input another login";
                }

                else
                {
                    var queryForInsert = string.Format(
                        @"INSERT INTO Users (Email, Password) VALUES ({0})", String.Concat("'", userName, "'", ", ", "'", password, "'")
                    );

                    var insertCommand = new SqlCommand(queryForInsert, connection);
                    insertCommand.ExecuteNonQuery();
                    resultMessage = "Successfully!";
                }

                return resultMessage;
            }
        }
    }
}
