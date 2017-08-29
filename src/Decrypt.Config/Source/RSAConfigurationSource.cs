using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Decrypt.Config.Source
{
    class RSAConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);

            return new RSAConfigurationProvider(this, "");
        }
    }
}
