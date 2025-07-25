using BankingApp.API.BankingApp.Application.Application.Interfaces;
using BankingApp.API.BankingApp.Domain.Entities;
using BankingApp.API.BankingApp.Infrastructure.Repositories.UnitOfWork;

namespace BankingApp.API.BankingApp.Application.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unit;

        public ClienteService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync() => await _unit.Clientes.GetAllAsync();

        public async Task<Cliente?> GetByIdAsync(Guid id) => await _unit.Clientes.GetByIdAsync(id);

        public async Task AddAsync(Cliente cliente)
        {
            await _unit.Clientes.AddAsync(cliente);
            await _unit.SaveChangesAsync();
        }

        public async Task<bool> ExistePorIdentificacionAsync(string identificacion)
        {
            var existente = await _unit.Clientes.GetByIdentificacionAsync(identificacion);
            return existente != null;
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _unit.Clientes.Update(cliente);
            await _unit.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var cliente = await _unit.Clientes.GetByIdAsync(id);
            if (cliente is null) throw new Exception("Cliente no encontrado");
            _unit.Clientes.Remove(cliente);
            await _unit.SaveChangesAsync();
        }
    }

}
