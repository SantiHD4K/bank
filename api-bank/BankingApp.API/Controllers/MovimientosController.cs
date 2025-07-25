using BankingApp.API.BankingApp.Application.Application.Interfaces;
using BankingApp.API.BankingApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MovimientosController : ControllerBase
{
    private readonly IMovimientoService _movimientoService;

    public MovimientosController(IMovimientoService movimientoService)
    {
        _movimientoService = movimientoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var movimientos = await _movimientoService.GetAllMovimientosAsync();
            return Ok(movimientos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("cuenta/{cuentaId}")]
    public async Task<IActionResult> GetByCuenta(Guid cuentaId)
    {
        try
        {
            var movimientos = await _movimientoService.GetByCuentaAsync(cuentaId);
            return Ok(movimientos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("cuenta/{cuentaId}/rango")]
    public async Task<IActionResult> GetByRango(Guid cuentaId, [FromQuery] DateTime desde, [FromQuery] DateTime hasta)
    {
        try
        {
            var movimientos = await _movimientoService.GetByCuentaYFechas(cuentaId, desde, hasta);
            return Ok(movimientos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{cuentaId}")]
    public async Task<IActionResult> CrearMovimiento(Guid cuentaId, [FromBody] Movimiento movimiento)
    {
        try
        {
            await _movimientoService.AddMovimientoAsync(cuentaId, movimiento);
            return Ok(movimiento);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{movimientoId}")]
    public async Task<IActionResult> Editar(Guid movimientoId, [FromBody] Movimiento movimiento)
    {
        try
        {
            await _movimientoService.UpdateMovimientoAsync(movimientoId, movimiento);
            return Ok(new { mensaje = "Movimiento actualizado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{movimientoId}")]
    public async Task<IActionResult> Eliminar(Guid movimientoId)
    {
        try
        {
            await _movimientoService.DeleteMovimientoAsync(movimientoId);
            return Ok(new { mensaje = "Movimiento eliminado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

}