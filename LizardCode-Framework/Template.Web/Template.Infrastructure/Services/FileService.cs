using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Template.Application.Interfaces.Services;

namespace Template.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _config;


        public FileService(IConfiguration config)
        {
            _config = config;
        }


        public async Task<string> PersistAsync(string folderConfigKey, string filename, Stream stream)
        {
            var buffer = await StreamToBuffer(stream);

            return await PersistAsync(folderConfigKey, filename, buffer);
        }

        public async Task<string> PersistAsync(string folderConfigKey, string filename, byte[] buffer)
        {
            var folder = _config.GetSection(folderConfigKey).Value;
            var path = Path.Combine(folder, filename);

            await PersistAsync(path, buffer);

            return path;
        }

        public async Task PersistAsync(string path, Stream stream)
        {
            var buffer = await StreamToBuffer(stream);

            await PersistAsync(path, buffer);
        }

        public async Task PersistAsync(string path, byte[] buffer)
        {
            var folder = Path.GetDirectoryName(path);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using var fs = File.Create(path);
            await fs.WriteAsync(buffer);
        }


        private async Task<byte[]> StreamToBuffer(Stream stream)
        {
            if (!stream.CanRead)
            {
                return null;
            }

            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var buffer = ms.ToArray();

            return buffer;
        }
    }
}
