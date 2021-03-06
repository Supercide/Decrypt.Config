# Decrypt.Config

[![Build status](https://ci.appveyor.com/api/projects/status/irvkwv3b3mmxyt41/branch/master?svg=true)](https://ci.appveyor.com/project/jordan-Anderson/decrypt-config/branch/master)

An extension package that provides methods for consuming encrypted configuration files for `Microsoft.Extensions.Configuration`.

Currently only supports encrypted json files via [Encrypt.Config](https://github.com/Supercide/Encrypt.Config) 

## Installing Decrypt.Config
To install [Decrypt.Config](https://www.nuget.org/packages/Decrypt.Config)

```
Install-Package Decrypt.Config
```

## Examples

```
ConfigurationBuilder builder = new ConfigurationBuilder();

builder.AddEncryptedFile(encryptedSettingsFile,
                         encryptionContainer, 
                         SignatureKey);
```

for information on how to create encryption containers and signing keys see [link](https://github.com/Supercide/Encrypt.Config/blob/master/README.MD)
