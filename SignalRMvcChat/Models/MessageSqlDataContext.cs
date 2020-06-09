using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SignalRMvcChat.Data;

namespace SignalRMvcChat.Models
{
    public class MessageSqlDataContext : SqlDataContextBase<int, Message>
    {
        public static MessageSqlDataContext Instance = new MessageSqlDataContext();

        public MessageSqlDataContext() : base(
            new DbEntityInfo(
                new[] { new KeyColumn { Name = "Id", Type = DbType.Int32 } },
                true))
        {
        }
        public void InsertMessageInDb(string userName,string to, string message)
        {
            int idFrom;
            int idTo;

            using (Microsoft.Data.SqlClient.SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                var queryForGetIdFrom = string.Format(
                    @"SELECT Id FROM Users WHERE Email = '{0}'", userName);
                var queryForGetIdTo = string.Format(
                    @"SELECT Id FROM Users WHERE Email = '{0}'", to);

                var getFromCommand = new SqlCommand(queryForGetIdFrom, connection);
                idFrom = (int)getFromCommand.ExecuteScalar();
                var getToCommand = new SqlCommand(queryForGetIdTo, connection);
                idTo = (int)getToCommand.ExecuteScalar();

                var queryForInsert = string.Format(
                    @"INSERT INTO Messages (IdFrom, IdTo, Text) VALUES ({0})", String.Concat(idFrom, ", ", idTo, " ,", "'", message, "'")
                );

                var insertCommand = new SqlCommand(queryForInsert, connection);
                insertCommand.ExecuteNonQuery();
            }
        }
    }
}
