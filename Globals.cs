using System;

namespace Randomly;

public class Globals
{
    private static String _extended=".png";
    public static string? Subfolder {get;set;}
    public static string? PdfPath {get;set;}
    public static string GetExtendedFromConverionedFile 
    {
        get => _extended;
    }
}
