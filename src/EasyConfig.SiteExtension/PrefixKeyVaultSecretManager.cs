namespace EasyConfig.SiteExtension
{
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Security.KeyVault.Secrets;
    using Microsoft.Extensions.Configuration;

    public class PrefixKeyVaultSecretManager : KeyVaultSecretManager
    {
        private readonly string prefix;
        private readonly bool removePrefix;

        public PrefixKeyVaultSecretManager(
            string prefix = "EASYCONFIG",
            bool removePrefix = false
        )
        {
            this.prefix = $"{prefix}--";
            this.removePrefix = removePrefix;
        }

        /// <summary>
        /// Maps secret to a configuration key.
        /// </summary>
        /// <param name="secret">The <see cref="KeyVaultSecret"/> instance.</param>
        /// <returns>Configuration key name to store secret value.</returns>
        public override bool Load(SecretProperties secret) => secret.Name.StartsWith(this.prefix);

        /// <summary>
        /// Checks if <see cref="KeyVaultSecret"/> value should be retrieved.
        /// </summary>
        /// <param name="secret">The <see cref="KeyVaultSecret"/> instance.</param>
        /// <returns><code>true</code> if secrets value should be loaded, otherwise <code>false</code>.</returns>
        public override string GetKey(KeyVaultSecret secret)
        {
            var secretIdentifier = secret.Name;

            if (this.removePrefix)
            {
                secretIdentifier = secretIdentifier.Substring(this.prefix.Length);
            }
            return secretIdentifier.Replace("--", ConfigurationPath.KeyDelimiter);
        }
    }
}
