using LizardCode.Framework.Helpers.Utilities;

namespace LizardCode.Framework.Application.Helpers
{
    public static class EncodingIdsHelper
    {
        public static string EncodeId(int id)
        {
            var salt = "EncodingIds:Salt".FromAppSettings<string>(notFoundException: true);
            var minHashLength = "EncodingIds:MinLength".FromAppSettings(6);

            return Cryptography.EncodeId(id, salt, minHashLength);
        }

        public static int DecodeId(string hash)
        {
            var salt = "EncodingIds:Salt".FromAppSettings<string>(notFoundException: true);
            var minHashLength = "EncodingIds:MinLength".FromAppSettings(6);

            return Cryptography.DecodeId(hash, salt, minHashLength);
        }
    }
}
