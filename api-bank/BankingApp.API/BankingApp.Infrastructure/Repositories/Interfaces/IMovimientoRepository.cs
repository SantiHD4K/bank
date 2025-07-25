using BankingApp.API.BankingApp.Domain.Entities;

namespace BankingApp.API.BankingApp.Infrastructure.Repositories.Interfaces
{
    public interface IMovimientoRepository : IGenericRepository<Movimiento>
    {
        Task<IEnumerable<Movimiento>> GetByCuentaIdAsync(Guid cuentaId);
        Task<IEnumerable<Movimiento>> GetByCuentaIdAndRangoFechasAsync(Guid cuentaId, DateTime fechaInicio, DateTime fechaFin);
        Task<decimal> ObtenerTotalDebitosHoyAsync(Guid cuentaId, DateTime hoy);
    }
}
