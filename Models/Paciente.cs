using System.ComponentModel.DataAnnotations;

namespace AgendaMedica.Models
{
    public class Paciente
    {
        [Key]
        public int PacienteId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(11)]
        public string CPF { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        public DateTime DataNascimento { get; set; }
    }
}
