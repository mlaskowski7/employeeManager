using System;
namespace departmentsAPI.Models
{
	public class Employee
	{
		public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DateOfJoining { get; set; }
        public int DepartmentID { get; set; }
    }
}

