namespace clamavAPI.ViewModels;

public class UploadViewModel
{
    public IFormFile? file {get;set;}
    public string expectedFileExtension {get; set;} = string.Empty;
}
