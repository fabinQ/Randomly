using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Randomly;

public class Program
{
    private static IEnumerable<string> GetNewPngFiles()
    {
    string folderPath = Globals.Subfolder ?? throw new ArgumentNullException("Subfolder cannot be null");
    var existingFiles = new HashSet<string>(Directory.GetFiles(folderPath, "*"+Globals.GetExtendedFromConverionedFile));
    var newFiles = new List<string>();
    System.Console.WriteLine($"Watching for new PNG files in '{folderPath}'");

    using (var watcher = new FileSystemWatcher(folderPath, "*"+Globals.GetExtendedFromConverionedFile))
    {
        watcher.Created += (sender, e) =>
        {
            if (!existingFiles.Contains(e.FullPath))
            {
                newFiles.Add(e.FullPath);
                existingFiles.Add(e.FullPath);
            }
        };
        watcher.EnableRaisingEvents = true;

        while (true)
        {
            if (newFiles.Count > 0)
            {
                foreach (var newFile in newFiles.ToList())
                {
                    yield return newFile;
                    newFiles.Remove(newFile);
                }
            }
            else if (!Directory.GetFiles(folderPath, Globals.GetExtendedFromConverionedFile).Any())
            {
                yield break;
            }
            Thread.Sleep(100); // Adjust the sleep time as needed
        }
    }
    }
    static void Main(string[] args)
    {
        // Wgranie pliku
        System.Console.WriteLine("Hello in Randomly! Let's check the numbers. Show me your file...");
        string pdfPath = @"C:\Users\Hyperbook\Documents\Maciej\Randomly\src\Lotto chybił trafił.pdf";
        Globals.PdfPath = pdfPath;

        // Konwersja na png
        PdfConverter pdfConverter = new PdfConverter();

        // Tworzenie podfolderu
        PdfConverter.CreateSubfolder(pdfPath);

        // Tworzenie wątków
        Thread pdfThread = new Thread(() => pdfConverter.PdfToImages(pdfPath));
        pdfThread.Start();
        

        // OCR
        string exctractedText;
        OCReader oCReader = new();

        // foreach (string imagePath in subfolder)
        // {
        //     exctractedText = oCReader.PerformOCR(imagePath);

        //     if (!string.IsNullOrWhiteSpace(exctractedText))
        //     {
        //         oCReader.SaveTextToFile(exctractedText, imagePath);
        //     }
        //     else
        //     {
        //         System.Console.WriteLine($"No text found in {imagePath}.");
        //     }
        // }

        foreach (var imagePath in GetNewPngFiles())
        {
            System.Console.WriteLine($"Performing OCR on {imagePath}...");
            exctractedText = oCReader.PerformOCR(imagePath);

            if (!string.IsNullOrWhiteSpace(exctractedText))
            {
            oCReader.SaveTextToFile(exctractedText, imagePath);
            }
            else
            {
            System.Console.WriteLine($"No text found in {imagePath}.");
            }
        }
        System.Console.WriteLine("Before join");
        pdfThread.Join();
        System.Console.WriteLine("After join");
    }
}
