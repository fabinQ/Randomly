using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Randomly;

public class Program
{
    private static string GetSubfolder(string dir){
        string subfolder = Path.GetDirectoryName(dir);
        subfolder = Path.Combine(subfolder, Path.GetFileNameWithoutExtension(dir));
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

        foreach (var imagePath in GetNewPngFiles(subfolder))
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

        private static IEnumerable<string> GetNewPngFiles(string folderPath)
        {
            var existingFiles = new HashSet<string>(Directory.GetFiles(folderPath, "*.png"));
            var newFiles = new List<string>();
            using (var watcher = new FileSystemWatcher(folderPath, "*.png"))
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
                    else if (!Directory.GetFiles(folderPath, "*.png").Any())
                    {
                        yield break;
                    }
                    Thread.Sleep(100); // Adjust the sleep time as needed
                }
            }
        }
}
