using BankingApp.API.BankingApp.Domain.Entities;
using BankingApp.API.BankingApp.Infrastructure.Data;
using BankingApp.API.BankingApp.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.API.BankingApp.Infrastructure.Repositories
{
    public class MovimientoRepository : GenericRepository<Movimiento>, IMovimientoRepository
    {
        public MovimientoRepository(BankingDbContext context) : base(context) { }

        public async Task<IEnumerable<Movimiento>> GetByCuentaIdAsync(Guid cuentaId)
        {
            return await _context.Movimientos
                .Where(m => m.CuentaId == cuentaId)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movimiento>> GetByCuentaIdAndRangoFechasAsync(Guid cuentaId, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Movimientos
                .Where(m => m.CuentaId == cuentaId &&
                            m.Fecha.Date >= fechaInicio.Date &&
                            m.Fecha.Date <= fechaFin.Date)
                .ToListAsync();
        }

        public async Task<decimal> ObtenerTotalDebitosHoyAsync(Guid cuentaId, DateTime hoy)
        {
            return await _context.Movimientos
                .Where(m => m.CuentaId == cuentaId &&
                            m.TipoMovimiento.ToLower() == "débito" &&
                            m.Fecha.Date == hoy.Date)
                .SumAsync(m => Math.Abs(m.Valor));
        }
    }
}
