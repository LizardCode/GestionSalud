using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LizardCode.Framework.Helpers.Utilities
{
    //
    // https://www.garykessler.net/library/file_sigs.html <-- Magic numbers
    // https://github.com/neilharvey/FileSignatures <-- Mejor usar este NuGet
    // https://stackoverflow.com/a/4212908/1812392 <-- Office Mime Types
    // https://developer.mozilla.org/es/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types <-- Resto de Mime Types
    //
    public class FileSignature
    {
        private string _hexSignature;


        public FileSignature(byte[] binary)
        {
            ExtractSignature(binary);
        }

        public FileSignature(Stream stream)
        {
            ExtractSignature(stream);
        }


        private void ExtractSignature(byte[] binary)
        {
            if (binary.Length < 20)
                throw new ArgumentException(nameof(binary));

            var signature = new byte[20];

            Array.Copy(binary, signature, 20);
            _hexSignature = signature.ToHexString().ToUpper();
        }

        private void ExtractSignature(Stream stream)
        {
            if (!stream.CanRead)
                throw new ArgumentException(nameof(stream));

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                if (ms.Length < 20)
                    throw new ArgumentException(nameof(stream));

                var signature = new byte[20];

                ms.Seek(0, SeekOrigin.Begin);
                ms.Position = 0;
                ms.Read(signature, 0, signature.Length);

                _hexSignature = signature.ToHexString().ToUpper();
            }
        }


        public FileFormat Parse()
        {
            if (IsJpg())
                return new("jpg", "image/jpeg");

            if (IsGif())
                return new("gif", "image/gif");

            if (IsPng())
                return new("png", "image/png");

            if (IsPdf())
                return new("pdf", "application/pdf");

            if (IsDocx())
                return new("docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            if (IsXlsx())
                return new("xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            if (IsPptx())
                return new("pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");

            if (IsZip())
                return new("zip", "application/zip");

            if (IsRar())
                return new("rar", "application/x-rar-compressed");

            if (IsRarV5())
                return new("rar", "application/x-rar-compressed");

            return null;
        }


        public bool IsJpg()
        {
            if (_hexSignature.StartsWith("FFD8FFDB")) // RAW JPEG
                return true;
            else if (_hexSignature.StartsWith("FFD8FFEE")) // RAW JPEG
                return true;
            else if (_hexSignature.StartsWith("FFD8FFE0")) // JFIF
                return true;
            else if (_hexSignature.StartsWith("FFD8FFE1")) // EXIF
                return true;
            else if (_hexSignature.StartsWith("FFD8FFE2")) // CANNON EOS
                return true;
            else if (_hexSignature.StartsWith("FFD8FFE3")) // SAMSUNG D500
                return true;

            return false;
        }

        public bool IsGif()
        {
            if (_hexSignature.StartsWith("474946383761")) // GIF87a
                return true;
            else if (_hexSignature.StartsWith("474946383961")) // GIF89a
                return true;

            return false;
        }

        public bool IsPng()
            => _hexSignature.StartsWith("89504E470D0A1A0A");

        public bool IsPdf()
            => _hexSignature.StartsWith("255044462D");

        public bool IsDocx()
            => _hexSignature.StartsWith("504B030414000600080000002100DFA4D26C5A01");

        public bool IsXlsx()
            => _hexSignature.StartsWith("504B03041400060008000000210062EE9D685E01");

        public bool IsPptx()
            => _hexSignature.StartsWith("504B030414000600080000002100DFCC18F5AD01");

        public bool IsZip()
            => _hexSignature.StartsWith("504B0304");

        public bool IsRar()
            => _hexSignature.StartsWith("526172211A0700");

        public bool IsRarV5()
            => _hexSignature.StartsWith("526172211A070100");


        public string ComputeMD5Hash(byte[] binary)
        {
            using (var md5 = MD5.Create())
            {
                using (var cs = new CryptoStream(Stream.Null, md5, CryptoStreamMode.Write))
                {
                    using (var ms = new MemoryStream(binary))
                    {
                        ms.CopyTo(cs);
                    }

                    cs.FlushFinalBlock();
                }

                var hex = new StringBuilder(md5.Hash.Length * 2);

                for (int i = 0; i < md5.Hash.Length; i++)
                    hex.Append(md5.Hash[i].ToString("x2"));

                return hex.ToString();
            }
        }
    }

    public class FileFormat
    {
        public string Extension { get; }
        public string MimeType { get; }


        public FileFormat(string extension, string mimeType)
        {
            Extension = extension;
            MimeType = mimeType;
        }
    }
}
