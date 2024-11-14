namespace QuickSilver2.Models.DTOs;

public class iTextJSON {
    public iTextMetadata Metadata { get; set; }
    public string[] Content { get; set; }//pages
}

public class iTextMetadata {
    //todo sarah JSON properties needed?
    public string _Author { get; set; }
    public string _CreationDate { get; set; }
    public string _Creator { get; set; }
    public string _ElsevierWebPDFSpecifications { get; set; }
    public string _ModDate { get; set; }
    public string _Producer { get; set; }
    public string _Subject { get; set; }
    public string _Keywords { get; set; }
    public string _Title { get; set; }
    public string _doi { get; set; }
    public string _grabs { get; set; }
    public string _robots { get; set; }
}