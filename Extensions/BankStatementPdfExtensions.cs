using BankStatement.Demo.DTOs;
using BankStatement.Demo.Extensions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BankStatement.Demo.Extensions
{

    public static class BankStatementPdfExtensions
    {

        public static byte[] GeneratePdf(this BankStatementDto statement)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    page.Header().Element(c => ComposeHeader(c, statement));
                    page.Content().Element(c => ComposeContent(c, statement));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        private static void ComposeHeader(IContainer container, BankStatementDto statement)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(statement.InstitutionName)
                       .FontSize(20).Bold().FontColor(Colors.Blue.Darken3);
                    col.Item().Text($"Routing: {statement.RoutingNumber}");
                });

                row.ConstantItem(200).AlignRight().Column(col =>
                {
                    col.Item().Text("ACCOUNT STATEMENT").FontSize(14).Bold();
                    col.Item().Text($"Statement ID: {statement.StatementId}");
                    col.Item().Text($"Period: {statement.StartDate:yyyy-MM-dd} to {statement.EndDate:yyyy-MM-dd}");
                });
            });
        }

        private static void ComposeContent(IContainer container, BankStatementDto statement)
        {
            container.PaddingVertical(10).Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                col.Item().PaddingVertical(10).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Account Holder: {statement.AccountHolder}").Bold();
                        c.Item().Text($"Account: {statement.AccountNumberMasked} ({statement.AccountType})");
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Starting Balance: {statement.StartingBalance:C}");
                        c.Item().Text($"Ending Balance: {statement.EndingBalance:C}").Bold();
                    });
                });

                col.Item().PaddingTop(10).Text("Transactions").FontSize(12).Bold();

                col.Item().PaddingTop(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(80);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                        columns.ConstantColumn(80);
                    });

                    table.Header(header =>
                    {
                        header.Cell().BorderBottom(1).Text("Date").Bold();
                        header.Cell().BorderBottom(1).Text("Merchant / Description").Bold();
                        header.Cell().BorderBottom(1).Text("Category").Bold();
                        header.Cell().BorderBottom(1).AlignRight().Text("Amount").Bold();
                    });

                    foreach (var tx in statement.Transactions)
                    {
                        var amountColor = tx.Type == "CREDIT" ? Colors.Green.Darken2 : Colors.Grey.Darken3;

                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(tx.Timestamp.ToString("yyyy-MM-dd"));
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(tx.Merchant?.Name ?? "N/A");
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Text(tx.Category);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{tx.Amount:C}").FontColor(amountColor);
                    }
                });
            });
        }
    }
}
