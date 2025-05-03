using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace LizardCode.Framework.Helpers.Utilities
{
    public static class Cryptography
    {
        //
        // http://stackoverflow.com/questions/273452/using-aes-encryption-in-c-sharp#26758901
        //

        private static readonly string cryptoKey;
        private static readonly string cryptoSalt;

        public enum CypherAction
        {
            Crypt,
            Decrypt
        }


        static Cryptography()
        {
            cryptoKey = "Cryptography:Key".FromAppSettings<string>(notFoundException: true);
            cryptoSalt = "Cryptography:Salt".FromAppSettings<string>(notFoundException: true);
        }


        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="saltValue">The salt value.</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <returns></returns>
        public static string Encrypt(string input, string saltValue = null, string passPhrase = null)
        {
            return Encrypt(input, saltValue ?? cryptoSalt, passPhrase ?? cryptoKey, "SHA1", 3, "JakdSup6N71Ay8gC", 256);
        }

        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="saltValue">The salt value.</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="passwordIterations">The password iterations.</param>
        /// <param name="initVector">The initialize vector.</param>
        /// <param name="keysize">The keysize.</param>
        /// <returns></returns>
        public static string Encrypt(string input, string saltValue, string passPhrase, string hashAlgorithm, int passwordIterations, string initVector, int keysize)
        {
            string functionReturnValue = null;
            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = null;
            initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = null;
            saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] pTextBytes = null;
            pTextBytes = Encoding.UTF8.GetBytes(input);
            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and
            // salt value. The password will be created using the specified hash
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = default;
            password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = null;
            keyBytes = password.GetBytes(keysize / 8);
            // Create uninitialized Rijndael encryption object.
            Aes symmetricKey = default;
            symmetricKey = Aes.Create();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;
            // Generate encryptor from the existing key bytes and initialization
            // vector. Key size will be defined based on the number of the key
            // bytes.
            ICryptoTransform encryptor = default;
            encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = default;
            memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = default;
            cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(pTextBytes, 0, pTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();
            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = null;
            cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = null;
            cipherText = Convert.ToBase64String(cipherTextBytes);

            functionReturnValue = cipherText;
            return functionReturnValue;
        }

        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static List<string> Encrypt(List<string> input)
        {
            if (input == null || input.Count == 0)
            {
                return null;
            }

            List<string> retList = new();

            input.ForEach(s => retList.Add(Encrypt(s)));

            return retList;
        }

        /// <summary>
        /// Encrypts the specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public static string Encrypt(string password, string userName)
        {
            return Encrypt(password, userName.ToLower(), cryptoKey, "SHA1", 3, "JakdSup6N71Ay8gC", 256);
        }

        /// <summary>
        /// Decrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="saltValue">The salt value.</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <returns></returns>
        public static string Decrypt(string input, string saltValue = null, string passPhrase = null)
        {
            return Decrypt(input, saltValue ?? cryptoSalt, passPhrase ?? cryptoKey, "SHA1", 3, "JakdSup6N71Ay8gC", 256);
        }

        /// <summary>
        /// Decrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="saltValue">The salt value.</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="passwordIterations">The password iterations.</param>
        /// <param name="initVector">The initialize vector.</param>
        /// <param name="keySize">Size of the key.</param>
        /// <returns></returns>
        public static string Decrypt(string input, string saltValue, string passPhrase, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            string functionReturnValue = null;

            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.


            byte[] initVectorBytes = null;
            initVectorBytes = Encoding.ASCII.GetBytes(initVector);

            byte[] saltValueBytes = null;
            saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = null;
            cipherTextBytes = Convert.FromBase64String(input);

            // First, we must create a password, from which the key will be
            // derived. This password will be generated from the specified
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = default;
            password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = null;
            keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            Aes symmetricKey = default;
            symmetricKey = Aes.Create();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization
            // vector. Key size will be defined based on the number of the key
            // bytes.
            ICryptoTransform decryptor = default;
            decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = default;
            memoryStream = new MemoryStream(cipherTextBytes);

            // Define memory stream which will be used to hold encrypted data.
            CryptoStream cryptoStream = default;
            cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = null;
            plainTextBytes = new byte[cipherTextBytes.Length + 1];

            // Start decrypting.
            int decryptedByteCount = 0;
            decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string.
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = null;
            plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            // Return decrypted string.
            functionReturnValue = plainText;


            return functionReturnValue;
        }

        /// <summary>
        /// Entities the cypher.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        public static T EntityCypher<T>(T entity, CypherAction action, params Expression<Func<T, object>>[] properties)
        {
            return (new List<T> { entity }).EntityCypher(action, properties).FirstOrDefault();
        }

        /// <summary>
        /// Entities the cypher.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">The entities.</param>
        /// <param name="action">The action.</param>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidExpressionException">
        /// </exception>
        public static List<T> EntityCypher<T>(this List<T> entities, CypherAction action, params Expression<Func<T, object>>[] properties)
        {
            if (entities == null || entities.Count == 0)
            {
                return new List<T>();
            }

            if (properties == null || properties.Length == 0)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            var type = typeof(T);

            foreach (var e in entities)
            {
                foreach (var p in properties)
                {
                    var me = (MemberExpression)p.Body;
                    if (me == null)
                    {
                        throw new InvalidExpressionException();
                    }

                    var pi = (PropertyInfo)me.Member;
                    if (pi == null)
                    {
                        throw new InvalidExpressionException();
                    }

                    var value = pi.GetValue(e, null);

                    if (value == null || !value.GetType().IsAssignableFrom(typeof(string)) || string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        continue;
                    }

                    switch (action)
                    {
                        case CypherAction.Crypt:
                            pi.SetValue(e, Encrypt(value.ToString()));
                            break;
                        case CypherAction.Decrypt:
                            pi.SetValue(e, Decrypt(value.ToString()));
                            break;
                    }

                }
            }

            return entities;
        }

        public static string ComputeMD5Hash(string input)
        {
            using var provider = MD5.Create();
            var result = provider.ComputeHash(Encoding.UTF8.GetBytes(input));

            return BitConverter.ToString(result).Replace("-", string.Empty);
        }

        public static byte[] GenerateSalt()
        {
            using var rnd = RandomNumberGenerator.Create();
            var saltBuffer = new byte[32];
            rnd.GetBytes(saltBuffer);

            return saltBuffer;
        }

        public static string HashPassword(string plain, byte[] saltBuffer)
        {
            //
            // https://immortalcoder.blogspot.com/2015/11/best-way-to-secure-password-using-cryptographic-algorithms-in-csharp-dotnet.html
            //

            if (string.IsNullOrWhiteSpace(plain))
            {
                throw new ArgumentNullException(nameof(plain));
            }

            if (saltBuffer == null || saltBuffer.Length == 0 || saltBuffer.All(a => a == 0))
            {
                throw new ArgumentException("Salto no definido", nameof(saltBuffer));
            }

            var passBuffer = Encoding.UTF8.GetBytes(plain.Trim());

            using var algorythm = new Rfc2898DeriveBytes(passBuffer, saltBuffer, 50000, HashAlgorithmName.SHA1);
            return Convert.ToBase64String(algorythm.GetBytes(64));
        }

        public static string EncodeId(int idToEncode, string salt, int minHashLength)
        {
            var h = new HashidsNet.Hashids(
                salt: salt,
                minHashLength: minHashLength
            );

            return h.Encode(idToEncode);
        }

        public static int DecodeId(string hash, string salt, int minHashLength)
        {
            var h = new HashidsNet.Hashids(
                salt: salt,
                minHashLength: minHashLength
            );

            return h.Decode(hash).FirstOrDefault();
        }

        public static string GetTempPassword()
        {
            var passwordCode = Guid.NewGuid().ToString();
            passwordCode = passwordCode.Substring(passwordCode.Length - 8, 8);

            return passwordCode;
        }
    }
}
