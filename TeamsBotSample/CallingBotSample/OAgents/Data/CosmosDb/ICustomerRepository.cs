using CallingBotSample.OAgents.Data.CosmosDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CallingBotSample.OAgents.Data.CosmosDb
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetCustomerByIdAsync(string customerId);
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task InsertCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
    }
}