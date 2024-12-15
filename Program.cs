using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Randomly;

public class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine("Hello in Randomly! Let's check the numbers. Show me your file...");
        string pdfPath = @"C:\Users\Hyperbook\Documents\Maciej\Randomly\src\Lotto chybił trafił.pdf";
        Globals.PdfPath = pdfPath;

        PdfConverter pdfConverter = new PdfConverter();

        // Konwersja na png
        List<string> images = pdfConverter.PdfToImages(pdfPath);

        // OCR
        OCReader oCReader = new();
        foreach (string imagePath in images)
        {
            string exctractedText = oCReader.PerformOCR(imagePath);

            if (!string.IsNullOrWhiteSpace(exctractedText))
            {
                oCReader.SaveTextToFile(exctractedText, imagePath);
            }
            else
            {
                System.Console.WriteLine($"No text found in {imagePath}.");
            }
        }
    }


}
