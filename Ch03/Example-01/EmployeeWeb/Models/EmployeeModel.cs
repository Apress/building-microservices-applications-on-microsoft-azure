using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeWeb.Models
{
    public class EmployeeViewModel
    {
        public Employee Employee { get; set; }

        public List<Employee> EmployeeList { get; set; }
    }


    public class Employee
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Mobile { get; set; }

        public long Id { get; set; }

        [Required]
        public string Designation { get; set; }
    }
}
