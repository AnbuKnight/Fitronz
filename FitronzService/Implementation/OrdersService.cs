using FitronzData;
using FitronzService.Interface;
using FitronzService.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Implementation
{
    public class OrdersService: IOrdersService
    {
        public OrdersService() { }
        private NpgsqlConnection _connection;
        public async Task<int> CreateOrder(Orders orderDetails)
        {
            int rowsImpacted = 0;
            using (_connection = DBConnection.CreateConnection())
            {
                try
                {
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    string commandText = "INSERT INTO orders (user_id,partner_id, slot_id, gymtype_id, persons, days, status, order_start_date, order_end_date, created_on, created_by, updated_on, updated_by) VALUES (@user_id,@partner_id, @slot_id, @gymtype_id, @persons, @days, @status, @order_start_date, @order_end_date, @created_on, @created_by, @updated_on, @updated_by)";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        cmd.Parameters.AddWithValue("user_id", orderDetails.user_id);
                        cmd.Parameters.AddWithValue("partner_id", orderDetails.partner_id);
                        cmd.Parameters.AddWithValue("slot_id", orderDetails.slot_id);
                        cmd.Parameters.AddWithValue("gymtype_id", orderDetails.gymtype_id);
                        cmd.Parameters.AddWithValue("persons", orderDetails.persons);
                        cmd.Parameters.AddWithValue("days", orderDetails.days);
                        cmd.Parameters.AddWithValue("status", orderDetails.status);
                        cmd.Parameters.AddWithValue("order_start_date", orderDetails.order_start_date);
                        cmd.Parameters.AddWithValue("order_end_date", orderDetails.order_end_date);
                        cmd.Parameters.AddWithValue("created_on", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("created_by", ((orderDetails.created_by != null && orderDetails.created_by != string.Empty) ? orderDetails.created_by : "System"));
                        cmd.Parameters.AddWithValue("updated_on", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("updated_by", ((orderDetails.updated_by != null && orderDetails.updated_by != string.Empty) ? orderDetails.updated_by : "System"));
                        

                        rowsImpacted = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at CreateOrder()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return rowsImpacted;
        }

        public async Task<GymDetails> GetGymDetailsForUsers(int partnerId)
        {
            GymDetails gymDetails=new GymDetails();
            List<SlotDetails> _slotDetails = new List<SlotDetails>();
            using (_connection = DBConnection.CreateConnection())
            {
                try
                {
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    string commandText = "select * from partners pt inner join gymdetails gt on pt.partner_id=gt.partner_id where pt.partner_id=@partner_id";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        List<GymTypeDetailz> gymTypeDetailsList = new List<GymTypeDetailz>();
                        cmd.Parameters.AddWithValue("partner_id", partnerId);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            int rowCount = 0;
                            int previousSlotId = 0;
                            while (reader.Read())
                            {
                                gymDetails.gym_name= Convert.ToString(reader["gym_name"]);
                                gymDetails.partner_id= Convert.ToInt32(reader["partner_id"]);
                                gymDetails.latitude = (float)Convert.ToDouble(reader["latitude"]);
                                gymDetails.longitude = (float)Convert.ToDouble(reader["longitude"]);
                                gymDetails.partner_id = Convert.ToInt32(reader["partner_id"]);
                                gymDetails.facilities = Convert.ToString(reader["facilities"]);
                                gymDetails.gym_address = Convert.ToString(reader["address"]);
                                gymDetails.gym_pincode = Convert.ToString(reader["pincode"]);
                                var tt = _slotDetails.FindIndex(x => x.slot_id == Convert.ToInt32(reader["slot_id"]));
                                if (tt >= 0)
                                {
                                    GymTypeDetailz gymTypeDetailz = new GymTypeDetailz();
                                    gymTypeDetailz.price = Convert.ToDecimal(reader["price"]);
                                    gymTypeDetailz.gymtype_id = Convert.ToInt32(reader["gymtype_id"]);
                                    //gymTypeDetailsList.Add(gymTypeDetailz);
                                    var ii = _slotDetails[tt].gym_type_details.ToList();
                                    ii.Add(gymTypeDetailz);
                                    _slotDetails[tt].gym_type_details = ii.ToArray();
                                }
                                else
                                {
                                    gymTypeDetailsList.Clear();
                                    SlotDetails _slotDetailsObject = new SlotDetails();
                                    _slotDetailsObject.slot_id = Convert.ToInt32(reader["slot_id"]);
                                    _slotDetailsObject.slot_start_time = Convert.ToDateTime(reader["slot_start_time"]);
                                    _slotDetailsObject.slot_end_time = Convert.ToDateTime(reader["slot_end_time"]);
                                    _slotDetailsObject.gender = Convert.ToString(reader["gender"]);

                                    GymTypeDetailz gymTypeDetailz = new GymTypeDetailz();
                                    gymTypeDetailz.price = Convert.ToDecimal(reader["price"]);
                                    gymTypeDetailz.gymtype_id = Convert.ToInt32(reader["gymtype_id"]);
                                    gymTypeDetailsList.Add(gymTypeDetailz);
                                    _slotDetailsObject.gym_type_details = gymTypeDetailsList.ToArray();

                                    _slotDetails.Add(_slotDetailsObject);
                                }
                                //previousSlotId = Convert.ToInt32(reader["slot_id"]);

                            }
                        }
                    }
                    gymDetails.slot_details = _slotDetails.ToArray();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at GetGymDetails()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return gymDetails;
        }


        public async Task<UserOrders> GetUserOrderDetails(int userid)
        {
            UserOrders userOrders = new UserOrders();
            userOrders.gymDetails=new GymDetails();
            using (_connection = DBConnection.CreateConnection())
            {
                try
                {
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    string commandText = "select * from orders os inner join gymdetails gt on os.partner_id=gt.partner_id inner join gymtype gty on gty.gymtype_id=gt.gymtype_id inner join partners pt on pt.partner_id=os.partner_id where os.user_id=1 and os.gymtype_id=gt.gymtype_id and os.slot_id=gt.slot_id";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        List<GymTypeDetailz> gymTypeDetailsList = new List<GymTypeDetailz>();
                        cmd.Parameters.AddWithValue("user_id", userid);
                        List<SlotDetails> _slotDetails = new List<SlotDetails>();
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userOrders.user_id = Convert.ToInt32(reader["user_id"]);                                
                                userOrders.gymDetails.gym_name = Convert.ToString(reader["gym_name"]);
                                userOrders.status = Convert.ToString(reader["status"]);
                                userOrders.gymDetails.partner_id = Convert.ToInt32(reader["partner_id"]);
                                userOrders.gymDetails.latitude = (float)Convert.ToDouble(reader["latitude"]);
                                userOrders.gymDetails.longitude = (float)Convert.ToDouble(reader["longitude"]);                                
                                userOrders.gymDetails.facilities = Convert.ToString(reader["facilities"]);
                                userOrders.gymDetails.gym_address = Convert.ToString(reader["address"]);
                                userOrders.gymDetails.gym_pincode = Convert.ToString(reader["pincode"]);
                                userOrders.order_start_date = Convert.ToDateTime(reader["order_start_date"]);
                                userOrders.order_end_date = Convert.ToDateTime(reader["order_end_date"]);
                                userOrders.persons= Convert.ToInt32(reader["persons"]);
                                userOrders.days = Convert.ToInt32(reader["days"]);

                                var tt = _slotDetails.FindIndex(x => x.slot_id == Convert.ToInt32(reader["slot_id"]));
                                if (tt >= 0)
                                {
                                    GymTypeDetailz gymTypeDetailz = new GymTypeDetailz();
                                    gymTypeDetailz.price = Convert.ToDecimal(reader["price"]);
                                    gymTypeDetailz.gymtype_id = Convert.ToInt32(reader["gymtype_id"]);
                                    //gymTypeDetailsList.Add(gymTypeDetailz);
                                    var ii = _slotDetails[tt].gym_type_details.ToList();
                                    ii.Add(gymTypeDetailz);
                                    _slotDetails[tt].gym_type_details = ii.ToArray();
                                }
                                else
                                {
                                    gymTypeDetailsList.Clear();
                                    SlotDetails _slotDetailsObject = new SlotDetails();
                                    _slotDetailsObject.slot_id = Convert.ToInt32(reader["slot_id"]);
                                    _slotDetailsObject.slot_start_time = Convert.ToDateTime(reader["slot_start_time"]);
                                    _slotDetailsObject.slot_end_time = Convert.ToDateTime(reader["slot_end_time"]);
                                    _slotDetailsObject.gender = Convert.ToString(reader["gender"]);

                                    GymTypeDetailz gymTypeDetailz = new GymTypeDetailz();
                                    gymTypeDetailz.price = Convert.ToDecimal(reader["price"]);
                                    gymTypeDetailz.gymtype_id = Convert.ToInt32(reader["gymtype_id"]);
                                    gymTypeDetailsList.Add(gymTypeDetailz);
                                    _slotDetailsObject.gym_type_details = gymTypeDetailsList.ToArray();

                                    _slotDetails.Add(_slotDetailsObject);
                                }
                            }
                        }
                        userOrders.gymDetails.slot_details = _slotDetails.ToArray();
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
            return userOrders;
        }
    }
}
