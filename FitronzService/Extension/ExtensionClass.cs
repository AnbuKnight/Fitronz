using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Extension
{
    public class ExtensionClass
    {
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            // Get all the properties of the type
            var properties = typeof(T).GetProperties();

            // Create the columns based on the properties
            foreach (var prop in properties)
            {
                if (prop.PropertyType.Name.Contains("Nullable"))

                    dataTable.Columns.Add(prop.Name, typeof(String));
                else

                    dataTable.Columns.Add(prop.Name, prop.PropertyType);


            }

            // Add the rows to the DataTable
            foreach (T item in items)
            {
                var values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
