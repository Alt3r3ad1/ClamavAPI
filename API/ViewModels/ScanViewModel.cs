
namespace clamavAPI.ViewModels;

public class ScanViewModel
{
    public bool? infected { get; set; }
    public ICollection<string>? viruses { get; set; }
    public FileExtensionViewModel? fileExtension { get; set; }

}
