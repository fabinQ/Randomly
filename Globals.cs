using System;

namespace Randomly;

public class Globals
{
    private static String _extended=".png";
    private static string _subfolder;
    public static string Subfolder 
    {
        get => _subfolder;
        set
        {
            if (value == null) throw new ArgumentNullException("Subfolder cannot be null");
            _subfolder = value;
        }
    }
    public static string? PdfPath {get;set;}
    public static string GetExtendedFromConverionedFile 
    {
        get => _extended;
    }
}
