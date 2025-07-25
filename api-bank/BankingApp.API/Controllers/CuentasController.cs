using BankingApp.API.BankingApp.Application.Application.Interfaces;
using BankingApp.API.BankingApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CuentasController : ControllerBase
{
    private readonly ICuentaService _cuentaService;

    public CuentasController(ICuentaService cuentaService)
    {
        _cuentaService = cuentaService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var cuentas = await _cuentaService.GetAllAsync();
            return Ok(cuentas);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var cuenta = await _cuentaService.GetByIdAsync(id);
            return cuenta == null ? NotFound(new { error = "Cuenta no encontrada" }) : Ok(cuenta);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> GetByCliente(Guid clienteId)
    {
        try
        {
            var cuentas = await _cuentaService.GetByClienteAsync(clienteId);
            return Ok(cuentas);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Cuenta cuenta)
    {
        try
        {
            var existe = await _cuentaService.ExistePorNumeroCuentaAsync(cuenta.NumeroCuenta);
            if (existe)
                return BadRequest(new { error = "Ya existe este numero de cuenta." });

            await _cuentaService.AddAsync(cuenta);
            return Ok(cuenta);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Cuenta cuenta)
    {
        if (id != cuenta.CuentaId)
            return BadRequest(new { error = "El ID de la cuenta no coincide." });

        try
        {
            var cuentaExistente = await _cuentaService.GetByNumeroCuentaAsync(cuenta.NumeroCuenta);
            if (cuentaExistente != null && cuentaExistente.CuentaId != cuenta.CuentaId)
            {
                return BadRequest(new { error = "Ya existe otra cuenta con este número de cuenta." });
            }

            await _cuentaService.UpdateAsync(cuenta);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> PatchEstado(Guid id, [FromBody] bool nuevoEstado)
    {
        try
        {
            var cuenta = await _cuentaService.GetByIdAsync(id);
            if (cuenta == null)
                return NotFound(new { error = "Cuenta no encontrada" });

            cuenta.Estado = nuevoEstado;
            await _cuentaService.UpdateAsync(cuenta);
            return Ok(new { mensaje = "Estado de cuenta actualizado", cuenta });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _cuentaService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
