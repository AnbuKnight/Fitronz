using ClosedXML.Excel;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Implementation
{
    public class ReportService
    {
        public ReportService() { }
        //public Base64 test() {
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
