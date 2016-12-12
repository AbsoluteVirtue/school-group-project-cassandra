using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PAD_5_REST.Models;
using Cassandra;

namespace PAD_5_REST.Controllers
{
    public class EmployeesController : ApiController
    {
        private EmployeeContext db = new EmployeeContext();

        // GET: api/Employees
        //public IQueryable<Employee> GetEmployees()
        //{     
        //    return db.Employees;
        //}
        public HttpResponseMessage Get()
        {
            var employees = ParseEmployees();
            HttpResponseMessage response = Request.CreateResponse(
                HttpStatusCode.OK, employees);
            return response;
        }

        List<Employee> ParseEmployees()
        {
            List<Employee> result = new List<Employee>();
            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            ISession session = cluster.Connect("pad6");
            RowSet rows = session.Execute("select * from employees");
            foreach (Row row in rows)
            {
                Employee e = new Employee();
                e.LastName = row["lastname"] + "";
                e.FirstName = row["firstname"] + "";
                e.Salary = (float)row["salary"];
                e.Department = row["department"] + "";
                e.Id = (int)row["id"];

                result.Add(e);
            }
            return result;
        }

        // GET: api/Employees/5
        //[ResponseType(typeof(Employee))]
        //public IHttpActionResult GetEmployee(int id)
        //{
        //    Employee employee = db.Employees.Find(id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(employee);
        //}
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;
            var employeeId = FindEmployee(id);
            if (id == 0 || employeeId.Id == 0)
            {
                response = Request.CreateResponse(
                    HttpStatusCode.NotFound);
            }
            else
            {  
                response = Request.CreateResponse(
                    HttpStatusCode.OK, employeeId);
            }
            return response;
        }

        Employee FindEmployee(int id)
        {
            Employee result = new Employee();
            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            ISession session = cluster.Connect("pad6");
            RowSet rows = session.Execute("select * from employees");
            foreach (Row row in rows)
            {
                if (id == (int)row["id"])
                {
                    result.LastName = row["lastname"] + "";
                    result.FirstName = row["firstname"] + "";
                    result.Salary = (float)row["salary"];
                    result.Department = row["department"] + "";
                    result.Id = (int)row["id"];
                }
            }
            return result;
        }

        // PUT: api/Employees/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutEmployee(int id, Employee employee)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != employee.Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(employee).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!EmployeeExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}
        public HttpResponseMessage Put(int id)
        {
            var employees = UpdateEmployee(id);
            HttpResponseMessage response = Request.CreateResponse(
                HttpStatusCode.OK, employees);
            return response;
        }

        Employee UpdateEmployee(int id)
        {
            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            ISession session = cluster.Connect("pad6");
            session.Execute("update employees set salary = 10500.5 where id = " + id);
            return FindEmployee(id);
        }

        // POST: api/Employees
        //[ResponseType(typeof(Employee))]
        //public IHttpActionResult PostEmployee(Employee employee)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Employees.Add(employee);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = employee.Id }, employee);
        //}
        public HttpResponseMessage Post(Employee e)
        {
            var employees = InsertEmployee(e);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, employees);
            return response;
        }

        List<Employee> InsertEmployee(Employee e)
        {
            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            ISession session = cluster.Connect("pad6");

            String values = String.Format(" values ('{0}', {1}, '{2}', '{3}', {4})"
                        , e.LastName, e.Salary, e.Department, e.FirstName
                        , GenerateId());
            session.Execute("insert into employees (LastName, Salary, Department, FirstName, Id)" + values);
            return ParseEmployees();
        }

        // DELETE: api/Employees/5
        //[ResponseType(typeof(Employee))]
        //public IHttpActionResult DeleteEmployee(int id)
        //{
        //    Employee employee = db.Employees.Find(id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Employees.Remove(employee);
        //    db.SaveChanges();

        //    return Ok(employee);
        //}
        public HttpResponseMessage Delete(int id)
        {
            var employees = RemoveEmployee(id);
            HttpResponseMessage response = Request.CreateResponse(
                HttpStatusCode.OK, employees);
            return response;
        }

        List<Employee> RemoveEmployee(int id)
        {
            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            ISession session = cluster.Connect("pad6");
            session.Execute("delete from employees where id = " + id);
            return ParseEmployees();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //private bool EmployeeExists(int id)
        //{
        //    return db.Employees.Count(e => e.Id == id) > 0;
        //}

        int GenerateId()
        {
            int result = 0;
            Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            ISession session = cluster.Connect("pad6");
            RowSet rows = session.Execute("select * from employees");
            List<int> Ids = new List<int>();
            foreach (Row row in rows)
            {
                Ids.Add((int)row["id"]);
            }
            result = Ids.Max() + 1;
            return result;
        }

        
    }
}