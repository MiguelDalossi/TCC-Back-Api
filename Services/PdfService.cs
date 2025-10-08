using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ConsultorioMedico.Api.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ConsultorioMedico.Api.Services
{
    public class PdfService
    {
        public PdfService()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }
        public Task<byte[]> GerarPrescricaoAsync(string paciente, string medico, List<PrescricaoItemDto> itens)
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4); // Define o tamanho da folha
                    page.Margin(40); // Margem em todas as bordas
                    page.PageColor(Colors.White); // Cor de fundo
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial")); // Fonte padr�o

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Prescri��o M�dica")
                                .FontSize(22)
                                .Bold()
                                .FontColor(Colors.Blue.Medium)
                                .AlignCenter();
                            col.Item().Text($"Data: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(10)
                                .AlignRight();
                        });
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text($"Paciente: {paciente}").Bold();
                        col.Item().Text($"M�dico: {medico}").Italic();
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        foreach (var item in itens)
                        {
                            col.Item().Text($"Medicamento: {item.Medicamento}").Bold();
                            col.Item().Text($"Posologia: {item.Posologia}");
                            if (!string.IsNullOrWhiteSpace(item.Orientacoes))
                                col.Item().Text($"Orienta��es: {item.Orientacoes}");
                            col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                        }
                    });

                    page.Footer().AlignCenter().Text("Assinatura do m�dico ___________________________")
                        .FontSize(10)
                        .Italic();
                });
            });

            using var ms = new MemoryStream();
            pdf.GeneratePdf(ms);
            return Task.FromResult(ms.ToArray());
        }

        public Task<byte[]> GerarProntuarioAsync(string paciente, string medico, ProntuarioDetailDto prontuario)
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.Grey.Lighten4);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Prontu�rio M�dico")
                                .FontSize(22)
                                .Bold()
                                .FontColor(Colors.Green.Darken2)
                                .AlignCenter();
                            col.Item().Text($"Data: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(10)
                                .AlignRight();
                        });
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text($"Paciente: {paciente}").Bold();
                        col.Item().Text($"M�dico: {medico}").Italic();
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().Text("Queixa Principal:").Bold();
                        col.Item().Text(prontuario.QueixaPrincipal ?? "-");

                        col.Item().Text("HDA:").Bold();
                        col.Item().Text(prontuario.Hda ?? "-");

                        col.Item().Text("Antecedentes:").Bold();
                        col.Item().Text(prontuario.Antecedentes ?? "-");

                        col.Item().Text("Exame F�sico:").Bold();
                        col.Item().Text(prontuario.ExameFisico ?? "-");

                        col.Item().Text("Hip�teses Diagn�sticas:").Bold();
                        col.Item().Text(prontuario.HipotesesDiagnosticas ?? "-");

                        col.Item().Text("Conduta:").Bold();
                        col.Item().Text(prontuario.Conduta ?? "-");
                    });

                    page.Footer().AlignCenter().Text("Assinatura do m�dico ___________________________")
                        .FontSize(10)
                        .Italic();
                });
            });

            using var ms = new MemoryStream();
            pdf.GeneratePdf(ms);
            return Task.FromResult(ms.ToArray());
        }
    }
}
