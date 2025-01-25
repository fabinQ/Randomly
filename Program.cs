using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Randomly;

public class Program
{
    private static string GetSubfolder(string dir){
        string subfolder = Path.GetDirectoryName(dir);
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

    static void Main(string[] args)
    {
        // Wgranie pliku
        System.Console.WriteLine("Hello in Randomly! Let's check the numbers. Show me your file...");
        string pdfPath = @"C:\Users\Hyperbook\Documents\Maciej\Randomly\src\Lotto chybił trafił.pdf";
        Globals.PdfPath = pdfPath;

        // Tworzenie podfolderu
        string subfolder = GetSubfolder(pdfPath);
        Globals.Subfolder = subfolder;

        // Konwersja na png
        PdfConverter pdfConverter = new PdfConverter();
        // Tworzenie wątków
        Thread pdfThread = new Thread(() => pdfConverter.PdfToImages(pdfPath));
        

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
        pdfThread.Start();

        foreach (var imagePath in GetNewJpgFiles(subfolder))
        {
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

        pdfThread.Join();
    }

        private static IEnumerable<string> GetNewJpgFiles(string folderPath)
        {
        var existingFiles = new HashSet<string>(Directory.GetFiles(folderPath, "*.jpg"));
        while (true)
        {
            var currentFiles = new HashSet<string>(Directory.GetFiles(folderPath, "*.jpg"));
            var newFiles = currentFiles.Except(existingFiles).ToList();

            foreach (var newFile in newFiles)
            {
            yield return newFile;
            existingFiles.Add(newFile);
            }

            if (newFiles.Count == 0 && !Directory.GetFiles(folderPath, "*.jpg").Any())
            {
            yield break;
            }

            Thread.Sleep(500); // Adjust the sleep time as needed
        }
        }
}
