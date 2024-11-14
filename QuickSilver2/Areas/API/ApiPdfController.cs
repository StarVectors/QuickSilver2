using Microsoft.AspNetCore.Mvc;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Newtonsoft.Json;
using QuickSilver2.Models.DTOs;

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
    public async Task<IActionResult> UploadPdf(IFormFile? file) {
        if (file == null || file.Length == 0) {
            return BadRequest(new { error = "No file uploaded." });
        }

        // Extract and convert
        // gets json string
        var v2 = ConvertPdfToJson(file);
        // gets iTextJSON object
        var v3 = await ConvertPdfToiTextJSONClass(file);
        // gets extracted text
        var extractedText = ExtractTextFromPdf(file);
        
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

            var docPdfDict = pdfDocument.GetTrailer();
            var docInfoDic = docPdfDict.GetAsDictionary(PdfName.Info);
            var keywords = docInfoDic.GetAsString(PdfName.Keywords);
            var docCat = pdfDocument.GetCatalog();

            var keys = docInfoDic.KeySet();
            // Extract metadata
            var info = pdfDocument.GetDocumentInfo();
            foreach (var key in keys) {
                var value = docInfoDic.GetAsString(key);
                var val = info.GetMoreInfo(key.GetValue());
                Console.WriteLine($"Key {key} -> {value}");
            }

            var keyWords = info.GetKeywords();
            if (keyWords is not null) {
                Console.WriteLine("Keywords: " + keyWords);
                var keyWordsList = keyWords?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var keyword in keyWordsList) {
                    var deets = info.GetMoreInfo(keyword);
                    Console.WriteLine($"Details for Keyword: {keyword.Trim()} -> {deets}");
                }
            }

            var title = info.GetTitle();
            var author = info.GetAuthor();
            var subject = info.GetSubject();
            var trapped = info.GetTrapped();
            Console.WriteLine("Title: " + title);
            Console.WriteLine("Author: " + author);
            Console.WriteLine("Subject: " + subject);
            Console.WriteLine($"Trapped: {trapped}");
        }

        return extractedText.ToString();
    }

    public string ConvertPdfToJson(IFormFile pdfFile) {
        var pdfData = new Dictionary<string, object>();

        using (PdfReader reader = new PdfReader(pdfFile.OpenReadStream()))
        using (PdfDocument pdfDoc = new PdfDocument(reader)) {
            // Extract metadata
            var metadata = new Dictionary<string, string>();
            PdfDictionary infoDictionary = pdfDoc.GetTrailer().GetAsDictionary(PdfName.Info);

            if (infoDictionary != null) {
                foreach (PdfName key in infoDictionary.KeySet()) {
                    PdfObject value = infoDictionary.Get(key);
                    metadata[key.ToString()] = value?.ToString();
                }
            }

            pdfData["Metadata"] = metadata;

            // Extract text content
            var pages = new List<string>();
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++) {
                string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
                pages.Add(pageText);
            }

            pdfData["Content"] = pages;
        }

        // Convert dictionary to JSON
        return JsonConvert.SerializeObject(pdfData, Formatting.Indented);
    }

    public async Task<iTextJSON> ConvertPdfToiTextJSONClass(IFormFile pdfFile) {
        var iTextJson = new iTextJSON();
        var metadata = new iTextMetadata();
        var pages = new List<string>();

        using (PdfReader reader = new PdfReader(pdfFile.OpenReadStream()))
        using (PdfDocument pdfDoc = new PdfDocument(reader)) {
            // Extract metadata
            PdfDictionary infoDictionary = pdfDoc.GetTrailer().GetAsDictionary(PdfName.Info);

            if (infoDictionary != null) {
                metadata._Author = infoDictionary.GetAsString(PdfName.Author)?.ToString();
                metadata._CreationDate = infoDictionary.GetAsString(PdfName.CreationDate)?.ToString();
                metadata._Creator = infoDictionary.GetAsString(PdfName.Creator)?.ToString();
                metadata._ElsevierWebPDFSpecifications =
                    infoDictionary.GetAsString(new PdfName("ElsevierWebPDFSpecifications"))?.ToString();
                metadata._ModDate = infoDictionary.GetAsString(PdfName.ModDate)?.ToString();
                metadata._Producer = infoDictionary.GetAsString(PdfName.Producer)?.ToString();
                metadata._Subject = infoDictionary.GetAsString(PdfName.Subject)?.ToString();
                metadata._Keywords = infoDictionary.GetAsString(PdfName.Keywords)?.ToString();
                metadata._Title = infoDictionary.GetAsString(PdfName.Title)?.ToString();
                metadata._doi = infoDictionary.GetAsString(new PdfName("doi"))?.ToString();
                metadata._grabs = infoDictionary.GetAsString(new PdfName("grabs"))?.ToString();
                metadata._robots = infoDictionary.GetAsString(new PdfName("robots"))?.ToString();
            }

            // Extract text content
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++) {
                string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
                pages.Add(pageText);
            }
        }

        iTextJson.Metadata = metadata;
        iTextJson.Content = pages.ToArray();

        return iTextJson;
    }
}