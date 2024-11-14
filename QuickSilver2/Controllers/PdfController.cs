using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Text;

namespace QuickSilver2.Controllers;

public class PdfController : Controller {
    public IActionResult Index() {
        return View();
    }

    public IActionResult UploadAspose() {
        return View();
    }

    public IActionResult pdfLib() {
        return View();
    }
    
    public IActionResult pdfJsToJSON() {
        return View();
    }
    
    // public static void ITextRead(string [] args)
    // {
    //     StringBuilder text = new StringBuilder();
    //     string fileName = @"D:/What_is_pdf.pdf";
    //     if (File.Exists(fileName)) {
    //         using PdfReader pdfReader = new PdfReader(fileName);
    //         using PdfDocument pdfDocument = new PdfDocument(pdfReader);
    //         for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
    //         {
    //             string currentText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page)); 
    //             currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
    //             text.Append(currentText);
    //         }
    //     }
    //     Console.WriteLine(text.ToString());
    // }

    public IActionResult ITextUploadToAPIToJSON() {
        return View();
    }

}