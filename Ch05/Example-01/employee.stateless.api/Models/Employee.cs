using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace employee.stateless.api.Models
{
    /// <summary>
    /// Employee Entity
    /// </summary>
    public class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Contact { get; set; }

        [Required]
        public string Country { get; set; }

        public string NativeLanguageName { get; set; }
    }
}
