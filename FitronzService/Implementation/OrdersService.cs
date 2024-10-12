using DocumentFormat.OpenXml.Office.Word;
using FitronzData;
using FitronzService.Interface;
using FitronzService.Models;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Implementation
{
    public class OrdersService: IOrdersService
    {
        public OrdersService() { }
        private NpgsqlConnection _connection;
        //NpgsqlConnection.GlobalTypeMapper.MapComposite<UserOrderSlotDetails>("user_order_slot_details");
        public async Task<int> CreateOrder(Orders orderDetails)
        {
            int rowsImpacted = 0;
            //var slotDetailsArray = orderDetails.user_slot_details.ToArray();
            var slotDetailsJson = JsonConvert.SerializeObject(orderDetails.user_slot_details);

            using (_connection = DBConnection.CreateConnection())
            {
                try
                {                    
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }

                    using (var command = new NpgsqlCommand("create_orders", _connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Adding parameters with specified data types
                        command.Parameters.Add(new NpgsqlParameter("user_id", NpgsqlDbType.Integer) { Value = orderDetails.user_id });
                        command.Parameters.Add(new NpgsqlParameter("partner_id", NpgsqlDbType.Integer) { Value = orderDetails.partner_id });
                        command.Parameters.Add(new NpgsqlParameter("persons", NpgsqlDbType.Integer) { Value = orderDetails.persons });
                        command.Parameters.Add(new NpgsqlParameter("days", NpgsqlDbType.Integer) { Value = orderDetails.days });
                        command.Parameters.Add(new NpgsqlParameter("status", NpgsqlDbType.Text) { Value = orderDetails.status });
                        command.Parameters.Add(new NpgsqlParameter("user_slot_details", NpgsqlDbType.Json) { Value = slotDetailsJson });
                        command.Parameters.Add(new NpgsqlParameter("order_start_date", NpgsqlDbType.TimestampTz) { Value = orderDetails.order_start_date });
                        command.Parameters.Add(new NpgsqlParameter("order_end_date", NpgsqlDbType.TimestampTz) { Value = orderDetails.order_end_date });                        
                        command.Parameters.Add(new NpgsqlParameter("created_by", NpgsqlDbType.Text) { Value = orderDetails.created_by });
                        command.Parameters.Add(new NpgsqlParameter("updated_by", NpgsqlDbType.Text) { Value = orderDetails.updated_by });

                        rowsImpacted = command.ExecuteNonQuery();
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
                    string commandText = "select * from orders os inner join gymdetails gt on os.partner_id=gt.partner_id inner join gymtype gty on gty.gymtype_id=gt.gymtype_id inner join partners pt on pt.partner_id=os.partner_id where os.user_id=@user_id and os.gymtype_id=gt.gymtype_id";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        List<GymTypeDetailz> gymTypeDetailsList = new List<GymTypeDetailz>();
                        cmd.Parameters.AddWithValue("user_id", userid);
                        List<SlotDetails> _slotDetails = new List<SlotDetails>();
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userOrders.order_id = Convert.ToInt32(reader["order_id"]);
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

        public async Task<List<UserOrders>> GetOrderDetails()
        {
            List<UserOrders> userOrdersList = new List<UserOrders>();
            GymDetails gymDetails = new GymDetails();
            //userOrders.gymDetails = new GymDetails();
            using (_connection = DBConnection.CreateConnection())
            {
                try
                {
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    string commandText = "select * from orders os inner join gymdetails gt on os.partner_id=gt.partner_id inner join gymtype gty on gty.gymtype_id=gt.gymtype_id inner join partners pt on pt.partner_id=os.partner_id inner join users us on us.user_id=os.user_id where os.gymtype_id=gt.gymtype_id order by os.order_id, os.slot_id, os.gymtype_id;";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        List<GymTypeDetailz> gymTypeDetailsList = new List<GymTypeDetailz>();
                        //cmd.Parameters.AddWithValue("user_id", userid);
                        List<SlotDetails> _slotDetails = new List<SlotDetails>();
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var vv = userOrdersList.FindIndex(x => x.order_id == Convert.ToInt32(reader["order_id"]));
                                if (vv >= 0)
                                {
                                    var tt = _slotDetails.FindIndex(x => x.slot_id == Convert.ToInt32(reader["slot_id"]));
                                    if (tt >= 0)
                                    {
                                        GymTypeDetailz gymTypeDetailz = new GymTypeDetailz();
                                        gymTypeDetailz.price = Convert.ToDecimal(reader["price"]);
                                        gymTypeDetailz.gymtype_id = Convert.ToInt32(reader["gymtype_id"]);
                                        gymTypeDetailz.gymtype = Convert.ToString(reader["gymtype"]);
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
                                        gymTypeDetailz.gymtype = Convert.ToString(reader["gymtype"]);
                                        gymTypeDetailsList.Add(gymTypeDetailz);
                                        _slotDetailsObject.gym_type_details = gymTypeDetailsList.ToArray();

                                        _slotDetails.Add(_slotDetailsObject);
                                    }
                                    userOrdersList[vv].gymDetails.slot_details = _slotDetails.ToArray();
                                }
                                else
                                {
                                    _slotDetails.Clear();
                                    UserOrders userOrders = new UserOrders();
                                    userOrders.gymDetails = new GymDetails();
                                    userOrders.order_id = Convert.ToInt32(reader["order_id"]);
                                    userOrders.user_age = Convert.ToInt32(reader["user_age"]);
                                    userOrders.user_name = Convert.ToString(reader["user_name"]);
                                    userOrders.user_gender = Convert.ToString(reader["user_gender"]);
                                    userOrders.user_id = Convert.ToInt32(reader["user_id"]);
                                    userOrders.gymDetails.gym_name = Convert.ToString(reader["gym_name"]);
                                    userOrders.status = Convert.ToString(reader["status"]);
                                    userOrders.gymDetails.partner_id = Convert.ToInt32(reader["partner_id"]);
                                    
                                    userOrders.gymDetails.owner_name = Convert.ToString(reader["owner_name"]);
                                    userOrders.gymDetails.mobile_number = Convert.ToString(reader["mobile_number"]);
                                    userOrders.gymDetails.email_address = Convert.ToString(reader["email_address"]);

                                    userOrders.gymDetails.latitude = (float)Convert.ToDouble(reader["latitude"]);
                                    userOrders.gymDetails.longitude = (float)Convert.ToDouble(reader["longitude"]);
                                    userOrders.gymDetails.facilities = Convert.ToString(reader["facilities"]);
                                    userOrders.gymDetails.gym_address = Convert.ToString(reader["address"]);
                                    userOrders.gymDetails.gym_pincode = Convert.ToString(reader["pincode"]);
                                    userOrders.order_start_date = Convert.ToDateTime(reader["order_start_date"]);
                                    userOrders.order_end_date = Convert.ToDateTime(reader["order_end_date"]);
                                    userOrders.persons = Convert.ToInt32(reader["persons"]);
                                    userOrders.days = Convert.ToInt32(reader["days"]);


                                    var tt = _slotDetails.FindIndex(x => x.slot_id == Convert.ToInt32(reader["slot_id"]));
                                    if (tt >= 0)
                                    {
                                        GymTypeDetailz gymTypeDetailz = new GymTypeDetailz();
                                        gymTypeDetailz.price = Convert.ToDecimal(reader["price"]);
                                        gymTypeDetailz.gymtype_id = Convert.ToInt32(reader["gymtype_id"]);
                                        gymTypeDetailz.gymtype = Convert.ToString(reader["gymtype"]);
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
                                        gymTypeDetailz.gymtype = Convert.ToString(reader["gymtype"]);
                                        gymTypeDetailsList.Add(gymTypeDetailz);
                                        _slotDetailsObject.gym_type_details = gymTypeDetailsList.ToArray();

                                        _slotDetails.Add(_slotDetailsObject);
                                    }
                                    userOrders.gymDetails.slot_details = _slotDetails.ToArray();
                                    userOrdersList.Add(userOrders);
                                }
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
            return userOrdersList;
        }

        public async Task<int> UpdateOrderStatus(OrderStatus orderStatus)
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
                    string commandText = "Update orders set status=@status, updated_on=@updated_on, updated_by=@updated_by where order_id=@order_id and partner_id=@partner_id";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        cmd.Parameters.AddWithValue("order_id", orderStatus.order_id);
                        cmd.Parameters.AddWithValue("partner_id", orderStatus.partner_id);
                        cmd.Parameters.AddWithValue("status", orderStatus.status);                        
                        cmd.Parameters.AddWithValue("updated_on", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("updated_by", ((orderStatus.updated_by != null && orderStatus.updated_by != string.Empty) ? orderStatus.updated_by : "System"));

                        rowsImpacted = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at UpdateOrderStatus()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return rowsImpacted;
        }

        public async Task<int> UpdateDaysandStatusForUserCheckin(OrderStatus orderStatus)
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
                    string commandText = "Update orders set days=days-1, status=@status, updated_on=@updated_on, updated_by=@updated_by where order_id=@order_id and partner_id=@partner_id and user_id=@user_id and days<>0";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        cmd.Parameters.AddWithValue("order_id", orderStatus.order_id);
                        cmd.Parameters.AddWithValue("partner_id", orderStatus.partner_id);
                        cmd.Parameters.AddWithValue("user_id", orderStatus.user_id);
                        cmd.Parameters.AddWithValue("status", orderStatus.status);
                        cmd.Parameters.AddWithValue("updated_on", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("updated_by", ((orderStatus.updated_by != null && orderStatus.updated_by != string.Empty) ? orderStatus.updated_by : "System"));

                        rowsImpacted = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at UpdateDaysandStatusForUserCheckin()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return rowsImpacted;
        }
    }
}
