namespace QuickSilver2.Models.DTOs;

public class PDFlibJSON {
    public LibMetadata metadata { get; set; }
}

public class LibMetadata {
    public string title { get; set; }
    public string author { get; set; }
    public string subject { get; set; }
    public string keywords { get; set; }
}