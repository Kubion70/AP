using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AP.Cryptography;
using Newtonsoft.Json;

namespace AP.Tests.Helpers
{
    public static class AuthorizationHelper
    {
        public static async Task<AuthenticationHeaderValue> AdminAuthorization(HttpClient client)
        {
            var challangeResponse = await client.GetAsync("api/auth/Admin");
            string challangeBody = await challangeResponse.Content.ReadAsStringAsync();
            string salt = challangeBody.Trim('"');

            var hashedPassword = SHA.GenerateSHA256String("AAdmin12!");
            var hashedWithSalt = SHA.ComputePasswordAndSalt(hashedPassword, salt, SHA256.Create());
            
            var jsonString = JsonConvert.SerializeObject(hashedWithSalt);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var authResponse = await client.PostAsync("api/auth/Admin", content);
            string authBody = await authResponse.Content.ReadAsStringAsync();
            string token = authBody.Trim('"');
            return new AuthenticationHeaderValue("Bearer", token);
        }
    }
}