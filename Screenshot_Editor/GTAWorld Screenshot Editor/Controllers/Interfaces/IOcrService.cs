using System.Threading.Tasks;

namespace GTAWorld_Screenshot_Editor.Controllers.Interfaces
{
    public interface IOcrService
    {
        Task<string> ExtractText(string image);
        Task<string> ExtractText(string image, string languageCode);
    }
}
