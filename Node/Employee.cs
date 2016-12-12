using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node
{
    public class Employee
    {
        public int Id;
        public String FirstName;
        public String LastName;
        public String Department;
        public float Salary;

        public Employee() : base() { }

        Employee(String dep)
        {
            FirstName = GenerateName();
            LastName = GenerateName();
            Department = dep;
            Salary = GenerateSalary();
        }

        Employee(String last, String first, String dep, float sal)
        {
            FirstName = first;
            LastName = last;
            Department = dep;
            Salary = sal;
        }

        private String GenerateName()
        {
            String name = "";
            String chars = "abcdefghijklmnopqrstuvwxyz";
            Random rng = new Random(Guid.NewGuid().GetHashCode());

            int length = rng.Next(3, 8);
            for (int i = 0; i != length; ++i)
            {
                name += chars[rng.Next(0, chars.Length - 1)];
                if (i == 0)
                {
                    name = name.ToUpper();
                }
            }

            return name;
        }

        private float GenerateSalary()
        {
            Random rng = new Random(Guid.NewGuid().GetHashCode());
            float salary = rng.Next(500, 100000);
            return salary;
        }

        public static Employee GenerateEmployee(String department)
        {
            Employee employee = new Employee(department);
            return employee;
        }

        public static Employee GetEmployeeFromJSON(String jsonObject)
        {
            Employee result = JsonConvert.DeserializeObject<Employee>(jsonObject);

            return result;
        }

        public override string ToString()
        {
            String formattedString = "";
            EmployeeContainer container = new EmployeeContainer(this);
            formattedString = JsonConvert.SerializeObject(container);

            return formattedString;
        }

        class EmployeeContainer
        {
            public String firstName;
            public String lastName;
            public String department;
            public float salary;

            public EmployeeContainer(Employee employee)
            {
                firstName = employee.FirstName;
                lastName = employee.LastName;
                department = employee.Department;
                salary = employee.Salary;
            }
        }
    }
}
