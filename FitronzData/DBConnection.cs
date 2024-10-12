using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzData
{
    public static class DBConnection
    {
        //public DBConnection()
        //{

        //}

        public static NpgsqlConnection CreateConnection()
        {
            NpgsqlConnection npgsqlConnection = new NpgsqlConnection("Host=194.238.18.190:5432;Username=fitronzdbuser;Password=FitronZ*555*;Database=fitronz");                                   
            return npgsqlConnection;
        }
    }
}
