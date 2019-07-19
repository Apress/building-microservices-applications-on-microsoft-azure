using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using employee.stateless.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace employee.stateless.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        /// <summary>
        /// Context
        /// </summary>
        private SampleContext _context = null;

        private HttpClient _httpClient = null;

        private AppSettings _appSettings = null;

        /// <summary>
        /// Employee Controller
        /// </summary>
        /// <param name="context"></param>
        public EmployeeController(SampleContext context, IHttpClientFactory httpClientFactory, IOptionsMonitor<AppSettings> appSettings)
        {
            _context = context;

            _appSettings = appSettings.CurrentValue;

            _httpClient = httpClientFactory.CreateClient();
        }


        /// <summary>
        /// Returns all the employees
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAll()
        {
            List<Employee> employeeList = await _context.Employees.ToListAsync<Employee>();

            return new OkObjectResult(employeeList);
        }

        /// <summary>
        /// Returns an employee based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            Employee employee = await _context.Employees.Where(e => e.Id.Equals(id)).FirstOrDefaultAsync();

            return new OkObjectResult(employee);
        }

        /// <summary>
        /// Creates an emaployee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(Employee employee)
        {
            employee.NativeLanguageName = await GetTranslatedText(employee.FirstName);

            await _context.Employees.AddAsync(employee);

            await _context.SaveChangesAsync();

            return new OkResult();
        }


        /// <summary>
        /// Deletes the employee based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Employee employee = await _context.Employees.Where(e => e.Id.Equals(id)).FirstOrDefaultAsync();

            if (employee == null)
            {
                return new NotFoundResult();

            }else
            {
                _context.Employees.Remove(employee);

                await _context.SaveChangesAsync();

                return new OkResult();
            }
        }


        /// <summary>
        /// Gets the name in hindi
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<string> GetTranslatedText(string name)
        {
            System.Object[] body = new System.Object[] { new { Text = name } };

            var requestBody = JsonConvert.SerializeObject(body);

            StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _appSettings.AccessKey);

            var result =  await _httpClient.PostAsync($"{_appSettings.TranslationApiUrl}/translate?api-version=3.0&to=hi", content);

            result.EnsureSuccessStatusCode();

            string translatedJson = await result.Content.ReadAsStringAsync();

            TranslationResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<TranslationResponse>(JArray.Parse(translatedJson)[0].ToString());

            return response.translations[0].text;

        }
    }
}