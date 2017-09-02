using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Decrypt.Config.ConfigurationProviders;
using Decrypt.ConfigTests;
using Encryption.Hybrid;
using Encryption.Hybrid.Asymmetric;
using Encryption.Hybrid.Hybrid;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Decrypt.ConfigTests {
    public class ConfigurationBuilderTests
    {
        [Test]
        public void GivenEncryptedFile_WhenDecryptingFile_ThenDecryptsFile()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            const string secretsettingsJson = "secretsettings.json";
            const string encryptedSettingsFile = "encryptedsettings";
            const string signatureContainer = "SignatureContainer";
            const string encryptionContainer = "ConfigurationContainer";

            var expected = JObject.Parse(File.ReadAllText(secretsettingsJson));

            var keys = ExportEncryptionVerificationKeys(encryptionContainer, signatureContainer);

            CreateEncryptedFile(signatureContainer, secretsettingsJson, encryptedSettingsFile, keys.encryptionKey);

            ConfigurationBuilder builder = new ConfigurationBuilder();

            builder.AddEncryptedFile(encryptedSettingsFile, encryptionContainer, signatureContainer);

            var root = builder.Build();

            Assert.That(root["Message"], Is.EqualTo(expected["Message"].Value<string>()));
        }

        private void CreateEncryptedFile(string signatureContainer, string secretsettingsJson, string encryptedSettingsFile, string encryptionKey)
        {
            var encryptedFile = EncryptFile(signatureContainer, secretsettingsJson, encryptionKey);

            var blob = encryptedFile.key.ExportToBlob();

            var encryptedFileInBase64 = $"{Convert.ToBase64String(blob)}.{Convert.ToBase64String(encryptedFile.encryptedData)}";

            File.WriteAllBytes(encryptedSettingsFile, Encoding.UTF8.GetBytes(encryptedFileInBase64));
        }

        private (SessionKeyContainer key, byte[] encryptedData) EncryptFile(string signatureContainer, string secretsettingsJson, string encryptionKey)
        {
            HybridEncryption encryption = HybridEncryption.Create(encryptionKey, signatureContainer);

            var symmetricKey = CreateSymmetricKey();

            (SessionKeyContainer key, byte[] encryptedData) kvp = encryption.EncryptData(symmetricKey.SessionKey,
                                                                                         File.ReadAllBytes(secretsettingsJson),
                                                                                         symmetricKey.Iv);
            return kvp;
        }

        private static (byte[] SessionKey, byte[] Iv) CreateSymmetricKey()
        {
            Random random = new Random();
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.NextBytes(sessionKey);
            random.NextBytes(iv);

            return (sessionKey, iv);
        }

        private string ExportKey(string containerName)
        {
            var rsa = RSAEncryption.LoadContainer(containerName);
            return rsa.ExportKey(false);
        }

        private (string encryptionKey, string signatureKey) ExportEncryptionVerificationKeys(string encryptionContainer, string signatureContainer)
        {
            return (ExportKey(encryptionContainer), ExportKey(signatureContainer));
        }
    }
}