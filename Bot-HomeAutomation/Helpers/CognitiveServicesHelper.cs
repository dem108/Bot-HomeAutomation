using System;
using System.Net.Http;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using HomeAutomationWebAPI.Models;
using System.Web;
using System.Net.Http.Headers;

namespace Bot_HomeAutomation.Helpers
{
    [Serializable]
    public class CognitiveServicesHelper
    { 
        private static readonly bool _DEBUG = true;

        [NonSerialized] private HttpClient httpclient;

        private string _cognitiveVisionApiBaseAddress = "https://southeastasia.api.cognitive.microsoft.com/vision/v1.0";
        private string _cognitiveVisionApiServiceName = "BHA-ComputerVision";
        private string _cognitiveVisionApiServiceKey = "95f77c17619940028adf930e32bbe658";

        private string _cognitiveFaceApiBaseAddress = "https://southeastasia.api.cognitive.microsoft.com/face/v1.0";
        private string _cognitiveFaceApiServiceName = "BHA-Face";
        private string _cognitiveFaceApiServiceKey = "e3c93d4b512a42b5b5eb0c04f102c81f";

        private string _cognitiveEmotionApiBaseAddress = "https://westus.api.cognitive.microsoft.com/emotion/v1.0";
        private string _cognitiveEmotionApiServiceName = "BHA-Emotion";
        private string _cognitiveEmotionApiServiceKey = "cda54e8899ca45bd9c2616d3e81a3348";

        public async Task<string> PostComputerVisionAnalyzeAsync(string imageUrl)
        {
            httpclient = new HttpClient();
            // Request headers.
            httpclient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _cognitiveVisionApiServiceKey);
            
            // Assemble the URI for the REST API Call.
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            queryString["visualFeatures"] = "Description"; // "Categories,Description,Color";
            queryString["details"] = "Celebrities";
            queryString["language"] = "en";
            string requestUri = _cognitiveVisionApiBaseAddress + "/analyze?" + queryString;
            
            var bodyJson = new
            {
                url = imageUrl
            };
            
            var content = JsonConvert.SerializeObject(bodyJson);

            HttpResponseMessage response = await httpclient.PostAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"));

            string result = response.Content.ReadAsStringAsync().Result;
            
            return result;
        }

        public async Task<string> GetAsync(string requestUri)
        {
            var response = await httpclient.GetAsync(requestUri);
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }
        


    }
}