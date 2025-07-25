using BankingApp.API.BankingApp.Domain.Entities;

namespace BankingApp.API.BankingApp.Infrastructure.Repositories.Interfaces
{
    public interface ICuentaRepository : IGenericRepository<Cuenta>
    {
        Task<IEnumerable<Cuenta>> GetAllAsync();
        Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta);
        Task<IEnumerable<Cuenta>> GetByClienteIdAsync(Guid clienteId);
    }
}
