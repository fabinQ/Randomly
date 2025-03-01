using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Randomly
{
    public class Program
    {
        private static Thread pdfThread;
        private static Thread extractedTextThread;
        private static readonly ConcurrentQueue<string> newFiles = new();
        private static string folderPath;

        private static IEnumerable<string> Watcher()
        {
            folderPath = Globals.Subfolder ?? throw new ArgumentNullException("Subfolder cannot be null");

            Console.WriteLine($"Watching for new PNG files in '{folderPath}'");

            using var watcher = new FileSystemWatcher(folderPath, "*" + Globals.GetExtendedFromConverionedFile);
            watcher.Created += (sender, e) =>
            {
                Console.WriteLine($"New file: {e.FullPath}");
                newFiles.Enqueue(e.FullPath);
            };
            watcher.EnableRaisingEvents = true;        
            
            while (true)
            {
                while (newFiles.TryDequeue(out var newFile))
                {
                    yield return newFile;
                }
                // Thread.Sleep(500);
            }
        }
        private static IEnumerable<string> GetFiles()
        {
            var processedFiles = new HashSet<string>();
            if (Directory.EnumerateFileSystemEntries(folderPath).Any())
            {
                foreach (var file in Directory.EnumerateFiles(folderPath, "*" + Globals.GetExtendedFromConverionedFile))
                {
                if (processedFiles.Add(file))
                {
                    yield return file;
                }
                }
            }

            while (true)
            {
            while (newFiles.TryDequeue(out var newFile))
            {
                if (processedFiles.Add(newFile))
                {
                yield return newFile;
                }
            }
            Thread.Sleep(500);
            }
        }

        private static void ExtractTextFromImages()
        {
            OCReader oCReader = new();
            while (true)
            {
                var imagePath1 = GetFiles();
                string imagePath = imagePath1.FirstOrDefault();
                Console.WriteLine($"Performing OCR on {imagePath}...");
                
                string extractedText = oCReader.PerformOCR(imagePath);
                oCReader.SaveTextToFile(extractedText, imagePath);
                
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello in Randomly! Let's check the numbers. Show me your file...");
            string pdfPath = @"C:\Users\Hyperbook\Documents\Maciej\Randomly\src\Lotto chybił trafił.pdf";
            Globals.PdfPath = pdfPath;

            PdfConverter pdfConverter = new PdfConverter();
            PdfConverter.CreateSubfolder(pdfPath);

            pdfThread = new Thread(() => pdfConverter.PdfToImages(pdfPath));
            pdfThread.Name = "PdfThread";
            pdfThread.Start();

            extractedTextThread = new Thread(ExtractTextFromImages);
            extractedTextThread.Name = "ExtractedTextThread";
            extractedTextThread.Start();

        }
    }
}
