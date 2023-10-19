using clamavAPI.ViewModels;
using MimeDetective;

namespace clamavAPI.Services;

public class ExtensionAnalysisService
{
    private bool _compatibleFileExtension;
    private string _expectedFileExtension = string.Empty;
    private string _contentType = string.Empty;

    public bool compatibleFileExtension { get { return _compatibleFileExtension; } }
    public string expectedFileExtension { get { return _expectedFileExtension; } }
    public string contentType { get { return _contentType; } }



    public Task Verify(UploadViewModel uploadViewModel, MemoryStream memoryStream)
    {
        try
        {
            var AnalysisFile = new ContentInspectorBuilder()
            {
                Definitions = new MimeDetective.Definitions.ExhaustiveBuilder()
                {
                    UsageType = MimeDetective.Definitions.Licensing.UsageType.PersonalNonCommercial
                }.Build()
            }.Build();

            _expectedFileExtension = uploadViewModel.expectedFileExtension!.Replace(".", "").ToLower();

            var possibleContentType = AnalysisFile.Inspect(memoryStream.ToArray()).ByFileExtension();

            if (possibleContentType.Count() == 0)
            {
                _contentType = _expectedFileExtension;
            }
            else
            {
                _contentType = possibleContentType[0].Extension.ToString().ToLower();
            }

            if (_contentType.Equals(expectedFileExtension))
            {
                _compatibleFileExtension = true;
            }
            else
            {
                _compatibleFileExtension = false;
            }
        }
        catch (Exception)
        {
            _compatibleFileExtension = false;
        }

        return Task.CompletedTask;
    }
}