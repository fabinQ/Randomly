using System.Collections.Generic;
using PdfConv;

namespace Randomly;

public class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine("Hello in Randomly! Let's check the numbers. Show me your file...");
        string pdfPath = @"C:\Users\Hyperbook\Documents\Maciej\Randomly\src\Lotto chybił trafił.pdf";

        PdfConverter pdfConverter = new PdfConverter();

        List<string> images = pdfConverter.PdfToImages(pdfPath);

        
    }


}
