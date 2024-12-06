using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfConv;

public class PdfConverter
{
    public List<string> PdfToImages(string pdfPath)
    {
        List<string> imagesPaths = new List<string>();

        try
        {
            // Otwórz dokument PDF
            PdfDocument document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.ReadOnly);

            // Iteruj przez strony PDF
            for (int i = 0; i < document.PageCount; i++)
            {
                PdfPage page = document.Pages[i];

                // Utwórz obraz dla strony
                using (var bitmap = RenderPdfPageToBitmap(page, 300, 300))
                {
                    string imagePath = $"Page_{i + 1}.png"; // Nazwa pliku
                    bitmap.Save(imagePath, XImageFormat.Png); // Zapisz jako PNG
                    imagesPaths.Add(imagePath);

                    Console.WriteLine($"Strona {i + 1} zapisana jako obraz: {imagePath}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas konwersji PDF: {ex.Message}");
        }

        return imagesPaths;
    }
    
    private static Bitmap RenderPdfPageToBitmap(PdfPage page, int dpiX, int dpiY)
    {
        // Rozmiar strony w pikselach
        double width = page.Width.Point * dpiX / 72;
        double height = page.Height.Point * dpiY / 72;

        Bitmap bitmap = new Bitmap((int)width, (int)height);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.White);
            graphics.DrawString(
                "TODO: Renderowanie treści PDF",
                new Font("Arial", 12),
                Brushes.Black,
                new PointF(10, 10));
        }

        return bitmap;
    }
}
