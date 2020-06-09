using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SignalRMvcChat.Data;

namespace SignalRMvcChat.Models
{
    public class SqlDataContextBase<K, V>
    {
        public const string CONNECTION_STRING = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SignalRChat;Trusted_Connection=True;";

        protected DbEntityInfo EntityInfo { get; set; }

        protected SqlDataContextBase(DbEntityInfo info)
        {
            EntityInfo = info;
        }



    }
}
