using BankingApp.API.BankingApp.Infrastructure.Repositories.Interfaces;

namespace BankingApp.API.BankingApp.Infrastructure.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IClienteRepository Clientes { get; }
        ICuentaRepository Cuentas { get; }
        IMovimientoRepository Movimientos { get; }

        Task<int> SaveChangesAsync();

    }

}
