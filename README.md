using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ListToDataViewExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example list of objects
            var people = new List<Person>
            {
                new Person { Id = 1, Name = "Alice", Age = 30 },
                new Person { Id = 2, Name = "Bob", Age = 25 },
                new Person { Id = 3, Name = "Charlie", Age = 35 }
            };

            // Convert the list to a DataView
            var dataView = ConvertListToDataView(people);

            // Display the DataView rows (for demonstration)
            foreach (DataRowView rowView in dataView)
            {
                var row = rowView.Row;
                Console.WriteLine($"Id: {row["Id"]}, Name: {row["Name"]}, Age: {row["Age"]}");
            }
        }

        public static DataView ConvertListToDataView<T>(IEnumerable<T> list)
        {
            // Convert list to DataTable
            var dataTable = new DataTable(typeof(T).Name);

            // Get properties of the object
            var properties = typeof(T).GetProperties();

            // Add columns to the DataTable based on object properties
            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            // Add rows to the DataTable
            foreach (var item in list)
            {
                var row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            // Return DataView
            return dataTable.DefaultView;
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
