using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace WcfFileClient
{
    public class CertHelper
    {
        public static async Task<X509Certificate2> GetAsync()
        {
            var keyVaultUrl = "https://kvmm.vault.azure.net/";
            var certificateName = "localhost";
            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            KeyVaultSecret secret = await client.GetSecretAsync(certificateName);
            var bytes = Convert.FromBase64String(secret.Value);
            // Specify X509KeyStorageFlags
            return new X509Certificate2(bytes, (string)null, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
        }
    }
}