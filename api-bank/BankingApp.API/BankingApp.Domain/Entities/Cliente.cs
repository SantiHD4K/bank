using System.ComponentModel.DataAnnotations;

namespace BankingApp.API.BankingApp.Domain.Entities
{
    public class Cliente : Persona
    {
        private string _contraseña = string.Empty;

        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "La contraseña debe tener 4 caracteres")]
        public string Contraseña
        {
            get => _contraseña;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length != 4)
                    throw new ArgumentException("La contraseña debe tener 4 caracteres.");
                _contraseña = value;
            }
        }

        public bool Estado { get; set; }
    }
}
