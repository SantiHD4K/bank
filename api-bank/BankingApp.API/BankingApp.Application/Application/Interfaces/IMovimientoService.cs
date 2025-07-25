using BankingApp.API.BankingApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingApp.API.BankingApp.Application.Application.Interfaces
{
    public interface IMovimientoService
    {
        Task<IEnumerable<Movimiento>> GetAllMovimientosAsync();
        Task<IEnumerable<Movimiento>> GetByCuentaAsync(Guid cuentaId);
        Task AddMovimientoAsync(Guid cuentaId, Movimiento movimiento);
        Task<IEnumerable<Movimiento>> GetByCuentaYFechas(Guid cuentaId, DateTime desde, DateTime hasta);
        Task UpdateMovimientoAsync(Guid movimientoId, Movimiento movimiento);
        Task DeleteMovimientoAsync(Guid movimientoId);

    }
}