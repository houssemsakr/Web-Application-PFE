using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Web_Application_PFE.Controllers
{
    public class RapportController : Controller
    {
        public IActionResult Rapport()
        {
            return View();
        }

        public IActionResult Generer_un_rapport()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generer_un_rapport(string periode, string client, string performance,
            string secteur, string statutRFQ, string graphique)
        {
            // Stocker les données dans TempData pour les réutiliser
            TempData["Periode"] = periode;
            TempData["Client"] = client;
            TempData["Performance"] = performance;
            TempData["Secteur"] = secteur;
            TempData["StatutRFQ"] = statutRFQ;
            TempData["Graphique"] = graphique;

            return RedirectToAction("Telecharger_Rapport");
        }

        public IActionResult Telecharger_Rapport()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Telecharger_Rapport(string format)
        {
            // Récupérer les données depuis TempData
            var periode = TempData["Periode"]?.ToString();
            var client = TempData["Client"]?.ToString();
            var performance = TempData["Performance"]?.ToString();
            var secteur = TempData["Secteur"]?.ToString();
            var statutRFQ = TempData["StatutRFQ"]?.ToString();
            var graphique = TempData["Graphique"]?.ToString();

            // Simuler des données pour l'exemple
            var data = new List<dynamic>
            {
                new { Période = periode, Client = client, Performance = performance,
                      Secteur = secteur, StatutRFQ = statutRFQ, Graphique = graphique }
            };

            if (format == "excel")
            {
                return GenerateExcel(data);
            }
            else if (format == "pdf")
            {
                return GeneratePdf(data);
            }

            return RedirectToAction("Telecharger_Rapport");
        }

        private IActionResult GenerateExcel(IEnumerable<dynamic> data)
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Rapport");

                // Entêtes
                worksheet.Cells[1, 1].Value = "Période";
                worksheet.Cells[1, 2].Value = "Client";
                worksheet.Cells[1, 3].Value = "Performance";
                worksheet.Cells[1, 4].Value = "Secteur";
                worksheet.Cells[1, 5].Value = "Statut RFQ";
                worksheet.Cells[1, 6].Value = "Graphique";

                // Données
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Période;
                    worksheet.Cells[row, 2].Value = item.Client;
                    worksheet.Cells[row, 3].Value = item.Performance;
                    worksheet.Cells[row, 4].Value = item.Secteur;
                    worksheet.Cells[row, 5].Value = item.StatutRFQ;
                    worksheet.Cells[row, 6].Value = item.Graphique;
                    row++;
                }

                package.Save();
            }

            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Rapport.xlsx");
        }

        private IActionResult GeneratePdf(IEnumerable<dynamic> data)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var stream = new MemoryStream();

            // Créer une classe concrète pour stocker les données
            var pdfItems = new List<PdfItem>();
            foreach (var item in data)
            {
                pdfItems.Add(new PdfItem
                {
                    Periode = item.Période?.ToString() ?? "-",
                    Client = item.Client?.ToString() ?? "-",
                    Performance = item.Performance?.ToString() ?? "-",
                    Secteur = item.Secteur?.ToString() ?? "-",
                    StatutRFQ = item.StatutRFQ?.ToString() ?? "-",
                    Graphique = item.Graphique?.ToString() ?? "-"
                });
            }

            Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .AlignCenter()
                        .Text("Rapport détaillé")
                        .FontSize(20).Bold();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // En-têtes
                            table.Header(header =>
                            {
                                header.Cell().Element(Block).Text("Période").Bold();
                                header.Cell().Element(Block).Text("Client").Bold();
                                header.Cell().Element(Block).Text("Performance").Bold();
                                header.Cell().Element(Block).Text("Secteur").Bold();
                                header.Cell().Element(Block).Text("Statut RFQ").Bold();
                                header.Cell().Element(Block).Text("Graphique").Bold();

                                header.Cell().Background(Colors.Grey.Lighten3);
                            });

                            // Données
                            foreach (var item in pdfItems)
                            {
                                table.Cell().Element(Block).Text(item.Periode);
                                table.Cell().Element(Block).Text(item.Client);
                                table.Cell().Element(Block).Text(item.Performance);
                                table.Cell().Element(Block).Text(item.Secteur);
                                table.Cell().Element(Block).Text(item.StatutRFQ);
                                table.Cell().Element(Block).Text(item.Graphique);
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" sur ");
                            text.TotalPages();
                        });
                });
            })
            .GeneratePdf(stream);

            stream.Position = 0;
            return File(stream, "application/pdf", "Rapport.pdf");
        }

        // Méthode helper pour les éléments de tableau
        static IContainer Block(IContainer container)
        {
            return container
                .Border(1)
                .Background(Colors.White)
                .Padding(5)
                .AlignCenter()
                .AlignMiddle();
        }

        // Classe pour représenter les données PDF
        private class PdfItem
        {
            public string Periode { get; set; }
            public string Client { get; set; }
            public string Performance { get; set; }
            public string Secteur { get; set; }
            public string StatutRFQ { get; set; }
            public string Graphique { get; set; }
        }
    }
}