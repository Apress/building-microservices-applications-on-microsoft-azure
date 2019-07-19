using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace employee.stateless.api.Models
{
    /// <summary>
    /// Context
    /// </summary>
    public class SampleContext:DbContext
    {
        public SampleContext(DbContextOptions<SampleContext> options) : base(options)
        {
        }

        /// <summary>
        /// Employees
        /// </summary>
        public DbSet<Employee> Employees { get; set; }


        /// <summary>
        /// Override the table name
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee");
        }
    }
}
