using FitronzService.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitronzData;
using FitronzService.Interface;
using Irony.Parsing;

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

        public async Task<List<GymDetails>> GetGymListWithSlotDetails()
        {
            List<SlotDetails> _slotDetailsList = new List<SlotDetails>();
            List<GymDetails> _gymDetailsList = new List<GymDetails>();

            using (_connection = DBConnection.CreateConnection())
            {
                try
                {
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    string commandText = "select * from partners pt left join gymdetails gt on pt.partner_id=gt.partner_id";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        List<GymTypeDetailz> gymTypeDetailsList = new List<GymTypeDetailz>();                        
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            int rowCount = 0;
                            int previousSlotId = 0;
                            while (reader.Read())
                            {
                                var tt = _gymDetailsList.FindIndex(x => x.partner_id == Convert.ToInt32(reader["partner_id"]));
                                if (tt >= 0)
                                {
                                    var qq = _gymDetailsList[tt].slot_details.ToList().FindIndex(x => x.slot_id == Convert.ToInt32(reader["slot_id"]));
                                    if(qq >= 0)
                                    {
                                        GymTypeDetailz gymTypeDetailsObject = new GymTypeDetailz();
                                        gymTypeDetailsObject.price = Convert.ToDecimal(reader["price"]);
                                        gymTypeDetailsObject.gymtype_id = Convert.ToInt32(reader["gymtype_id"]);

                                        var bb = _gymDetailsList[tt].slot_details.ToList();
                                        var cc=bb[qq].gym_type_details.ToList();
                                        cc.Add(gymTypeDetailsObject);

                                        _gymDetailsList[tt].slot_details[qq].gym_type_details=cc.ToArray();
                                    }
                                    else
                                    {
                                        gymTypeDetailsList.Clear();

                                        SlotDetails _slotDetailsObject = new SlotDetails();
                                        _slotDetailsObject.slot_id = Convert.ToInt32(reader["slot_id"]);
                                        _slotDetailsObject.slot_start_time = Convert.ToDateTime(reader["slot_start_time"]);
                                        _slotDetailsObject.slot_end_time = Convert.ToDateTime(reader["slot_end_time"]);
                                        _slotDetailsObject.gender = Convert.ToString(reader["gender"]);

                                        GymTypeDetailz gymTypeDetailsObject = new GymTypeDetailz();
                                        gymTypeDetailsObject.price = Convert.ToDecimal(reader["price"]);
                                        gymTypeDetailsObject.gymtype_id = Convert.ToInt32(reader["gymtype_id"]);
                                        gymTypeDetailsList.Add(gymTypeDetailsObject);
                                        _slotDetailsObject.gym_type_details = gymTypeDetailsList.ToArray();

                                        var ff = _gymDetailsList[tt].slot_details.ToList();
                                        ff.Add(_slotDetailsObject);
                                        _gymDetailsList[tt].slot_details= ff.ToArray();
                                    }

                                }
                                else
                                {
                                    gymTypeDetailsList.Clear();
                                    _slotDetailsList.Clear();

                                    GymDetails _gymDetailsObject = new GymDetails();
                                    _gymDetailsObject.partner_id= Convert.ToInt32(reader["partner_id"]);
                                    _gymDetailsObject.owner_name = Convert.ToString(reader["owner_name"]);
                                    _gymDetailsObject.gym_name = Convert.ToString(reader["gym_name"]);
                                    _gymDetailsObject.latitude = (float)((reader["latitude"] != DBNull.Value) ? (Convert.ToDouble(reader["latitude"])) : (0));
                                    _gymDetailsObject.longitude = (float)((reader["longitude"] != DBNull.Value) ? (Convert.ToDouble(reader["longitude"])) : (0));
                                    _gymDetailsObject.is_activated = Convert.ToBoolean(reader["is_activated"]);

                                    SlotDetails _slotDetailsObject = new SlotDetails();
                                    _slotDetailsObject.slot_id = Convert.ToInt32(reader["slot_id"]);
                                    _slotDetailsObject.slot_start_time = Convert.ToDateTime(reader["slot_start_time"]);
                                    _slotDetailsObject.slot_end_time = Convert.ToDateTime(reader["slot_end_time"]);
                                    _slotDetailsObject.gender = Convert.ToString(reader["gender"]);

                                    GymTypeDetailz gymTypeDetailsObject = new GymTypeDetailz();
                                    gymTypeDetailsObject.price = Convert.ToDecimal(reader["price"]);
                                    gymTypeDetailsObject.gymtype_id = Convert.ToInt32(reader["gymtype_id"]);
                                    gymTypeDetailsList.Add(gymTypeDetailsObject);
                                    _slotDetailsObject.gym_type_details = gymTypeDetailsList.ToArray();

                                    _slotDetailsList.Add(_slotDetailsObject);
                                    _gymDetailsObject.slot_details = _slotDetailsList.ToArray();

                                    _gymDetailsList.Add(_gymDetailsObject);
                                }
                                //previousSlotId = Convert.ToInt32(reader["slot_id"]);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at GetGymDetails()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return _gymDetailsList;
        }
    }
}
