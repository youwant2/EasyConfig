namespace EasyConfig.SiteExtension
{
    using System;
    using System.Collections.Generic;
    using Azure.Identity;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureAppConfiguration(
                    (ctx, builder) =>
                    {
                        //Build the config from sources we have
                        var config = builder.Build();

                        var uriList = new List<string>();

                        // Get the uri for the Vault from configuration
                        // Try to get a string from configutation
                        // This will happen when the config looks like:
                        // {
                        //     "KeyVault":{
                        //         "Uri": "sample.vault.azure.net/"
                        //     }
                        // }
                        var uriString = config["KeyVault:Uri"];
                        if (!string.IsNullOrWhiteSpace(uriString))
                        {
                            uriList.Add(uriString);
                        }
                        // This will happen when the config looks like:
                        // {
                        //     "KeyVault":{
                        //         "Uri": [
                        //             "sample1.vault.azure.net/",
                        //             "sample2.vault.azure.net/"
                        //         ]
                        //     }
                        // }
                        else
                        {
                            uriList = config.GetSection("KeyVault:Uri").Get<List<string>>();
                        }


                        // Add KeyVault only if the uri is not empty
                        if (uriList?.Count > 0)
                        {
                            foreach (var uri in uriList)
                            {
                                if (Uri.TryCreate(uri, UriKind.Absolute, out var keyvaultUri))
                                {
                                    // Add Key Vault to configuration pipeline
                                    _ = builder.AddAzureKeyVault(
                                        keyvaultUri,
                                        new DefaultAzureCredential(),
                                        new PrefixKeyVaultSecretManager()
                                    );
                                }
                                else
                                {
                                    // TODO: Write Log
                                }
                            }
                        }
                    }
                );
    }
}
