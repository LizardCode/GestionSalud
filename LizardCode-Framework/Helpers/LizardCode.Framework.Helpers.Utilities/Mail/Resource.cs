﻿using System;
using System.IO;

namespace LizardCode.Framework.Helpers.Utilities.Mail
{
    public class Resource
    {
        public string ContentId { get; private set; }

        public string Path { get; private set; }

        public string Base64Content { get; private set; }

        public byte[] BinaryContent { get; private set; }

        public MemoryStream Stream { get; private set; }


        public Resource(string contentId, string path, FileContentType type = FileContentType.Binary)
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

            var cacheKey = string.Concat("Helpers.Mail.LinkedResource.", type.ToString(), contentId);

            switch (type)
            {
                case FileContentType.Binary:
                    var binaryContent = cacheKey.FromCache(
                        () => File.ReadAllBytes(fullPath),
                        false
                    );

                    ConstructFromBinary(contentId, binaryContent);
                    break;

                case FileContentType.Base64:
                    var base64Content = cacheKey.FromCache(
                        () => File.ReadAllText(fullPath),
                        false
                    );

                    ConstructFromBase64(contentId, base64Content);
                    break;
            }
        }

        public Resource(string contentId, string base64Content)
        {
            ConstructFromBase64(contentId, base64Content);
        }

        public Resource(string contentId, byte[] binaryContent)
        {
            ConstructFromBinary(contentId, binaryContent);
        }


        private void ConstructFromBinary(string contentId, byte[] binaryContent)
        {
            if (string.IsNullOrWhiteSpace(contentId))
            {
                throw new ArgumentNullException(nameof(contentId));
            }

            if (BinaryContent.Length == 0)
            {
                throw new ArgumentNullException(nameof(binaryContent));
            }

            ContentId = contentId;
            BinaryContent = binaryContent;
            Stream = new MemoryStream(BinaryContent);
        }

        private void ConstructFromBase64(string contentId, string base64Content)
        {
            if (string.IsNullOrWhiteSpace(contentId))
            {
                throw new ArgumentNullException(nameof(contentId));
            }

            if (string.IsNullOrWhiteSpace(base64Content))
            {
                throw new ArgumentNullException(nameof(base64Content));
            }

            ContentId = contentId;
            Base64Content = base64Content;

            if (Utilities.IsBase64String(base64Content))
            {
                Stream = new MemoryStream(Convert.FromBase64String(base64Content));
            }
            else
            {
                throw new Base64FormatException();
            }
        }
    }
}
