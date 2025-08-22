using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaMedica.Models
{
    public class Consulta
    {
        [Key]
        public int ConsultaId { get; set; }

        public DateTime DataConsulta { get; set; }

        // FK Paciente
        [ForeignKey("Paciente")]
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        // FK Medico
        [ForeignKey("Medico")]
        public int MedicoId { get; set; }
        public Medico Medico { get; set; }

        // Relacionado ao prontuário
        public string Diagnostico { get; set; }
        public string Receita { get; set; }
    }
}
