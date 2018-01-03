using Microsoft.VisualStudio.TestTools.UnitTesting;
using PgpEncryptor;
using System.IO;

namespace Tests
{
    [TestClass]
    public class EncryptorTests
    {
        [TestMethod]
        public void Should_Encrypt()
        {
            var input = "Fixtures/Message.txt";
            var output = $"{input}.pgp";
            var key = "Fixtures/Public.asc";

            try
            {
                Encryptor.EncryptFile(input, output, key);

                Assert.IsTrue(File.Exists(output), "Expected output file to exist, but it didn't.");
            }
            finally
            {
                File.Delete(output);
            }
        }
    }
}
