using BankingApp.API.BankingApp.Infrastructure.Data;
using BankingApp.API.BankingApp.Infrastructure.Repositories.Interfaces;

namespace BankingApp.API.BankingApp.Infrastructure.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankingDbContext _context;

        public IClienteRepository Clientes { get; private set; }
        public ICuentaRepository Cuentas { get; private set; }
        public IMovimientoRepository Movimientos { get; private set; }

        public UnitOfWork(BankingDbContext context)
        {
            _context = context;
            Clientes = new ClienteRepository(context);
            Cuentas = new CuentaRepository(context);
            Movimientos = new MovimientoRepository(context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
