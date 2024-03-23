using FitronzData;
using FitronzService.Interface;
using FitronzService.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Implementation
{
    public class RegisterService: IRegisterService
    {
        public RegisterService()
        {

        }

        private NpgsqlConnection _connection;
        public async Task<int> AddPartner(Partner partnerDetails)
        {
            int rowsImpacted = 0;
            using(_connection= DBConnection.CreateConnection())
            {
                try
                {
                    if(_connection.State!=System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    string commandText = "INSERT INTO partners (owner_name,gym_name, email_address, password, mobile_number, address, pincode, latitude, longitude, created_on, created_by, updated_on, updated_by, facilities) VALUES (@owner_name,@gym_name, @email_address, @password, @mobile_number, @address, @pincode, @latitude, @longitude, @created_on, @created_by, @updated_on, @updated_by, @facilities)";
                    await using(var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        cmd.Parameters.AddWithValue("owner_name", partnerDetails.owner_name);
                        cmd.Parameters.AddWithValue("gym_name", partnerDetails.gym_name);
                        cmd.Parameters.AddWithValue("email_address", partnerDetails.email_address);
                        cmd.Parameters.AddWithValue("password", partnerDetails.password);
                        cmd.Parameters.AddWithValue("mobile_number", partnerDetails.mobile_number);
                        cmd.Parameters.AddWithValue("address", partnerDetails.address);
                        cmd.Parameters.AddWithValue("pincode", partnerDetails.pincode);
                        cmd.Parameters.AddWithValue("latitude", partnerDetails.latitude);
                        cmd.Parameters.AddWithValue("longitude", partnerDetails.longitude);
                        cmd.Parameters.AddWithValue("created_on", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("created_by", ((partnerDetails.created_by!=null && partnerDetails.created_by != string.Empty) ? partnerDetails.created_by:"System"));
                        cmd.Parameters.AddWithValue("updated_on", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("updated_by", ((partnerDetails.updated_by != null && partnerDetails.updated_by != string.Empty) ? partnerDetails.updated_by : "System"));
                        cmd.Parameters.AddWithValue("facilities", partnerDetails.facilities);

                        rowsImpacted = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception at AddPartner()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return rowsImpacted;
        }

        public async Task<Partner> GetPartnerDetails(string mobileNumber, string password)
        {
            Partner partner = new Partner();
            using (_connection = DBConnection.CreateConnection())
            {
                try
                {
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    string commandText = "select * from partners where mobile_number=@mobile_number and password=@password";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        cmd.Parameters.AddWithValue("mobile_number", mobileNumber);
                        cmd.Parameters.AddWithValue("password", password);
                        using (NpgsqlDataReader reader=cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                partner.partner_id= Convert.ToInt32(reader["partner_id"]);
                                partner.owner_name =Convert.ToString(reader["owner_name"]);
                                partner.gym_name = Convert.ToString(reader["gym_name"]);
                                partner.email_address = Convert.ToString(reader["email_address"]);
                                partner.mobile_number = Convert.ToString(reader["mobile_number"]);
                                partner.address = Convert.ToString(reader["address"]);
                                partner.pincode = Convert.ToString(reader["pincode"]);
                                partner.latitude = (float)((reader["latitude"]!=DBNull.Value)?(Convert.ToDouble(reader["latitude"])):(0));
                                partner.longitude = (float)((reader["longitude"] != DBNull.Value) ? (Convert.ToDouble(reader["longitude"])) : (0));
                                partner.created_on = Convert.ToDateTime(reader["created_on"]);
                                partner.created_by = Convert.ToString(reader["created_by"]);
                                partner.updated_on = Convert.ToDateTime(reader["updated_on"]);
                                partner.updated_by = Convert.ToString(reader["updated_by"]);
                                partner.is_activated= Convert.ToBoolean(reader["is_activated"]);
                                partner.facilities = Convert.ToString(reader["facilities"]);
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
            return partner;
        }

        public async Task<int> UpsertGymDetails(GymDetails gymDetails)
        {

            int rowsImpacted = 0;
            int previousSlotID = 0;
            using (_connection = DBConnection.CreateConnection())
            {
                try
                {
                    foreach(var slotDetails in gymDetails.slot_details)
                    {
                        
                        if (slotDetails.slot_id==0)
                        {
                            slotDetails.slot_id = previousSlotID + 1;
                            previousSlotID = slotDetails.slot_id;
                        }
                        else
                        {
                            previousSlotID = slotDetails.slot_id;
                        }
                    }
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }                    
                    string commandText = "CALL upsert_gym_details($1, $2, $3, $4)";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {                       
                        cmd.Parameters.Add(new()
                        {
                            Value = gymDetails.partner_id
                        });
                        cmd.Parameters.Add(new()
                        {
                            Value = gymDetails.slot_details
                        });
                        cmd.Parameters.Add(new()
                        {
                            Value = ((gymDetails.created_by != null && gymDetails.created_by != string.Empty) ? gymDetails.created_by : "System")
                        });
                        cmd.Parameters.Add(new()
                        {
                            Value = ((gymDetails.updated_by != null && gymDetails.updated_by != string.Empty) ? gymDetails.updated_by : "System")
                        });                        

                        rowsImpacted = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at UpsertGymDetails()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            //rowsImpacted will return -1 because SET NOCOUNT OFF is not set in Stored procedure
            return rowsImpacted;
        }

        public async Task<List<SlotDetails>> GetGymDetails(int partnerId)
        {   
            List<SlotDetails> _slotDetails = new List<SlotDetails>();
            using (_connection = DBConnection.CreateConnection())
            {
                try
                {
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    string commandText = "select * from gymdetails where partner_id=@partner_id";
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
                                var tt= _slotDetails.FindIndex(x => x.slot_id== Convert.ToInt32(reader["slot_id"]));
                                if(tt>=0)
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at GetGymDetails()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return _slotDetails;
        }

        public async Task<int> DeleteSlot(int partnerId, int slotId)
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
                    string commandText = "delete from gymdetails where partner_id=@partner_id and slot_id=@slot_id";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        cmd.Parameters.AddWithValue("partner_id", partnerId);
                        cmd.Parameters.AddWithValue("slot_id", slotId);                        

                        rowsImpacted = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at DeleteSlot()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return rowsImpacted;
        }

        public async Task<int> AddUser(Users userDetails)
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
                    string commandText = "INSERT INTO users (user_name,user_age, user_gender, user_address, user_pincode, user_mobile_number, user_email_address, user_password, user_profile_picture, created_on, created_by, updated_on, updated_by) VALUES (@user_name,@user_age, @user_gender, @user_address, @user_pincode, @user_mobile_number, @user_email_address, @user_password, @user_profile_picture, @created_on, @created_by, @updated_on, @updated_by)";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        cmd.Parameters.AddWithValue("user_name", userDetails.user_name);
                        cmd.Parameters.AddWithValue("user_age", userDetails.user_age);
                        cmd.Parameters.AddWithValue("user_gender", userDetails.user_gender);
                        cmd.Parameters.AddWithValue("user_address", userDetails.user_address);
                        cmd.Parameters.AddWithValue("user_pincode", userDetails.user_pincode);
                        cmd.Parameters.AddWithValue("user_mobile_number", userDetails.user_mobile_number);
                        cmd.Parameters.AddWithValue("user_email_address", userDetails.user_email_address);
                        cmd.Parameters.AddWithValue("user_password", userDetails.user_password);
                        cmd.Parameters.AddWithValue("user_profile_picture", userDetails.user_profile_picture);
                        cmd.Parameters.AddWithValue("created_on", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("created_by", ((userDetails.created_by != null && userDetails.created_by != string.Empty) ? userDetails.created_by : "System"));
                        cmd.Parameters.AddWithValue("updated_on", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("updated_by", ((userDetails.updated_by != null && userDetails.updated_by != string.Empty) ? userDetails.updated_by : "System"));                        

                        rowsImpacted = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at AddUser()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return rowsImpacted;
        }

        public async Task<Users> GetUserDetails(string mobileNumber, string emailAddress, string password)
        {
            Users user=new Users();
            string commandText = string.Empty;
            using (_connection = DBConnection.CreateConnection())
            {
                try
                {
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    if(emailAddress!=string.Empty && emailAddress!=null)
                    {
                        commandText = "select * from users where user_email_address=@user_email_address and user_password=@user_password";
                    }
                    else
                    {
                        commandText = "select * from users where user_mobile_number=@user_mobile_number and user_password=@user_password";
                    }
                    
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        if (emailAddress != string.Empty && emailAddress != null)
                        {
                            cmd.Parameters.AddWithValue("user_email_address", emailAddress);
                            cmd.Parameters.AddWithValue("user_password", password);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("user_mobile_number", mobileNumber);
                            cmd.Parameters.AddWithValue("user_password", password);
                        }

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user.user_id = Convert.ToInt32(reader["user_id"]);
                                user.user_name = Convert.ToString(reader["user_name"]);
                                user.user_age = Convert.ToInt32(reader["user_age"]);
                                user.user_gender = Convert.ToString(reader["user_gender"]);
                                user.user_address = Convert.ToString(reader["user_address"]);
                                user.user_pincode = Convert.ToString(reader["user_pincode"]);
                                user.user_email_address = Convert.ToString(reader["user_email_address"]);
                                user.user_mobile_number = Convert.ToString(reader["user_mobile_number"]);
                                user.user_password = Convert.ToString(reader["user_password"]);
                                user.user_profile_picture = Convert.ToString(reader["user_profile_picture"]);
                                user.created_on = Convert.ToDateTime(reader["created_on"]);
                                user.created_by = Convert.ToString(reader["created_by"]);
                                user.updated_on = Convert.ToDateTime(reader["updated_on"]);
                                user.updated_by = Convert.ToString(reader["updated_by"]);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at GetUserDetails()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return user;
        }
    }
}
