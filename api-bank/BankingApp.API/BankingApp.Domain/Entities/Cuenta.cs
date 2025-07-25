using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankingApp.API.BankingApp.Domain.Entities
{
    public class Cuenta
    {
        [Key]
        public Guid CuentaId { get; set; }

        private string _numeroCuenta = string.Empty;
        [Required]
        public string NumeroCuenta
        {
            get => _numeroCuenta;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 6 || value.Length > 12)
                    throw new ArgumentException("El número de cuenta debe tener entre 6 y 12 caracteres.");
                _numeroCuenta = value;
            }
        }

        private string _tipoCuenta = string.Empty;
        [Required]
        public string TipoCuenta
        {
            get => _tipoCuenta;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El tipo de cuenta es obligatorio.");
                _tipoCuenta = value;
            }
        }

        private decimal _saldoInicial;
        [Range(0, double.MaxValue, ErrorMessage = "El saldo debe ser igual o mayor a 0")]
        public decimal SaldoInicial
        {
            get => _saldoInicial;
            set
            {
                if (value < 0)
                    throw new ArgumentException("El saldo inicial no puede ser negativo.");
                _saldoInicial = value;
            }
        }

        public bool Estado { get; set; }

        [Required]
        public Guid ClienteId { get; set; }

        [JsonIgnore]
        public virtual Cliente? Cliente { get; set; }
    }
}
