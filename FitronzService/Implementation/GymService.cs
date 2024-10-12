using FitronzService.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitronzData;
using FitronzService.Interface;

namespace FitronzService.Implementation
{
    public class GymService: IGymService
    {
        public GymService() { }
        private NpgsqlConnection _connection;
        public async Task<List<Partner>> GetGymList()
        {
            List<Partner> partners = new List<Partner>();
            using (_connection = DBConnection.CreateConnection())
            {
                try
                {                    
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    string commandText = "select partner_id, gym_name, owner_name, latitude, longitude, is_activated from partners;";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Partner partner = new Partner();
                                partner.partner_id = Convert.ToInt32(reader["partner_id"]);
                                partner.owner_name = Convert.ToString(reader["owner_name"]);
                                partner.gym_name = Convert.ToString(reader["gym_name"]);                               
                                partner.latitude = (float)((reader["latitude"] != DBNull.Value) ? (Convert.ToDouble(reader["latitude"])) : (0));
                                partner.longitude = (float)((reader["longitude"] != DBNull.Value) ? (Convert.ToDouble(reader["longitude"])) : (0));                               
                                partner.is_activated = Convert.ToBoolean(reader["is_activated"]);                                
                                partners.Add(partner);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at GetPartnerDetails()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return partners;
        }
    }
}
