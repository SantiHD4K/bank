using BankingApp.API.BankingApp.Application.Application.Interfaces;
using BankingApp.API.BankingApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _clienteService.GetAllAsync();
            return Ok(result);
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
            var cliente = await _clienteService.GetByIdAsync(id);
            return cliente == null ? NotFound(new { error = "Cliente no encontrado" }) : Ok(cliente);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Cliente cliente)
    {
        try
        {
            var existe = await _clienteService.ExistePorIdentificacionAsync(cliente.Identificacion);
            if (existe)
                return BadRequest(new { error = "Existe cliente con esta identificacion." });

            if (cliente.Id == Guid.Empty)
                cliente.Id = Guid.NewGuid();

            await _clienteService.AddAsync(cliente);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Cliente cliente)
    {
        if (id != cliente.Id)
            return BadRequest(new { error = "El ID del cliente no coincide." });

        try
        {
            await _clienteService.UpdateAsync(cliente);
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
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound(new { error = "Cliente no encontrado" });

            cliente.Estado = nuevoEstado;
            await _clienteService.UpdateAsync(cliente);
            return Ok(new { mensaje = "Estado actualizado", cliente });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}/contraseña")]
    public async Task<IActionResult> PatchContraseña(Guid id, [FromBody] string nuevaContraseña)
    {
        try
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound(new { error = "Cliente no encontrado" });

            cliente.Contraseña = nuevaContraseña;
            await _clienteService.UpdateAsync(cliente);
            return Ok(new { mensaje = "Contraseña actualizada", cliente });
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
            await _clienteService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
