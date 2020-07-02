using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.IO;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Pdfdownloader
{
    class Program
    {
       


        static void Main(string[] args)
        {
            WebClient client = new WebClient();

            List<String> linkS = new List<String>();
            //List<String> linkS1 = new List<String>();
            List<String> linkS2 = new List<String>();

            ICollection<IPdfTextLocation> textLocations = new List<IPdfTextLocation>();

            PdfReader pdfReader = new PdfReader(@"C:\Users\ocan4214\Desktop\SpringerEbooks.pdf");
            PdfDocument pdfdoc = new PdfDocument(pdfReader);

            Console.WriteLine("page number = " + pdfdoc.GetNumberOfPages());

            RegexBasedLocationExtractionStrategy regexStrat = new RegexBasedLocationExtractionStrategy(@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");

            //PdfCanvasProcessor canvasProcessor = new PdfCanvasProcessor(regexStrat);

            LocationTextExtractionStrategy strat = new LocationTextExtractionStrategy();

            PdfCanvasProcessor canvasProcessor = new PdfCanvasProcessor(strat);

            StringBuilder text = new StringBuilder();





            for (int i = 1; i <= pdfdoc.GetNumberOfPages(); i++)
            {

                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                string currentText = PdfTextExtractor.GetTextFromPage(pdfdoc.GetPage(i), strategy);

                currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));

                text.Append(currentText);

                /*
                canvasProcessor.ProcessPageContent(pdfdoc.GetPage(i));
                canvasProcessor.Reset();
                foreach (var m in regexStrat.GetResultantLocations())
                {
                   textLocations.Add(m);
                }
                */

            }

            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (Match m in linkParser.Matches(text.ToString()))
            {
                linkS.Add(m.ToString());
            }

            /* foreach(var l in linkS)
             {
                 var st = l.Replace(@"http://link.springer.com/openurl?genre=book&isbn=978-", "https://link.springer.com/content/pdf/10.1007%2F978-");
                 st += ".pdf";
                 linkS1.Add(st);
             }*/
            int j = 0;
            foreach (var link in linkS)
            {


                HtmlWeb web = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc = web.Load(link);
                var node = doc.DocumentNode.SelectSingleNode("//a[@title='Download this book in PDF format']");
                var titlenode = doc.DocumentNode.SelectSingleNode("//h1");
                if (node != null && titlenode != null)
                {
                    HtmlAttribute href = node.Attributes["href"];
                    if (href != null)
                    {
                        Console.WriteLine("https://link.springer.com/" + href.Value + " title = " + titlenode.InnerText);

                        var titlesafe = titlenode.InnerText;

                        if (titlesafe.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
                            titlesafe = "Change the Name of this " + j;
                          
                            

                        linkS2.Add(@"https://link.springer.com/" + href.Value);
                        client.DownloadFile(new Uri("https://link.springer.com/" + href.Value), System.IO.Path.Combine(@"C:\Users\ocan4214\Desktop\pdfler\TEST", titlesafe + ".pdf"));



                        j++;

                    }
                }


            }

            /*        
           foreach (var m in textLocations)
            {
                Console.WriteLine("Link is "+ m.GetText());
            }
            */


            // https://link.springer.com/book/10.1007%2F978-3-319-18398-5 Phyton for ARCGIS   Book link
            // https://link.springer.com/content/pdf/10.1007%2F978-3-319-18398-5.pdf PDF link
            // http://link.springer.com/openurl?genre=book&isbn=978-3-319-18398-5 our link

            // https://link.springer.com/book/10.1007%2F978-1-4899-7550-8  The finite element method  Book link
            // https://link.springer.com/content/pdf/10.1007%2F978-1-4899-7550-8.pdf PDF link
            // http://link.springer.com/openurl?genre=book&isbn=978-1-4899-7550-8

            var a = 5;
        }



    }
}



