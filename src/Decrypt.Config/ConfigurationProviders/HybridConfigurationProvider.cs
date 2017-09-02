using System;
using System.IO;
using System.Text;
using Encryption.Hybrid;
using Encryption.Hybrid.Asymmetric;
using Encryption.Hybrid.Hybrid;
using Encryption.Hybrid.Symmetric;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;

namespace Decrypt.Config.ConfigurationProviders {
    public class HybridConfigurationProvider : FileConfigurationProvider
    {
        private readonly string _containerName;
        private readonly string _signatureKey;

        public override void Load()
        {
            IFileInfo fileInfo = Source.FileProvider.GetFileInfo(Source.Path);

            if (fileInfo.Exists)
            {
                using (Stream stream = fileInfo.CreateReadStream())
                {
                    Load(stream);
                }
            }
        }

        public override void Load(Stream stream)
        {
            byte[] encryptedData;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                encryptedData = memoryStream.ToArray();
            }

            var base64Array = Encoding.UTF8.GetString(encryptedData).Split('.');

            var sessionKeyBlob = Convert.FromBase64String(base64Array[0]);

            var encryptedConfigData = Convert.FromBase64String(base64Array[1]);

            var sessionKey = SessionKeyContainer.FromBlob(sessionKeyBlob);

            var hybridDecryption = HybridDecryption.Create(_containerName, _signatureKey);

            var data = hybridDecryption.DecryptData(sessionKey, encryptedConfigData);

            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                var configurationFileParser = new JsonConfigurationFileParser();
                this.Data = configurationFileParser.Parse(memoryStream);
            }
        }

        public HybridConfigurationProvider(HybridConfigurationSource source, string containerName, string signatureKey) : base(source)
        {
            _containerName = containerName;
            _signatureKey = signatureKey;
        }
    }
}