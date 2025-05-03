using System;
using System.IO;

namespace LizardCode.Framework.Helpers.Utilities.Mail
{
    public class Attachment
    {
        public string Name { get; private set; }

        public string Path { get; private set; }

        public byte[] BinaryContent { get; private set; }

        public MemoryStream Stream { get; private set; }


        public Attachment(string name, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var fullPath = Utilities.ResolvePath(path);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(path);
            }

            Path = fullPath;

            var delimitedName = name
                .Trim()
                .Replace(" ", ".");

            var cacheKey = string.Concat("Helpers.Mail.Attachment.", delimitedName);

            var binaryContent = cacheKey.FromCache(
                () => File.ReadAllBytes(fullPath),
                false
            );

            ConstructFromBinary(name, binaryContent);
        }

        public Attachment(string name, byte[] binaryContent)
        {
            ConstructFromBinary(name, binaryContent);
        }


        private void ConstructFromBinary(string name, byte[] binaryContent)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (binaryContent.Length == 0)
            {
                throw new ArgumentNullException(nameof(binaryContent));
            }

            Name = name;
            BinaryContent = binaryContent;
            Stream = new MemoryStream(BinaryContent);
        }
    }
}
