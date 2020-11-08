using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Newtonsoft.Json;
using Xunit;

namespace Docman.IntegrationTests.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetAsync<T>(this HttpClient httpClient, Uri resourceUri)
        {
            var response = await httpClient.GetAsync(resourceUri);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
        
        public static async Task<Uri> CreateDocumentAsync(this HttpClient httpClient, string number, string description)
        {
            var createDocumentCommand = new CreateDocumentCommand(number, description);
            var content = GetStringContent(createDocumentCommand);

            var createResponse = await httpClient.PostAsync("/documents", content);

            createResponse.EnsureSuccessStatusCode();
            Assert.NotNull(createResponse.Headers.Location);

            return createResponse.Headers.Location;
        }

        public static async Task UpdateDocumentAsync(this HttpClient httpClient, Uri documentUri,
            UpdateDocumentCommand command)
        {
            var content = GetStringContent(command);

            var updateDocumentResponse = await httpClient.PutAsync(documentUri, content);
            updateDocumentResponse.EnsureSuccessStatusCode();
        }

        public static async Task<Uri> AddFileAsync(this HttpClient httpClient, Uri documentUri, AddFileCommand command)
        {
            var content = GetStringContent(command);
            var requestUri = new Uri(Path.Combine(documentUri.OriginalString, "files"), UriKind.Relative);

            var response = await httpClient.PostAsync(requestUri, content);
            
            response.EnsureSuccessStatusCode();
            Assert.NotNull(response.Headers.Location);

            return response.Headers.Location;
        }

        private static StringContent GetStringContent(object command)
        {
            var json = JsonConvert.SerializeObject(command);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}