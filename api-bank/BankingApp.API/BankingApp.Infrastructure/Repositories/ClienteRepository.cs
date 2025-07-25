using BankingApp.API.BankingApp.Domain.Entities;
using BankingApp.API.BankingApp.Infrastructure.Data;
using BankingApp.API.BankingApp.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.API.BankingApp.Infrastructure.Repositories
{
    public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
    {
        public ClienteRepository(BankingDbContext context) : base(context) { }

        public async Task<Cliente?> GetByIdentificacionAsync(string identificacion)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Identificacion == identificacion);
        }
    }

}
