using BankingApp.API.BankingApp.Domain.Entities;
using BankingApp.API.BankingApp.Infrastructure.Data;
using BankingApp.API.BankingApp.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.API.BankingApp.Infrastructure.Repositories
{
    public class CuentaRepository : GenericRepository<Cuenta>, ICuentaRepository
    {
        public CuentaRepository(BankingDbContext context) : base(context) { }

        public async Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta)
        {
            return await _context.Cuentas
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);
        }

        public async Task<IEnumerable<Cuenta>> GetByClienteIdAsync(Guid clienteId)
        {
            return await _context.Cuentas
                .Where(c => c.ClienteId == clienteId)
                .ToListAsync();
        }
    }
}
