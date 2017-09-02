using System;
using Microsoft.Extensions.Configuration;

namespace Decrypt.Config.ConfigurationProviders
{
    public static class EncryptedConfigurationExtensions
    {
        public static IConfigurationBuilder AddEncryptedFile(this IConfigurationBuilder builder, string path, string containerName, string signatureKey, bool optional = true, bool reload = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("must provide valid file path", nameof(path));

            HybridConfigurationSource configurationSource = new HybridConfigurationSource(containerName, signatureKey)
            {
                Path = path,
                Optional = optional,
                ReloadOnChange = reload,
            };

            configurationSource.ResolveFileProvider();

            builder.Add(configurationSource);

            return builder;
        }
    }
}
