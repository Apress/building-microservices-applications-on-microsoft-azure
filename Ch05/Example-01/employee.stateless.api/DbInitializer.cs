using employee.stateless.api.Models;

namespace employee.stateless.api
{
    /// <summary>
    /// Class to initialize database
    /// </summary>
    public class DbInitializer
    {
        private SampleContext _context = null;

        public DbInitializer(SampleContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            _context.Database.EnsureCreated();
        }
    }
}
