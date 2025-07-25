using BankingApp.API.BankingApp.Domain.Entities;

namespace BankingApp.API.BankingApp.Infrastructure.Repositories.Interfaces
{
    public interface IClienteRepository : IGenericRepository<Cliente>
    {
        Task<Cliente?> GetByIdentificacionAsync(string identificacion);
    }

}
