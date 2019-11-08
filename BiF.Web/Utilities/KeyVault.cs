using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using BiF.DAL.Extensions;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;

namespace BiF.Web.Utilities
{
    public class KeyVault
    {
        private static KeyVault _keyVault;
        private static Dictionary<string, SecureString> _keys;

        public static string LastException { get; set; }

        private KeyVault() {
            _keys = new Dictionary<string, SecureString>();
        }

        public static async Task<string> GetSecret(string name) {

            if (_keyVault == null)
                _keyVault = new KeyVault();

            if (_keys.ContainsKey(name)) {
                return _keys[name].Unsecure();
            }
            
            try
            {
                /* The next four lines of code show you how to use AppAuthentication library to fetch secrets from your key vault */
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                string secretIdentifier = "https://beeritforward-kv.vault.azure.net/secrets/" + name;
                SecretBundle secret = await keyVaultClient.GetSecretAsync(secretIdentifier).ConfigureAwait(false);

                _keys.Add(name, secret.Value.Secure());
                LastException = null;
                return secret.Value;
            }
            /* If you have throttling errors see this tutorial https://docs.microsoft.com/azure/key-vault/tutorial-net-create-vault-azure-web-app */
            /// <exception cref="KeyVaultErrorException">
            /// Thrown when the operation returned an invalid status code
            /// </exception>
            catch (KeyVaultErrorException keyVaultException)
            {
                LastException = keyVaultException.Message;
                return null;
            }



        }

    }
}