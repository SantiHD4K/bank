using BankingApp.API.BankingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BankingApp.API.BankingApp.Infrastructure.Data
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>().HasKey(c => c.Id);

            modelBuilder.Entity<Cuenta>().HasKey(c => c.CuentaId);
            modelBuilder.Entity<Movimiento>().HasKey(m => m.MovimientoId);

            modelBuilder.Entity<Cuenta>()
                .HasOne(c => c.Cliente)
                .WithMany()
                .HasForeignKey(c => c.ClienteId);

            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.Cuenta)
                .WithMany()
                .HasForeignKey(m => m.CuentaId);
        }
    }


}
