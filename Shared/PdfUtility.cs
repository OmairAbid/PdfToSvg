
using PdfToSvg;
using WebSupergoo.ABCpdf12;
using WebSupergoo.ABCpdf12.Objects;

namespace Shared
{
    using GrapeCity.Documents.Pdf;
    using GrapeCity.Documents.Svg;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Xml;
    

    public static class PdfUtility
    {
        public static void ConvertPDFtoSVG(string path)
        {

           
            if (File.Exists(path))
            {
                // load PDF with an instance of Document                        
                //var document = new PdfDocument(file);

                ////Load a sample PDF file
                //document.LoadFromFile(file);


                //// save document in PPTX format
                //document.SaveToFile(ProjectDirPath +"output.svg",5,5, FileFormat.SVG);
            }
            else
            {
                throw new FileNotFoundException();
            }

        }

        public static void ConvertUsingGrapecity(string path)
        {
            

            if (File.Exists(path))
            {
                var pdf = new GcPdfDocument();
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    pdf.Load(fs);
                    var pdfPage = pdf.Pages[0];
                    var view = new GrapeCity.Documents.Pdf.Layers.ViewState(pdf);
                    view.SetLayersUIState(true, "lighting_plan.pdf");
                    view.SetLayersUIStateExcept(false, "lighting_plan.pdf");

                    pdfPage.SaveAsSvg("pdfToSvg_view.svg", view, null, null);
                }

            }
        }

        public static void ConvertUsingPdfToSvfNET(string path)
        {

            string ProjectDirPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));

            using (var doc = PdfDocument.Open(path))
            {
                var pageNo = 1;
                string pathToSave = ProjectDirPath + $"{path.Split('\\')[path.Split('\\').Length -1]}Convert\\";
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);

                foreach (var page in doc.Pages)
                {
                    page.SaveAsSvg(pathToSave + $"output-{pageNo++}.svg");
                }
            }
        }

        public static async Task<string> ConvertUsingAbcPdf(string directory, string folderPath, string path, string filename, CancellationToken token = default)
        {

            Directory.SetCurrentDirectory(directory);
            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderPath, filename + $"UsingConvertABC{Path.DirectorySeparatorChar}");
            string pathToSaveImages = Path.Combine(pathToSave , $"Images{ Path.DirectorySeparatorChar }" );
            string image = string.Empty;
            int pageNumber = 0;
            string pageOne = string.Empty;
            List<string> images = new List<string>();


            using (Doc doc = new Doc())
            {
                doc.Read(path);
                for (var currentPageNumber = 1; currentPageNumber <= doc.PageCount; currentPageNumber++)
                {
                    token.ThrowIfCancellationRequested();
                    images.Clear();
                    pageNumber = doc.PageCount;
                    doc.PageNumber = currentPageNumber;
                    ((WebSupergoo.ABCpdf12.Objects.Page)doc.ObjectSoup[doc.Page]).VectorizeText();

                    if (!Directory.Exists(pathToSave))
                        Directory.CreateDirectory(pathToSave);

                    doc.Rendering.Save(pathToSave + $"{currentPageNumber}.svg");
                    string svg = File.ReadAllText(pathToSave + $"{currentPageNumber}.svg");
                    HashSet<string> hrefs = new HashSet<string>();
                    string pattern = "<image xlink:href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
                    MatchCollection matches = Regex.Matches(svg, pattern);
                    if (!Directory.Exists(pathToSaveImages))
                        Directory.CreateDirectory(pathToSaveImages);
                    foreach (Match match in matches)
                        hrefs.Add(match.Groups[1].Value);
                    string? folder = Path.GetDirectoryName(pathToSaveImages);
                    foreach (string href in hrefs)
                    {
                        image = Path.Combine(folder, href);

                        //if (!File.Exists(image))
                        //{
                            int id = int.Parse(href.Substring(5, href.Length - 9));
                            PixMap pm = doc.ObjectSoup[id] as PixMap;
                            using (Bitmap bm = pm.GetBitmap())
                                bm.Save(image);

                            images.Add(image);
                        //}
                    }

                    XmlDocument docLoad = new XmlDocument();
                    docLoad.Load(pathToSave + $"{currentPageNumber}.svg");


                    int counter = 0;
                    if(currentPageNumber == doc.PageCount)
                    {

                    }
                    
                    XmlNodeList elemList = docLoad.GetElementsByTagName("image");
                    foreach (XmlElement elem in elemList)
                    {
                        if (elem.HasAttribute("xlink:href")) 
                        {
                            elem.SetAttribute("xlink:href", images.ElementAt(counter));
                        }
                        counter++;
                        
                    }
                    if (currentPageNumber == 1)
                    {
                        pageOne = docLoad.OuterXml;
                    }
                    docLoad.Save(pathToSave + $"{currentPageNumber}.svg");
                }

            }

            return pageOne;
        }
            

    }
}
