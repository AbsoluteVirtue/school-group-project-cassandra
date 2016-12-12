using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Node;

namespace Client
{
    class ClientInstance
    {
        static String warehouseAddress = "http://localhost:61132/";
        static List<Employee> employees;

        static void Main(string[] args)
        {

            Console.WriteLine("Retrieving list of employees from the database...");

            GetAllEmployees().Wait();

            //String input = "";
            //Employee employee = new Employee();
            //Console.WriteLine("Push new entry to the database.");

            //Console.Write("First name: ");
            //input = Console.ReadLine();
            //employee.FirstName = input;

            //Console.Write("Last name: ");
            //input = Console.ReadLine();
            //employee.LastName = input;

            //Console.Write("Department: ");
            //input = Console.ReadLine();
            //employee.Department = input;

            //Console.Write("Salary: ");
            //input = Console.ReadLine();
            //employee.Salary = Convert.ToSingle(input);

            //Console.WriteLine("Pushing new entry...");
            //PostEmployee(employee).Wait();
            //GetAllEmployees().Wait();

            //Console.Write("Update employee salary. Enter id to update: ");
            //input = Console.ReadLine();
            //int id = Convert.ToInt32(input);

            //UpdateEmployee(id, 5000f).Wait();
            //GetAllEmployees().Wait();

            //Console.Write("Delete employee. Enter id: ");
            //input = Console.ReadLine();
            //int id = Convert.ToInt32(input);

            //DeleteEmployee(id).Wait();
            //GetAllEmployees().Wait();

            Console.ReadKey();
        }

        static async Task DeleteEmployee(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(warehouseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/Employees/" + id);
                Console.WriteLine("GET status: " + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    response = await client.DeleteAsync("api/Employees/" + id);
                    Console.WriteLine("DELETE status: " + response.StatusCode);
                }
            }
        }

        static async Task UpdateEmployee(int id, float salary)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(warehouseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/Employees/" + id);
                Console.WriteLine("GET status: " + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    Employee e = await response.Content.ReadAsAsync<Employee>();
                    e.Salary = salary;
                    response = await client.PutAsJsonAsync("api/Employees/" + id, e);
                    Console.WriteLine("PUT status: " + response.StatusCode);
                } 
            }
        }

        static async Task PostEmployee(Employee employee)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(warehouseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response
                    = await client.PostAsJsonAsync("api/Employees", employee);
                Console.WriteLine("POST status: " + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    Uri url = response.Headers.Location;
                }
            }
        }

        static async Task GetAllEmployees()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(warehouseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = 
                    await client.GetAsync("api/Employees");

                Console.WriteLine("GET status: " + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    employees = await response.Content.ReadAsAsync<List<Employee>>();
                    foreach (Employee e in employees)
                    {
                        Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}"
                            , e.Id
                            , e.FirstName
                            , e.LastName
                            , e.Department
                            , e.Salary
                            );
                    }
                }
            }
        }
    }
}
