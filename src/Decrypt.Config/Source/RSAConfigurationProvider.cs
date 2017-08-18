using Microsoft.Extensions.Configuration.Json;

namespace Decrypt.Config.Source {
    public class RSAConfigurationProvider : JsonConfigurationProvider
    {
        private readonly string _containerName;

        public override void Load()
        {
            base.Load();

            this.Data = new RSADictionary<string>(Data, _containerName);
        }

        public RSAConfigurationProvider(JsonConfigurationSource source, string containerName) : base(source)
        {
            _containerName = containerName;
        }
    }
}