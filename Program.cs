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
        private static Thread watchThread;

        private static readonly ConcurrentQueue<string> newFiles = new();
        private static readonly HashSet<string> existingFiles = new();
        private static string folderPath;

        private static void Watcher()
        {
            folderPath = Globals.Subfolder ?? throw new ArgumentNullException("Subfolder cannot be null");

            foreach (var file in Directory.GetFiles(folderPath, "*" + Globals.GetExtendedFromConverionedFile))
            {
                existingFiles.Add(file);
            }

            extractedTextThread = new Thread(ExtractTextFromImages);
            extractedTextThread.Name = "ExtractedTextThread";
            extractedTextThread.Start(existingFiles);

            Console.WriteLine($"Watching for new PNG files in '{folderPath}'");

            using var watcher = new FileSystemWatcher(folderPath, "*" + Globals.GetExtendedFromConverionedFile);
            watcher.Created += (sender, e) =>
            {
                System.Console.WriteLine($"New file: {e.FullPath}");
                lock (existingFiles)
                {

                    if (existingFiles.Add(e.FullPath))
                    {
                        newFiles.Enqueue(e.FullPath);
                    }
                }
            };
            watcher.EnableRaisingEvents = true;        
            
            while (true)
            {
                Thread.Sleep(500);
            }
        }

        private static void ExtractTextFromImages(object filesObj)
        {
            OCReader oCReader = new();
            var files = (HashSet<string>)filesObj;
            foreach (var imagePath in files)
            {
                Console.WriteLine($"Performing OCR on {imagePath}...");
                string extractedText = oCReader.PerformOCR(imagePath);

                if (!string.IsNullOrWhiteSpace(extractedText))
                {
                    oCReader.SaveTextToFile(extractedText, imagePath);
                }
                else
                {
                    Console.WriteLine($"No text found in {imagePath}.");
                }
            }

            Console.WriteLine("No more images to process.");
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

            watchThread = new Thread(Watcher);
            watchThread.Name = "WatchThread";
            watchThread.Start();

            watchThread.Join();

        }
    }
}
