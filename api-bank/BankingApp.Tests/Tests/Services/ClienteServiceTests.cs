using BankingApp.API.BankingApp.Application.Application.Services;
using BankingApp.API.BankingApp.Domain.Entities;
using BankingApp.API.BankingApp.Infrastructure.Repositories.UnitOfWork;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Tests.Tests.Services
{
    public class ClienteServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ClienteService _clienteService;

        public ClienteServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _clienteService = new ClienteService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddCliente()
        {
            // Arrange
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nombre = "Jose Lema",
                Genero = "Masculino",
                Edad = 30,
                Identificacion = "123456789",
                Direccion = "Otavalo",
                Telefono = "0982548785",
                Contraseña = "1234",
                Estado = true
            };

            _unitOfWorkMock.Setup(u => u.Clientes.AddAsync(cliente))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _clienteService.AddAsync(cliente);

            // Assert
            _unitOfWorkMock.Verify(u => u.Clientes.AddAsync(cliente), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCliente()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new Cliente { Id = id, Nombre = "Marianela" };

            _unitOfWorkMock.Setup(u => u.Clientes.GetByIdAsync(id))
                .ReturnsAsync(expected);

            // Act
            var result = await _clienteService.GetByIdAsync(id);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
