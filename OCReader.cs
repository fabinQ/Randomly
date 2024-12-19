using System;
using iText.Kernel.Pdf.Annot.DA;
using iText.Layout.Element;
using Tesseract;

namespace Randomly;

public class OCReader
{
    public string PerformOCR(string imagePath)
    {
        try
        {
            using(var ocrEngine = new TesseractEngine(@"./../../../tessdata",language: "pol", EngineMode.Default))
                using(Pix img = Pix.LoadFromFile(imagePath))
                    using (Page page = ocrEngine.Process(img))
                    {
                        return page.GetText();
                    }
        }
        catch (Exception exception)
        {
            System.Console.WriteLine($"Error during OCR process: {exception.Message}");
            return string.Empty;
        }
    }
    public void SaveTextToFile(string exctractedText, string imagePath)
    {
        string outputTextPath = Path.Combine(Globals.Subfolder,Path.GetFileNameWithoutExtension(Globals.PdfPath) + Path.GetFileNameWithoutExtension(imagePath)+"_OCR.txt") ;
        try
        {
            File.WriteAllText(outputTextPath, exctractedText);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error saving text to file {imagePath}: {ex.Message}");
        }
        finally
        {
            System.Console.WriteLine($"Text from {Path.GetFileName(imagePath)} has been saved.");
        }
    }
}
