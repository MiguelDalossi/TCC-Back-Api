using System.ComponentModel.DataAnnotations;

namespace AgendaMedica.Models
{
    public class Medico
    {
        [Key]
        public int MedicoId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(50)]
        public string CRM { get; set; }

        [MaxLength(100)]
        public string Especialidade { get; set; }
    }
}