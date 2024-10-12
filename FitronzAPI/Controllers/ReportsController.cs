using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FitronzService.Extension;
using FitronzService.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IO;

namespace FitronzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {

        private readonly IOrdersService _ordersService;

        public ReportsController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpGet]
        [Route("GetOrderReport")]
        public async Task<IActionResult> GetOrderReport()
        {
            var resultText = string.Empty;
            var result = await _ordersService.GetOrderDetails();


            // Create a DataTable to hold flattened data
            DataTable dataTable = new DataTable();

            // Define columns for the DataTable
            dataTable.Columns.Add("Order ID", typeof(string));
            dataTable.Columns.Add("User ID", typeof(string));
            dataTable.Columns.Add("User Name", typeof(string));
            dataTable.Columns.Add("User Age", typeof(int));
            dataTable.Columns.Add("User Gender", typeof(string));
            dataTable.Columns.Add("Gym Name", typeof(string));
            dataTable.Columns.Add("Gym Address", typeof(string));
            dataTable.Columns.Add("Slot ID", typeof(int));
            dataTable.Columns.Add("Slot Start Time", typeof(DateTime));
            dataTable.Columns.Add("Slot End Time", typeof(DateTime));
            dataTable.Columns.Add("GymType", typeof(string));
            dataTable.Columns.Add("Total Amount (with GST)", typeof(decimal));
            dataTable.Columns.Add("Rate (without GST)", typeof(decimal));
            dataTable.Columns.Add("GST", typeof(decimal));

            // Loop through UserOrders and flatten the structure
            foreach (var order in result)
            {
                bool isFirstRowForOrder = true; // Flag to track if it's the first row for the current order

                foreach (var slot in order.gymDetails.slot_details)
                {
                    foreach (var gymType in slot.gym_type_details)
                    {
                        // Add a row for each combination of UserOrders, SlotDetails, and GymTypeDetailz
                        DataRow row = dataTable.NewRow();

                        // Fill "Order ID" and "User ID" only for the first row of each order
                        if (isFirstRowForOrder)
                        {
                            row["Order ID"] ="FOCO - "+ order.order_id;
                            row["User ID"] = "CUST - "+order.user_id;
                            row["User Name"] = order.user_name;
                            row["User Age"] = order.user_age;
                            row["User Gender"] = order.user_gender;
                            row["Gym Name"] = order.gymDetails.gym_name;
                            row["Gym Address"] = order.gymDetails.gym_address;
                            isFirstRowForOrder = false;  // Set flag to false after the first row is filled
                        }
                        else
                        {
                            row["Order ID"] = DBNull.Value;  // Leave empty for subsequent rows
                            row["User ID"] = DBNull.Value;
                            row["User Name"] = DBNull.Value;
                            row["User Age"] = DBNull.Value;
                            row["User Gender"] = DBNull.Value;
                            row["Gym Name"] = DBNull.Value;
                            row["Gym Address"] = DBNull.Value;
                        }

                        // Fill other columns
                        
                        row["Slot ID"] = slot.slot_id;
                        row["Slot Start Time"] = slot.slot_start_time;
                        row["Slot End Time"] = slot.slot_end_time;
                        row["GymType"] = gymType.gymtype;
                        row["Total Amount (with GST)"] = gymType.price;
                        row["Rate (without GST)"] = (Convert.ToDecimal(gymType.price) - (Convert.ToDecimal(18.0 / 100.0) * gymType.price));
                        row["GST"] = (gymType.price * 18) / 100;

                        // Add row to DataTable
                        dataTable.Rows.Add(row);
                    }
                }
            }

            // Now generate Excel using ClosedXML
            using (var wb = new XLWorkbook())
            {
                var sheet = wb.AddWorksheet("Order details");
                int startingRow = 10;
                sheet.Cell(startingRow, 1).InsertTable(dataTable);

                // Additional static information
                sheet.Cell(2, 1).Value = "GYM NAME";
                sheet.Cell(2, 2).Value = result[0].gymDetails.gym_name;
                sheet.Cell(3, 1).Value = "OWNER NAME";
                sheet.Cell(3, 2).Value = result[0].gymDetails.owner_name;
                sheet.Cell(4, 1).Value = "ADDRESS";
                sheet.Cell(4, 2).Value = result[0].gymDetails.gym_address;
                sheet.Cell(5, 1).Value = "PHONE";
                sheet.Cell(5, 2).Value = result[0].gymDetails.mobile_number;
                sheet.Cell(6, 1).Value = "MAIL ID";
                sheet.Cell(6, 2).Value = result[0].gymDetails.email_address;
                sheet.Cell(8, 1).Value = "Report generated from";

                // Adjust column widths
                sheet.Columns().AdjustToContents();

                // Save the Excel file to memory stream
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);

                    ms.Seek(0, SeekOrigin.Begin);
                    var fileName = "OrderReport.xlsx";
                    var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(ms.ToArray(), mimeType, fileName);
                }
            }
        }

        //[HttpGet("ExportExcel")]
        //public async Task<ActionResult> GetExcelFile(DataTable dt)
        //{
        //    // Assuming GetEmployeeData is an asynchronous method that retrieves employee data
        //    //var employeeData = await GetEmployeeData();


        //    string base64String;

        //    using (var wb = new XLWorkbook())
        //    {
        //        var sheet = wb.AddWorksheet(dt, "Employee Records");

        //        // Apply font color to columns 1 to 5
        //        sheet.Columns(1, 5).Style.Font.FontColor = XLColor.Black;

        //        using (var ms = new MemoryStream())
        //        {
        //            wb.SaveAs(ms);

        //            // Convert the Excel workbook to a base64-encoded string
        //            base64String = Convert.ToBase64String(ms.ToArray());
        //        }
        //    }

        //    // Return a CreatedResult with the base64-encoded Excel data
        //    return new CreatedResult(string.Empty, new
        //    {
        //        Code = 200,
        //        Status = true,
        //        Message = "",
        //        Data = base64String
        //    });
        //}
    }
}
