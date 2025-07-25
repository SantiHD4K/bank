using BankingApp.API.BankingApp.Application.Application.Services;
using BankingApp.API.BankingApp.Domain.Entities;
using BankingApp.API.BankingApp.Infrastructure.Repositories.UnitOfWork;
using FluentAssertions;
using Moq;
using Xunit;

public class MovimientoServiceTests
{
    [Fact]
    public async Task AddMovimientoAsync_ShouldThrow_WhenSaldoInsuficiente()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();

        var cuenta = new Cuenta
        {
            CuentaId = cuentaId,
            SaldoInicial = 100
        };

        var movimiento = new Movimiento
        {
            TipoMovimiento = "débito",
            Valor = 150 // mayor al saldo actual
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        unitOfWorkMock.Setup(u => u.Cuentas.GetByIdAsync(cuentaId))
                      .ReturnsAsync(cuenta);

        unitOfWorkMock.Setup(u => u.Movimientos.ObtenerTotalDebitosHoyAsync(cuentaId, It.IsAny<DateTime>()))
                      .ReturnsAsync(0);

        var movimientoService = new MovimientoService(unitOfWorkMock.Object);

        // Act
        Func<Task> act = async () => await movimientoService.AddMovimientoAsync(cuentaId, movimiento);

        // Assert
        await act.Should().ThrowAsync<Exception>()
                 .WithMessage("Saldo no disponible");
    }
}
