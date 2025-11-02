using ImperialSanAPI.DTOs.OrderDTO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ImperialSanAPI.Utils
{
    public class ReceiptService
    {
        private static readonly string _fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Fonts", "DejaVuSans.ttf");

        public static byte[] GenerateReceiptPdf(OrderReceiptDTO order)
        {
            using var stream = new MemoryStream();
            var document = new Document(PageSize.A4);
            var writer = PdfWriter.GetInstance(document, stream);

            document.Open();

            var baseFont = BaseFont.CreateFont(_fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var fontHeader = new Font(baseFont, 14, Font.BOLD);
            var fontNormal = new Font(baseFont, 10, Font.NORMAL);

            document.Add(new Paragraph($"Заказ №{order.OrderId} от {order.DateOfCreate:dd.MM.yyyy}", fontHeader)
            {
                Alignment = Element.ALIGN_CENTER
            });
            document.Add(new Paragraph($"Адрес получения заказа: {order.DeliveryAddress}", fontNormal));
            document.Add(Chunk.NEWLINE);

            var table = new PdfPTable(6) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 5, 10, 40, 15, 15, 15 });

            AddCell(table, "№", fontNormal, Element.ALIGN_CENTER);
            AddCell(table, "Код", fontNormal, Element.ALIGN_CENTER);
            AddCell(table, "Наименование товара", fontNormal, Element.ALIGN_CENTER);
            AddCell(table, "Цена", fontNormal, Element.ALIGN_CENTER);
            AddCell(table, "Кол-во", fontNormal, Element.ALIGN_CENTER);
            AddCell(table, "Сумма", fontNormal, Element.ALIGN_CENTER);

            for (int i = 0; i < order.Positions.Count; i++)
            {
                var pos = order.Positions[i];
                AddCell(table, (i + 1).ToString(), fontNormal, Element.ALIGN_CENTER);
                AddCell(table, pos.ProductId.ToString(), fontNormal, Element.ALIGN_CENTER);
                AddCell(table, pos.ProductTitle, fontNormal, Element.ALIGN_LEFT);
                AddCell(table, pos.Price.ToString("N2"), fontNormal, Element.ALIGN_RIGHT);
                AddCell(table, pos.Quantity.ToString(), fontNormal, Element.ALIGN_CENTER);
                AddCell(table, pos.Total.ToString("N2"), fontNormal, Element.ALIGN_RIGHT);
            }

            var totalCell = new PdfPCell(new Phrase("Сумма:", fontNormal))
            { Colspan = 5, HorizontalAlignment = Element.ALIGN_RIGHT };
            table.AddCell(totalCell);
            var totalValueCell = new PdfPCell(new Phrase(order.TotalPrice.ToString("N2"), fontNormal))
            { HorizontalAlignment = Element.ALIGN_RIGHT };
            table.AddCell(totalValueCell);

            document.Add(table);
            document.Close();
            return stream.ToArray();
        }

        private static void AddCell(PdfPTable table, string text, Font font, int alignment)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = alignment,
                Padding = 5
            };
            table.AddCell(cell);
        }
    }
}
