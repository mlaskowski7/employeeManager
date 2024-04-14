using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using departmentsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace departmentsAPI.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string query = @"select * from Employee";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

            try
            {
                using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                    {
                        using (var myReader = await myCommand.ExecuteReaderAsync())
                        {
                            table.Load(myReader);
                        }
                    }
                }

                return new JsonResult(table);
            } catch (Exception e)
            {
                return StatusCode(500, "Internal server error: " + e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            string query = @"insert into Employee (Name, Surname, DateOfJoining, DepartmentID) values (@Name, @Surname, @DateOfJoining, @DepartmentID)";

            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

            try
            {
                using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Name", employee.Name);
                        myCommand.Parameters.AddWithValue("@Surname", employee.Surname);
                        DateTime dateOfJoining;
                        if (DateTime.TryParse(employee.DateOfJoining, out dateOfJoining))
                        {
                            // Parsing was successful, add the parameter as a DateTime object
                            myCommand.Parameters.Add(new NpgsqlParameter("DateOfJoining", NpgsqlTypes.NpgsqlDbType.Date) { Value = dateOfJoining });
                        }
                        else
                        {
                            // Parsing failed, handle the error, perhaps by returning a BadRequest response
                            return BadRequest("Invalid date format for DateOfJoining.");
                        }
                        myCommand.Parameters.AddWithValue("@DepartmentID", employee.DepartmentID);

                        await myCommand.ExecuteNonQueryAsync();
                    }
                }
                return StatusCode(201, "Employee Created Successfully");
            } catch (Exception e)
            {
                return StatusCode(500, "Internal Sever Error: " + e.Message);
            }
        }
    }
}

