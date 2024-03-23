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
            NpgsqlConnection npgsqlConnection = new NpgsqlConnection("Host=34.132.187.39:5432;Username=postgres;Password=FitronZ*555*;Database=fitronz");                                   
            return npgsqlConnection;
        }
    }
}
