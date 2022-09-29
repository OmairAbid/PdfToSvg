using Shared;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Hello, World!");

            string ProjectDirPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string file = ProjectDirPath + "UltimateGuideCCSP-2018.pdf";

            //ConvertPDFtoSVG();
            //ConvertUsingGrapecity();
            //PdfUtility.ConvertUsingPdfToSvfNET(file);
            //PdfUtility.ConvertUsingAbcPdf(file);
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }
}