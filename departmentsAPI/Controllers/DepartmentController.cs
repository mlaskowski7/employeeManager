using System;
using System.Data;
using System.Threading.Tasks;
using departmentsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace departmentsAPI.Controllers
{
    [Route("api/[controller]")]
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string query = @"select DepartmentID, DepartmentName from Department";
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
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department department)
        {
            string query = @"insert into Department (DepartmentName) values (@DepartmentName)";

            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

            try
            {
                using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
                        await myCommand.ExecuteNonQueryAsync();
                    }
                }
                return StatusCode(201, "Department Created Successfully");
            } catch (Exception e)
            {
                return StatusCode(500, "Internal Server Error: " + e.Message);
            }
        }

    }
}
