using BankingApp.API.BankingApp.Application.Application.Interfaces;
using BankingApp.API.BankingApp.Domain.Entities;
using BankingApp.API.BankingApp.Infrastructure.Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingApp.API.BankingApp.Application.Application.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IUnitOfWork _unit;
        private const decimal LIMITE_DIARIO = 1000;

        public MovimientoService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IEnumerable<Movimiento>> GetAllMovimientosAsync()
            => await _unit.Movimientos.GetAllAsync();

        public async Task<IEnumerable<Movimiento>> GetByCuentaAsync(Guid cuentaId)
            => await _unit.Movimientos.GetByCuentaIdAsync(cuentaId);

        public async Task<IEnumerable<Movimiento>> GetByCuentaYFechas(Guid cuentaId, DateTime desde, DateTime hasta)
            => await _unit.Movimientos.GetByCuentaIdAndRangoFechasAsync(cuentaId, desde, hasta);

        public async Task AddMovimientoAsync(Guid cuentaId, Movimiento movimiento)
        {
            if (movimiento.Valor <= 0)
                throw new Exception("El valor del movimiento debe ser mayor que cero.");

            var cuenta = await _unit.Cuentas.GetByIdAsync(cuentaId)
                         ?? throw new Exception("Cuenta no encontrada");

            if (!cuenta.Estado)
                throw new Exception("La cuenta está inactiva y no permite movimientos.");

            var valor = movimiento.Valor;
            var tipo = movimiento.TipoMovimiento.ToLower();

            if (tipo == "crédito")
            {
                cuenta.SaldoInicial += valor;
            }
            else if (tipo == "débito")
            {
                if (valor > cuenta.SaldoInicial)
                    throw new Exception("Saldo no disponible");

                var totalHoy = await _unit.Movimientos.ObtenerTotalDebitosHoyAsync(cuentaId, DateTime.UtcNow);
                if ((totalHoy + valor) > LIMITE_DIARIO)
                    throw new Exception("Cupo diario excedido");

                cuenta.SaldoInicial -= valor;
            }
            else
            {
                throw new Exception("Tipo de movimiento inválido");
            }

            movimiento.Fecha = DateTime.Now;
            movimiento.Saldo = cuenta.SaldoInicial;
            movimiento.CuentaId = cuentaId;

            await _unit.Movimientos.AddAsync(movimiento);
            _unit.Cuentas.Update(cuenta);
            await _unit.SaveChangesAsync();
        }

        public async Task UpdateMovimientoAsync(Guid movimientoId, Movimiento movimientoEditado)
        {
            var existente = await _unit.Movimientos.GetByIdAsync(movimientoId)
                ?? throw new ArgumentException("Movimiento no encontrado");

            var cuenta = await _unit.Cuentas.GetByIdAsync(existente.CuentaId)
                ?? throw new Exception("Cuenta no encontrada");

            if (!cuenta.Estado)
                throw new Exception("La cuenta está inactiva y no permite movimientos.");

            if (existente.TipoMovimiento.ToLower() == "crédito")
            {
                cuenta.SaldoInicial -= existente.Valor;
            }
            else if (existente.TipoMovimiento.ToLower() == "débito")
            {
                cuenta.SaldoInicial += existente.Valor;
            }

            if (movimientoEditado.Valor <= 0)
                throw new Exception("El valor debe ser mayor que cero.");

            if (movimientoEditado.TipoMovimiento.ToLower() == "crédito")
            {
                cuenta.SaldoInicial += movimientoEditado.Valor;
            }
            else if (movimientoEditado.TipoMovimiento.ToLower() == "débito")
            {
                if (movimientoEditado.Valor > cuenta.SaldoInicial)
                    throw new Exception("Saldo insuficiente.");

                var totalHoy = await _unit.Movimientos.ObtenerTotalDebitosHoyAsync(cuenta.CuentaId, DateTime.UtcNow);
                if ((totalHoy + movimientoEditado.Valor - existente.Valor) > LIMITE_DIARIO)
                    throw new Exception("Cupo diario excedido");

                cuenta.SaldoInicial -= movimientoEditado.Valor;
            }
            else
            {
                throw new Exception("Tipo de movimiento inválido");
            }

            existente.TipoMovimiento = movimientoEditado.TipoMovimiento;
            existente.Valor = movimientoEditado.Valor;
            existente.Fecha = movimientoEditado.Fecha;
            existente.Saldo = cuenta.SaldoInicial;

            _unit.Movimientos.Update(existente);
            _unit.Cuentas.Update(cuenta);

            await _unit.SaveChangesAsync();
        }


        public async Task DeleteMovimientoAsync(Guid movimientoId)
        {
            var movimiento = await _unit.Movimientos.GetByIdAsync(movimientoId)
                ?? throw new ArgumentException("Movimiento no encontrado");

            _unit.Movimientos.Remove(movimiento);
            await _unit.SaveChangesAsync();
        }

    }
}