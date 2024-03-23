using FitronzData;
using FitronzService.Interface;
using FitronzService.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Implementation
{
    public class FileService : IFileService
    {
        public FileService() { }
        private NpgsqlConnection _connection;

        public async Task<int> AddFileDetailsToDB(Files fileDetails)
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
                    string commandText = "CALL insert_file_details(:partnerid, :emailaddress, :filename, :filelink, :filedescription, :createdby, :updatedby)";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        cmd.Parameters.AddWithValue("partnerid", NpgsqlDbType.Integer, fileDetails.partner_id);
                        cmd.Parameters.AddWithValue("emailaddress", NpgsqlDbType.Varchar, fileDetails.email_address);
                        cmd.Parameters.AddWithValue("filename", NpgsqlDbType.Varchar, fileDetails.partner_id+"_"+fileDetails.file_name);
                        cmd.Parameters.AddWithValue("filelink", NpgsqlDbType.Varchar, fileDetails.file_link);
                        cmd.Parameters.AddWithValue("filedescription", NpgsqlDbType.Varchar, fileDetails.file_description);                        
                        cmd.Parameters.AddWithValue("createdby", NpgsqlDbType.Varchar,((fileDetails.created_by != null && fileDetails.created_by != string.Empty) ? fileDetails.created_by : "System"));                        
                        cmd.Parameters.AddWithValue("updatedby", NpgsqlDbType.Varchar,((fileDetails.updated_by != null && fileDetails.updated_by != string.Empty) ? fileDetails.updated_by : "System"));

                        rowsImpacted = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at AddFileDetailsToDB()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return rowsImpacted;
        }

        public async Task<List<Files>> GetFileDetails(int partnerId)
        {
            List<Files> _files = new List<Files>();
            using (_connection = DBConnection.CreateConnection())
            {
                try
                {
                    if (_connection.State != System.Data.ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    string commandText = "select * from files where partner_id=@partner_id";
                    await using (var cmd = new NpgsqlCommand(commandText, _connection))
                    {
                        List<GymTypeDetailz> gymTypeDetailsList = new List<GymTypeDetailz>();
                        cmd.Parameters.AddWithValue("partner_id", partnerId);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Files files = new Files();
                                files.partner_id = Convert.ToInt32(reader["partner_id"]);
                                files.file_link= Convert.ToString(reader["file_link"]);
                                files.file_name = Convert.ToString(reader["file_name"]);
                                files.file_description= Convert.ToString(reader["file_description"]);
                                files.created_by= Convert.ToString(reader["created_by"]);
                                files.created_on = ((DateTime)reader["created_on"]);
                                _files.Add(files);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception at GetFileDetails()");
                    Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                    Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                }
            }
            return _files;
        }
    }
}
