using System.IO;
using System.Threading.Tasks;

namespace Template.Application.Interfaces.Services
{
    public interface IFileService
    {
        Task PersistAsync(string path, byte[] buffer);
        Task PersistAsync(string path, Stream stream);
        Task<string> PersistAsync(string folderConfigKey, string filename, byte[] buffer);
        Task<string> PersistAsync(string folderConfigKey, string filename, Stream stream);
    }
}