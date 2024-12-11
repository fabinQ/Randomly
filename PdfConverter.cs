using System.Drawing;
using PdfiumViewer;

namespace PdfConv;

public class PdfConverter
{
    public List<string> PdfToImages(string pdfPath)
    {
        List<string> imagesPaths = new List<string>();

        try
        {
            // Otwórz dokument PDF
            using(PdfDocument pdfDocument = PdfDocument.Load(pdfPath))
            {
            string subfolder = GetSubfolder(pdfPath);
            // Iteruj przez strony PDF
            for (int i = 0; i < pdfDocument.PageCount; i++)
            {
                // Utwórz obraz dla strony
                using (Bitmap bitmap = RenderPdfPageToBitmap(pdfDocument, i, 300))
                {
                    string imagePath = $"{subfolder}\\Page_{i + 1}.png"; // Nazwa pliku
                    bitmap.Save(imagePath); // Zapisz jako PNG
                    imagesPaths.Add(imagePath);
                    Console.WriteLine($"Strona {i + 1} zapisana jako obraz: {Path.GetFileName(imagePath)}");
                }
            }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas konwersji PDF: {ex.Message}");
        }

        return imagesPaths;
    }
    
    private static Bitmap RenderPdfPageToBitmap(PdfDocument page, int pageIndex, int dpi)
    {
        return (Bitmap)page.Render(pageIndex, dpi, dpi, PdfRenderFlags.CorrectFromDpi);
    }
    private string GetSubfolder(string dir){
        string subfolder = Path.GetDirectoryName(dir);
        // string fileName = Path.GetFileName
        subfolder = Path.Join(subfolder, Path.GetFileNameWithoutExtension(dir));
        if (!Directory.Exists(subfolder))
        {
            System.Console.WriteLine($"Creating folter: '{subfolder}'");
            Directory.CreateDirectory(subfolder);
        }
        else
        {
            System.Console.WriteLine($"'{subfolder}' already exist");
        }
        return subfolder;
    }
}
