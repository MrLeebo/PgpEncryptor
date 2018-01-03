using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.IO;
using System.Linq;

namespace PgpEncryptor
{
    public static class Encryptor
    {
        private const int BUFFER_SIZE = 32768; // 32kb

        public static void EncryptFile(string inputFile, string outputFile, string publicKey)
        {
            using (var input = File.OpenRead(inputFile))
            using (var file = File.Create(outputFile))
            using (var encrypted = EncryptOut(file, publicKey))
            using (var compressed = CompressOut(encrypted))
            using (var literal = LiteralOut(compressed, inputFile))
                input.CopyTo(literal, BUFFER_SIZE);
        }

        private static Stream EncryptOut(Stream stream, string publicKey)
        {
            var key = GetEncryptionKey(publicKey);
            var encryptor = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, true);
            encryptor.AddMethod(key);

            return encryptor.Open(stream, new byte[BUFFER_SIZE]);
        }

        private static Stream CompressOut(Stream stream)
        {
            var compressor = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
            return compressor.Open(stream, new byte[BUFFER_SIZE]);
        }

        private static Stream LiteralOut(Stream stream, string inputFile)
        {
            var literal = new PgpLiteralDataGenerator();
            return literal.Open(stream, PgpLiteralData.Binary, inputFile, DateTime.Now, new byte[BUFFER_SIZE]);
        }

        private static PgpPublicKey GetEncryptionKey(string keyFile)
        {
            using (var inputStream = File.OpenRead(keyFile))
            using (var decodedStream = PgpUtilities.GetDecoderStream(inputStream))
            {
                return new PgpPublicKeyRingBundle(decodedStream)
                    .GetKeyRings().OfType<PgpPublicKeyRing>()
                    .SelectMany(x => x.GetPublicKeys().OfType<PgpPublicKey>())
                    .First(x => x.IsEncryptionKey);
            }
        }
    }
}
