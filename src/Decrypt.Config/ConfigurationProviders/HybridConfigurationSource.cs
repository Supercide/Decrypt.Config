using Microsoft.Extensions.Configuration;

namespace Decrypt.Config.ConfigurationProviders
{
    public class HybridConfigurationSource : FileConfigurationSource
    {
        private readonly string _containerName;
        private readonly string _signatureKey;

        public HybridConfigurationSource(string containerName, string signatureKey)
        {
            _containerName = containerName;
            _signatureKey = signatureKey;
        }
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);

            return new HybridConfigurationProvider(this, _containerName, _signatureKey);
        }
    }
}
