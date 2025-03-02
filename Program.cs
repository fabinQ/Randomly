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
        private static readonly ManualResetEvent newFileEvent = new(false);
        private static string folderPath;

        private static IEnumerable<string> Watcher()
        {
            folderPath = Globals.Subfolder;

            Console.WriteLine($"Watching for new PNG files in '{folderPath}'");

            var watcher = new FileSystemWatcher(folderPath, "*" + Globals.GetExtendedFromConverionedFile);
            var newFilePaths = new List<string>();
            var fileCreatedEvent = new ManualResetEvent(false);

            watcher.Created += (sender, e) =>
            {
                Console.WriteLine($"New file: {e.FullPath}");
                newFilePaths.Add(e.FullPath);
                fileCreatedEvent.Set();
            };
            watcher.EnableRaisingEvents = true;

            fileCreatedEvent.WaitOne();
            return newFilePaths;
        }
        private static IEnumerable<string> GetFiles()
        {
            var processedFiles = new HashSet<string>();
            folderPath = Globals.Subfolder;
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
            else
            {
                foreach (var file in Watcher())
                {
                    yield return file;
                }

            }
        }

        private static void ExtractTextFromImages()
        {
            OCReader oCReader = new();
            foreach(var imagePath in GetFiles())
            {
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
            // extractedTextThread.Join();

        }
    }
}
