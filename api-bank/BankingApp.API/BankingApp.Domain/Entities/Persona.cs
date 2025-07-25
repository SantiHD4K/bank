using System.ComponentModel.DataAnnotations;

namespace BankingApp.API.BankingApp.Domain.Entities
{
    public abstract class Persona
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _nombre = string.Empty;
        [Required]
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El nombre es obligatorio.");
                _nombre = value;
            }
        }

        private string _genero = string.Empty;
        public string Genero
        {
            get => _genero;
            set => _genero = value?.Trim() ?? "";
        }

        private int _edad;
        [Range(18, 120, ErrorMessage = "La edad debe estar entre 18 y 120 años")]
        public int Edad
        {
            get => _edad;
            set
            {
                if (value < 18 || value > 120)
                    throw new ArgumentOutOfRangeException(nameof(Edad), "La edad debe estar entre 18 y 120 años");
                _edad = value;
            }
        }

        private string _identificacion = string.Empty;
        [Required]
        public string Identificacion
        {
            get => _identificacion;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La identificación es obligatoria.");
                _identificacion = value;
            }
        }

        private string _direccion = string.Empty;
        [Required]
        public string Direccion
        {
            get => _direccion;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La dirección es obligatoria.");
                _direccion = value;
            }
        }

        private string _telefono = string.Empty;
        [Phone]
        public string Telefono
        {
            get => _telefono;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && (value.Length < 10 || !value.All(char.IsDigit)))
                    throw new ArgumentException("El teléfono debe tener al menos 10 dígitos numéricos.");
                _telefono = value;
            }
        }
    }
}
