using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankingApp.API.BankingApp.Domain.Entities
{
    public class Movimiento
    {
        [Key]
        public Guid MovimientoId { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        private string _tipoMovimiento = string.Empty;
        [Required]
        public string TipoMovimiento
        {
            get => _tipoMovimiento;
            set
            {
                var tipo = value?.ToLower().Trim();
                if (tipo != "crédito" && tipo != "débito")
                    throw new ArgumentException("El tipo de movimiento debe ser 'crédito' o 'débito'.");
                _tipoMovimiento = tipo;
            }
        }

        private decimal _valor;
        [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser mayor a 0")]
        public decimal Valor
        {
            get => _valor;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("El valor debe ser mayor a 0.");
                _valor = value;
            }
        }

        public decimal Saldo { get; set; }

        [Required]
        public Guid CuentaId { get; set; }
        
        [JsonIgnore]
        public Cuenta? Cuenta { get; set; }
    }
}
