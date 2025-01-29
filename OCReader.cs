using Tesseract;


namespace Randomly;

public class OCReader
{
    public string PerformOCR(string imagePath)
    {
        try
        {
            using (var ocrEngine = new TesseractEngine(@"./../../../tessdata", language: "pol", EngineMode.LstmOnly, @"C:\Users\Hyperbook\Documents\Maciej\Randomly\tessdata\numbers_configfile.txt"))
            using (Pix img = Pix.LoadFromFile(imagePath))
            {
                // Increase contrast
                using (Pix contrastImg = img.ConvertRGBToGray())
                using (Page page = ocrEngine.Process(contrastImg))
                {
                    var confidence = page.GetMeanConfidence();
                    System.Console.WriteLine(confidence);
                    return page.GetText();
                }
                
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
        string subfolder = Globals.Subfolder ?? throw new ArgumentNullException("Subfolder cannot be null");
        string outputTextPath = Path.Combine(subfolder, Path.GetFileNameWithoutExtension(Globals.PdfPath) + Path.GetFileNameWithoutExtension(imagePath)+"_OCR.txt") ;
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
