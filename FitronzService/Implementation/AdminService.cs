using FitronzService.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitronzData;
using FitronzService.Interface;
using System.Data;
using NpgsqlTypes;

namespace FitronzService.Implementation
{
    public class AdminService: IAdminService
    {
        private NpgsqlConnection _connection;

        public async Task<int> ApproveOrRejectPartner(Partner partnerDetails)
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

                    using (var command = new NpgsqlCommand("approve_reject_partner_information", _connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Adding parameters with specified data types
                        command.Parameters.Add(new NpgsqlParameter("owner_name", NpgsqlDbType.Text) { Value = partnerDetails.owner_name });
                        command.Parameters.Add(new NpgsqlParameter("gym_name", NpgsqlDbType.Text) { Value = partnerDetails.gym_name });
                        command.Parameters.Add(new NpgsqlParameter("email_address", NpgsqlDbType.Text) { Value = partnerDetails.email_address });
                        command.Parameters.Add(new NpgsqlParameter("password", NpgsqlDbType.Text) { Value = partnerDetails.password });
                        command.Parameters.Add(new NpgsqlParameter("mobile_number", NpgsqlDbType.Text) { Value = partnerDetails.mobile_number });
                        command.Parameters.Add(new NpgsqlParameter("address", NpgsqlDbType.Text) { Value = partnerDetails.address });
                        command.Parameters.Add(new NpgsqlParameter("pincode", NpgsqlDbType.Text) { Value = partnerDetails.pincode });
                        command.Parameters.Add(new NpgsqlParameter("latitude", NpgsqlDbType.Numeric) { Value = partnerDetails.latitude });
                        command.Parameters.Add(new NpgsqlParameter("longitude", NpgsqlDbType.Numeric) { Value = partnerDetails.longitude });
                        command.Parameters.Add(new NpgsqlParameter("facilities", NpgsqlDbType.Text) { Value = partnerDetails.facilities });
                        command.Parameters.Add(new NpgsqlParameter("created_by", NpgsqlDbType.Text) { Value = partnerDetails.created_by });
                        command.Parameters.Add(new NpgsqlParameter("updated_by", NpgsqlDbType.Text) { Value = partnerDetails.updated_by });
                        command.Parameters.Add(new NpgsqlParameter("admin_action", NpgsqlDbType.Text) { Value = partnerDetails.admin_action });
                        command.Parameters.Add(new NpgsqlParameter("files_to_be_deleted", NpgsqlDbType.Text) { Value = (partnerDetails.files_to_be_deleted!=null? partnerDetails.files_to_be_deleted:string.Empty) });

                        rowsImpacted = command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    rowsImpacted = 99;
                    Console.WriteLine("Exception at ApproveOrRejectPartner()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return rowsImpacted;
        }
    }
}
