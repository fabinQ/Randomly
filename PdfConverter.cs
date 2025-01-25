using System.Drawing;
using PdfiumViewer;

namespace Randomly;

public class PdfConverter
{

    public List<string> imagesPaths { get; set; } = new List<string>();

    public void PdfToImages(string pdfPath)
    {
        List<string> imagesPaths = new List<string>();
        
        try
        {
            // Otwórz dokument PDF
            using(PdfDocument pdfDocument = PdfDocument.Load(pdfPath))
            {
            // Iteruj przez strony PDF
            for (int i = 1; i <= pdfDocument.PageCount; i++)
            {
                // Utwórz obraz dla strony
                using (Bitmap bitmap = RenderPdfPageToBitmap(pdfDocument, i-1, 300))
                {
                    string imagePath = $"{Globals.Subfolder}\\Page_{i}.png"; // Nazwa pliku
                    bitmap.Save(imagePath); // Zapisz jako PNG
                    imagesPaths.Add(imagePath);
                    Console.WriteLine($"Strona {i} zapisana jako obraz: {Path.GetFileName(imagePath)}");
                }
            }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas konwersji PDF: {ex.Message}");
        }

    }
    
    private static Bitmap RenderPdfPageToBitmap(PdfDocument page, int pageIndex, int dpi)
    {
        return (Bitmap)page.Render(pageIndex, dpi, dpi, PdfRenderFlags.CorrectFromDpi);
    }
    

}
