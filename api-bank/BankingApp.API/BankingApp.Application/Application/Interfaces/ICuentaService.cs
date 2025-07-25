using BankingApp.API.BankingApp.Domain.Entities;

namespace BankingApp.API.BankingApp.Application.Application.Interfaces
{
    public interface ICuentaService
    {
        Task<IEnumerable<Cuenta>> GetAllAsync();
        Task<IEnumerable<Cuenta>> GetByClienteAsync(Guid clienteId);
        Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta);
        Task<Cuenta?> GetByIdAsync(Guid id);
        Task AddAsync(Cuenta cuenta);
        Task<bool> ExistePorNumeroCuentaAsync(string numeroCuenta);
        Task UpdateAsync(Cuenta cuenta);
        Task DeleteAsync(Guid id);
    }
}
