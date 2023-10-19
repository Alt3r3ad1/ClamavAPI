namespace clamavAPI.ViewModels;

public class FileExtensionViewModel
{
    public bool? compatibleFileExtension { get; set; }
    public string? expectedFileExtension { get; set; }
    public string? fileExtension { get; set; }
    public string? contentType { get; set; }
}