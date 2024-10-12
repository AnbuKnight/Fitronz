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
    public class PaymentService: IPaymentService
    {
        public PaymentService() { }
        private NpgsqlConnection _connection;
        public async Task<int> AddPaymentInfo(Payment paymentDetails)
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
                    string commandText = "INSERT INTO payment_info (partner_id,account_number, account_holder_name, bank_name, ifsc_code, created_on, created_by, updated_on, updated_by) VALUES (@partner_id,@account_number, @account_holder_name, @bank_name, @ifsc_code, @created_on, @created_by, @updated_on, @updated_by)";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        cmd.Parameters.AddWithValue("partner_id", paymentDetails.partner_id);
                        cmd.Parameters.AddWithValue("account_number", paymentDetails.account_number);
                        cmd.Parameters.AddWithValue("account_holder_name", paymentDetails.account_holder_name);
                        cmd.Parameters.AddWithValue("bank_name", paymentDetails.bank_name);
                        cmd.Parameters.AddWithValue("ifsc_code", paymentDetails.ifsc_code);
                        cmd.Parameters.AddWithValue("created_on", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("created_by", ((paymentDetails.created_by != null && paymentDetails.created_by != string.Empty) ? paymentDetails.created_by : "System"));
                        cmd.Parameters.AddWithValue("updated_on", DateTime.Now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("updated_by", ((paymentDetails.updated_by != null && paymentDetails.updated_by != string.Empty) ? paymentDetails.updated_by : "System"));                        

                        rowsImpacted = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at AddPaymentInfo()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return rowsImpacted;
        }
    }
}
