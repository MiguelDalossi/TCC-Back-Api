namespace ConsultorioMedico.Api.Dtos
{
    public record ControleFinanceiroDto(
    Guid Id,
    DateTime Data,
    decimal Valor,
    Guid ConsultaId,
    string PacienteNome,
    string MedicoNome,
    Guid MedicoId // NOVO, se desejar
);

    public record ControleFinanceiroCreateDto(
        DateTime Data,
        decimal Valor,
        Guid ConsultaId
    );
}
