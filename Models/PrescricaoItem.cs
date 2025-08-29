namespace ConsultorioMedico.Api.Models
{
    public class PrescricaoItem
    {
        public Guid Id { get; set; }
        public Guid ConsultaId { get; set; }
        public Consulta Consulta { get; set; } = default!;


        public string Medicamento { get; set; } = default!;
        public string Posologia { get; set; } = default!; // dose e frequência
        public string Orientacoes { get; set; } = string.Empty;
    }
}
