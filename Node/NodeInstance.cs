using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Cassandra;
using System.Net;
using System.Diagnostics;

namespace Node
{
    class NodeInstance
    {
        static String warehouseAddress = "http://localhost:61132/";

        static void Main(string[] args)
        {
            List<Employee> employees = GenerateEmployeeRecords();
            String output = GetStringFromEmployeeList(employees);

            Console.WriteLine(output);
            Console.ReadKey();

            //PostEmployeesToServiceAsync(employees).Wait();

            PostEmployeesToDB(employees);

            Console.WriteLine("Employees have been posted.");
            Console.ReadKey();
        }

        static void PostEmployeesToDB(List<Employee> employees)
        {
            try
            {
                Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();

                Console.WriteLine("Connected to cluster: " + cluster.Metadata.ClusterName.ToString());

                ISession session = cluster.Connect("pad6");

                foreach (Employee e in employees)
                {
                    String FirstName = e.FirstName;
                    String LastName = e.LastName;
                    String Department = e.Department;
                    float Salary = e.Salary;
                    int Id = GenerateId();

                    String values = String.Format(" values ('{0}', {1}, '{2}', '{3}', {4})"
                        , LastName, Salary, Department, FirstName, Id);

                    session.Execute("insert into employees (LastName, Salary, Department, FirstName, Id)" + values);
                }

                RowSet rows = session.Execute("select * from employees");
                foreach (Row row in rows)
                { 
                    Console.WriteLine("{0} {1}", row["lastname"], row["salary"]); 
                }
            }
            catch (Cassandra.NoHostAvailableException e)
            {
                foreach (KeyValuePair<IPEndPoint, Exception> kvp in e.Errors)
                {
                    Debug.WriteLine(kvp.Key);
                }
            }
        }

        static int GenerateId()
        {
            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            ISession session = cluster.Connect("pad6");
            RowSet rows = session.Execute("select * from employees");
            int result = 0;
            foreach (Row r in rows)
            {
                result += 1;
            }
            return result;
        }

        static async Task PostEmployeesToServiceAsync(List<Employee> employees)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(warehouseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                foreach (Employee e in employees) 
                {
                    HttpResponseMessage response =
                        await client.PostAsJsonAsync("api/Employees", e);

                    Console.WriteLine("POST status: " + response.StatusCode);
                }

                //if (response.IsSuccessStatusCode)
                //{
                //    resourceUrl = response.Headers.Location;
                //}
            }
        }

        private static List<Employee> GenerateEmployeeRecords()
        {
            Random rng = new Random();
            int steps = rng.Next(1, 5);
            List<Employee> employees = new List<Employee>();
            for (int i = 0; i != steps; ++i)
            {
                Employee employee = Employee.GenerateEmployee("HR");
                employees.Add(employee);
            }

            return employees;
        }

        private static String GetStringFromEmployeeList(List<Employee> employees)
        {
            String formattedString = "";
            foreach (Employee e in employees)
            {
                formattedString += e.ToString() + "\n";
            }

            return formattedString;
        }
    }
}
