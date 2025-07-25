using BankingApp.API.BankingApp.Infrastructure.Repositories.Interfaces;
using BankingApp.API.BankingApp.Infrastructure.Repositories.UnitOfWork;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly ICuentaRepository _cuentas;
    private readonly IMovimientoRepository _movimientos;
    private readonly PdfGenerator _pdfGenerator;

    public ReportesController(IUnitOfWork unit, IConverter converter)
    {
        _cuentas = unit.Cuentas;
        _movimientos = unit.Movimientos;
        _pdfGenerator = new PdfGenerator();
    }

    [HttpGet]
    public async Task<IActionResult> GenerarReporte(Guid clienteId, DateTime desde, DateTime hasta, string formato = "json")
    {
        try
        {
            var cuentas = await _cuentas.GetByClienteIdAsync(clienteId);
            if (!cuentas.Any())
                return NotFound(new { error = "No se encontraron cuentas para el cliente" });

            var reporte = new List<object>();

            foreach (var cuenta in cuentas)
            {
                var movimientos = await _movimientos.GetByCuentaIdAndRangoFechasAsync(cuenta.CuentaId, desde, hasta);
                var totalCreditos = movimientos.Where(m => m.TipoMovimiento == "crédito").Sum(m => m.Valor);
                var totalDebitos = movimientos.Where(m => m.TipoMovimiento == "débito").Sum(m => m.Valor);

                reporte.Add(new
                {
                    cuenta.NumeroCuenta,
                    cuenta.TipoCuenta,
                    Saldo = cuenta.SaldoInicial,
                    TotalCreditos = totalCreditos,
                    TotalDebitos = totalDebitos,
                    Movimientos = movimientos.Select(m => new
                    {
                        m.Fecha,
                        m.TipoMovimiento,
                        m.Valor,
                        m.Saldo
                    })
                });
            }

            if (formato == "pdf")
            {
                var pdfBytes = GenerarPDFDesdeHTML(reporte);
                var base64 = Convert.ToBase64String(pdfBytes);
                return Ok(new { pdfBase64 = base64 });
            }

            return Ok(reporte);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private byte[] GenerarPDFDesdeHTML(List<object> reporte)
    {
        var sb = new StringBuilder();

        // Estilos CSS para mejorar la presentación
        sb.Append(@"
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        h1 { color: #2c3e50; text-align: center; }
        .cuenta { 
            background-color: #f8f9fa; 
            border: 1px solid #dee2e6; 
            border-radius: 5px; 
            padding: 15px; 
            margin-bottom: 20px;
        }
        .cuenta-header { 
            display: flex; 
            justify-content: space-between; 
            border-bottom: 1px solid #dee2e6; 
            padding-bottom: 10px; 
            margin-bottom: 10px;
        }
        .movimiento { 
            display: flex; 
            justify-content: space-between; 
            padding: 8px 0; 
            border-bottom: 1px dotted #ddd;
        }
        .resumen { 
            background-color: #e9ecef; 
            padding: 10px; 
            border-radius: 5px; 
            margin-top: 10px;
        }
        .fecha { width: 120px; }
        .tipo { width: 100px; }
        .valor { width: 100px; text-align: right; }
        .saldo { width: 100px; text-align: right; }
        .negativo { color: #dc3545; }
        .positivo { color: #28a745; }
        table { width: 100%; border-collapse: collapse; }
        th { background-color: #343a40; color: white; padding: 8px; }
        td { padding: 8px; border-bottom: 1px solid #ddd; }
    </style>
    ");

        // Encabezado del reporte
        sb.Append("<h1>Reporte de Movimientos Bancarios</h1>");
        sb.Append($"<p style='text-align: right;'>Fecha de generación: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");

        foreach (var item in reporte)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSerializer.Serialize(item));

            sb.Append("<div class='cuenta'>");
            sb.Append("<div class='cuenta-header'>");
            sb.Append($"<h2>Cuenta: {dict["NumeroCuenta"].GetString()}</h2>");
            sb.Append($"<span>Tipo: {dict["TipoCuenta"].GetString()}</span>");
            sb.Append("</div>");

            sb.Append("<div class='resumen'>");
            sb.Append($"<p>Saldo Inicial: {dict["Saldo"].GetDecimal().ToString("C")}</p>");
            sb.Append($"<p>Total Créditos: <span class='positivo'>{dict["TotalCreditos"].GetDecimal().ToString("C")}</span></p>");
            sb.Append($"<p>Total Débitos: <span class='negativo'>{dict["TotalDebitos"].GetDecimal().ToString("C")}</span></p>");
            sb.Append($"<p><strong>Saldo Final: {dict["Saldo"].GetDecimal().ToString("C")}</strong></p>");
            sb.Append("</div>");

            if (dict["Movimientos"].ValueKind == JsonValueKind.Array && dict["Movimientos"].GetArrayLength() > 0)
            {
                sb.Append("<h3>Detalle de Movimientos</h3>");
                sb.Append("<table>");
                sb.Append("<tr><th>Fecha</th><th>Tipo</th><th>Valor</th><th>Saldo</th></tr>");

                foreach (var mov in dict["Movimientos"].EnumerateArray())
                {
                    var m = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(mov.GetRawText());
                    var tipoMovimiento = m["TipoMovimiento"].GetString();
                    var esDebito = tipoMovimiento == "Débito";

                    sb.Append("<tr>");
                    sb.Append($"<td class='fecha'>{m["Fecha"].GetDateTime().ToString("dd/MM/yyyy")}</td>");
                    sb.Append($"<td class='tipo'>{tipoMovimiento}</td>");
                    sb.Append($"<td class='valor {(esDebito ? "negativo" : "positivo")}'>");
                    sb.Append($"{(esDebito ? "-" : "+")}{m["Valor"].GetDecimal().ToString("C")}</td>");
                    sb.Append($"<td class='saldo'>{m["Saldo"].GetDecimal().ToString("C")}</td>");
                    sb.Append("</tr>");
                }

                sb.Append("</table>");
            }
            else
            {
                sb.Append("<p>No se encontraron movimientos en el período seleccionado</p>");
            }

            sb.Append("</div>");
        }

        return _pdfGenerator.Generar(sb.ToString());
    }

}
