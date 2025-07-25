using BankingApp.API.BankingApp.Domain.Entities;

namespace BankingApp.API.BankingApp.Application.Application.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(Guid id);
        Task AddAsync(Cliente cliente);
        Task<bool> ExistePorIdentificacionAsync(string identificacion);

        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(Guid id);
    }
}
