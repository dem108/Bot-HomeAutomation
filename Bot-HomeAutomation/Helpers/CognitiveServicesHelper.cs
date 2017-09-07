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
using Microsoft.ProjectOxford;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace Bot_HomeAutomation.Helpers
{
    [Serializable]
    public class CognitiveServicesHelper
    { 
        private static readonly bool _DEBUG = true;

        [NonSerialized] private HttpClient httpclient;
        [NonSerialized] private VisionServiceClient visionclient;

        private string _cognitiveVisionApiBaseAddress = "https://southeastasia.api.cognitive.microsoft.com/vision/v1.0";
        private string _cognitiveVisionApiServiceName = "BHA-ComputerVision";
        private string _cognitiveVisionApiServiceKey = "95f77c17619940028adf930e32bbe658";

        private string _cognitiveFaceApiBaseAddress = "https://southeastasia.api.cognitive.microsoft.com/face/v1.0";
        private string _cognitiveFaceApiServiceName = "BHA-Face";
        private string _cognitiveFaceApiServiceKey = "e3c93d4b512a42b5b5eb0c04f102c81f";

        private string _cognitiveEmotionApiBaseAddress = "https://westus.api.cognitive.microsoft.com/emotion/v1.0";
        private string _cognitiveEmotionApiServiceName = "BHA-Emotion";
        private string _cognitiveEmotionApiServiceKey = "cda54e8899ca45bd9c2616d3e81a3348";

        public async Task<string> PostComputerVisionAnalyzeAsync(IDialogContext context, string imageUrl)
        {
            using (httpclient = new HttpClient())
            {
                // Request headers.
                httpclient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _cognitiveVisionApiServiceKey);

                // Assemble the URI for the REST API Call.
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["visualFeatures"] = "Description,Faces"; // "Categories,Description,Color";
                queryString["details"] = "Celebrities";
                queryString["language"] = "en";
                string requestUri = _cognitiveVisionApiBaseAddress + "/analyze?" + queryString;
                HttpContent content = new StringContent(JsonConvert.SerializeObject(new { url = imageUrl }), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpclient.PostAsync(requestUri, content);
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public async Task<string> PostComputerVisionDescribeAsync(IDialogContext context, string imageUrl)
        {
            using (httpclient = new HttpClient())
            {
                // Request headers.
                httpclient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _cognitiveVisionApiServiceKey);

                // Assemble the URI for the REST API Call.
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["maxCandidates"] = "2";
                string requestUri = _cognitiveVisionApiBaseAddress + "/describe?" + queryString;
                HttpContent content = new StringContent(JsonConvert.SerializeObject(new { url = imageUrl }), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpclient.PostAsync(requestUri, content);

                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public async Task<Caption[]> SdkComputerVisionDescribeAsync(IDialogContext context, string imageUrl, int maxCandidates = 1)
        {
            visionclient = new VisionServiceClient(_cognitiveVisionApiServiceKey);
            Caption[] captions = new Caption[] { new Caption() { Text = "", Confidence = 0 } };
            //Check leak

            try
            {
                AnalysisResult analysisResult = await visionclient.DescribeAsync(imageUrl, maxCandidates);

                captions = analysisResult.Description.Captions;

            }
            catch (NullReferenceException e)
            {
                await context.PostAsync($"Check if the device that bot is trying to talk with is operational (Is the bot talking to right device?): {e.ToString()}");

            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }


            return captions;
            
        }


        public async Task<string> GetAsync(IDialogContext context, string requestUri)
        {
            var response = await httpclient.GetAsync(requestUri);
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }
        


    }
}