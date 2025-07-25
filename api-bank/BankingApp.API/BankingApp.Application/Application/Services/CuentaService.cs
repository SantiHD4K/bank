using BankingApp.API.BankingApp.Application.Application.Interfaces;
using BankingApp.API.BankingApp.Domain.Entities;
using BankingApp.API.BankingApp.Infrastructure.Repositories.UnitOfWork;

namespace BankingApp.API.BankingApp.Application.Application.Services
{
    public class CuentaService : ICuentaService
    {
        private readonly IUnitOfWork _unit;

        public CuentaService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IEnumerable<Cuenta>> GetAllAsync()
        {
            return await _unit.Cuentas.GetAllAsync();
        }

        public async Task<IEnumerable<Cuenta>> GetByClienteAsync(Guid clienteId)
            => await _unit.Cuentas.GetByClienteIdAsync(clienteId);


        public async Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta)
        {
            return await _unit.Cuentas.GetByNumeroCuentaAsync(numeroCuenta);
        }

        public async Task<Cuenta?> GetByIdAsync(Guid id)
            => await _unit.Cuentas.GetByIdAsync(id);

        public async Task AddAsync(Cuenta cuenta)
        {
            var cliente = await _unit.Clientes.GetByIdAsync(cuenta.ClienteId);
            if (cliente == null)
                throw new Exception("El cliente no existe.");

            var cuentasCliente = await _unit.Cuentas.GetByClienteIdAsync(cuenta.ClienteId);
            var existeTipo = cuentasCliente.Any(c =>
                string.Equals(c.TipoCuenta.Trim(), cuenta.TipoCuenta.Trim(), StringComparison.OrdinalIgnoreCase));

            if (existeTipo)
                throw new Exception($"El cliente ya tiene una cuenta tipo {cuenta.TipoCuenta}.");

            await _unit.Cuentas.AddAsync(cuenta);
            await _unit.SaveChangesAsync();
        }


        public async Task<bool> ExistePorNumeroCuentaAsync(string numeroCuenta)
        {
            var cuenta = await _unit.Cuentas.GetByNumeroCuentaAsync(numeroCuenta);
            return cuenta != null;
        }

        public async Task UpdateAsync(Cuenta cuenta)
        {
            var cuentaExistente = await _unit.Cuentas.GetByIdAsync(cuenta.CuentaId);
            if (cuentaExistente == null)
                throw new Exception("La cuenta a actualizar no existe.");

            var cuentasCliente = await _unit.Cuentas.GetByClienteIdAsync(cuenta.ClienteId);
            var existeTipo = cuentasCliente.Any(c =>
                c.CuentaId != cuenta.CuentaId &&
                string.Equals(c.TipoCuenta.Trim(), cuenta.TipoCuenta.Trim(), StringComparison.OrdinalIgnoreCase));

            if (existeTipo)
                throw new Exception($"El cliente ya tiene una cuenta tipo {cuenta.TipoCuenta}.");

            cuentaExistente.NumeroCuenta = cuenta.NumeroCuenta;
            cuentaExistente.TipoCuenta = cuenta.TipoCuenta;
            cuentaExistente.SaldoInicial = cuenta.SaldoInicial;
            cuentaExistente.Estado = cuenta.Estado;
            cuentaExistente.ClienteId = cuenta.ClienteId;

            await _unit.SaveChangesAsync();
        }



        public async Task DeleteAsync(Guid id)
        {
            var cuenta = await _unit.Cuentas.GetByIdAsync(id);
            if (cuenta is null) throw new Exception("Cuenta no encontrada");
            _unit.Cuentas.Remove(cuenta);
            await _unit.SaveChangesAsync();
        }
    }

}
