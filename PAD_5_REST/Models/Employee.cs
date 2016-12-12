using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PAD_5_REST.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Department { get; set; }
        public float Salary { get; set; }
    }
}