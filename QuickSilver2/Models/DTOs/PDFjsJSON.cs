namespace QuickSilver2.Models.DTOs;

public class PDFjsJSON {
    public Pages[] pages { get; set; }
}

public class Pages {
    public int page { get; set; }
    public string text { get; set; }
}