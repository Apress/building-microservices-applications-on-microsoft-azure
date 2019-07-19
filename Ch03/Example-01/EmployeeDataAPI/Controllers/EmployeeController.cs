using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace EmployeeDataAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IReliableStateManager stateManager;

        public EmployeeController(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAll()
        {
            CancellationToken ct = new CancellationToken();

            IReliableDictionary<string, Employee> employees = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, Employee>>("employees");

            List<Employee> employeesList = new List<Employee>();

            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<string, Employee>> list = await employees.CreateEnumerableAsync(tx);

                Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<string, Employee>> enumerator = list.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(ct))
                {
                    employeesList.Add(enumerator.Current.Value);
                }
            }

            return new ObjectResult(employeesList);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(string id)
        {
            IReliableDictionary<string, Employee> employees = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, Employee>>("employees");

            Employee employee = null;

            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                ConditionalValue<Employee> currentEmployee = await employees.TryGetValueAsync(tx, id);

                if (currentEmployee.HasValue)
                {
                    employee = currentEmployee.Value;
                }
            }

            return new OkObjectResult(employee);

        }

        [HttpPost]
        public async Task<ActionResult> Post(Employee employee)
        {
            IReliableDictionary<string, Employee> employees = await this.stateManager.GetOrAddAsync<IReliableDictionary<string,Employee>>("employees");
            
            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                ConditionalValue<Employee> currentEmployee = await employees.TryGetValueAsync(tx, employee.Id.ToString());

                if (currentEmployee.HasValue)
                {
                    await employees.SetAsync(tx, employee.Id.ToString(), employee);

                }else
                {
                    await employees.AddAsync(tx, employee.Id.ToString(), employee);
                }
                                
                await tx.CommitAsync();
            }

            return new OkResult();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            IReliableDictionary<string, Employee> employees = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, Employee>>("employees");

            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                if (await employees.ContainsKeyAsync(tx, id))
                {
                    await employees.TryRemoveAsync(tx, id);

                    await tx.CommitAsync();

                    return new OkResult();
                }
                else
                {
                    return new NotFoundResult();
                }
            }
        }
        
    }

    public class Employee
    {
        public string Name { get; set; }

        public string Mobile { get; set; }

        public long Id { get; set; }

        public string Designation { get; set; }
    }
}