using Microsoft.AspNetCore.Mvc;
using QuickSilver2.Models;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace QuickSilver2.Areas.API;

[Route("api/[controller]")]
[ApiController]
public class ApiPdfController : ControllerBase {
    /// <summary>
    /// Using iText
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost("upload")]
    public IActionResult UploadPdf(IFormFile? file) {
        if (file == null || file.Length == 0) {
            return BadRequest(new { error = "No file uploaded." });
        }

        // Extract and convert
        var extractedText = ExtractTextFromPdf(file);

        // Wrap extracted text in a JSON structure to return as JSON
        // todo Sarah this can change
        var json = JsonConvert.SerializeObject(new { text = extractedText.ToString() }, Formatting.Indented);

        return Ok(new { text = extractedText });
    }

    private string ExtractTextFromPdf(IFormFile pdfFile) {
        var extractedText = new StringBuilder();

        using (var pdfReader = new PdfReader(pdfFile.OpenReadStream()))
        using (var pdfDocument = new PdfDocument(pdfReader)) {
            int numberOfPages = pdfDocument.GetNumberOfPages();
            for (int i = 1; i <= numberOfPages; i++) {
                var pageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i));
                var pageTextSTES =
                    PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i), new SimpleTextExtractionStrategy());
                //todo sarah so pageText is an entire page at this point?
                //loop through at this point and define sections
                extractedText.Append(pageText);
            }

            // Extract metadata
            var info = pdfDocument.GetDocumentInfo();
            Console.WriteLine($"SARAH PDF DOC INFO: {info}");
            var keyWords = info.GetKeywords();
            if (keyWords is not null) {
                var keyWordsList = keyWords?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var keyword in keyWordsList) {
                    var deets = info.GetMoreInfo(keyword);
                    Console.WriteLine($"Details for Keyword: {keyword.Trim()} -> {deets}");
                }
            }

            Console.WriteLine("Title: " + info.GetTitle());
            Console.WriteLine("Author: " + info.GetAuthor());
            Console.WriteLine("Subject: " + info.GetSubject());
            Console.WriteLine("Keywords: " + keyWords);
            Console.WriteLine($"Trapped: {info.GetTrapped()}");
        }

        return extractedText.ToString();
    }

    public static void ExtractTextAndMetadata(string pdfPath) {
        using (var pdfReader = new PdfReader(pdfPath))
        using (var pdfDocument = new PdfDocument(pdfReader)) {
            // Extract text content
            StringBuilder extractedText = new StringBuilder();
            int numberOfPages = pdfDocument.GetNumberOfPages();
            for (int i = 1; i <= numberOfPages; i++) {
                var pageText =
                    PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i), new SimpleTextExtractionStrategy());
                extractedText.Append(pageText);
            }

            Console.WriteLine("Extracted Text:");
            Console.WriteLine(extractedText.ToString());

            // Extract metadata
            var info = pdfDocument.GetDocumentInfo();
            Console.WriteLine("Title: " + info.GetTitle());
            Console.WriteLine("Author: " + info.GetAuthor());
            Console.WriteLine("Subject: " + info.GetSubject());
            Console.WriteLine("Keywords: " + info.GetKeywords());
        }
    }
}