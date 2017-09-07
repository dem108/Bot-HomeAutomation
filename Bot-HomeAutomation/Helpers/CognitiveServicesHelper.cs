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
using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System.Collections.Generic;

namespace Bot_HomeAutomation.Helpers
{
    public class EmotionData
    {
        public string EmotionName { get; set; }
        public float EmotionScore { get; set; }
    }


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
                queryString["maxCandidates"] = "1";
                string requestUri = _cognitiveVisionApiBaseAddress + "/describe?" + queryString;
                HttpContent content = new StringContent(JsonConvert.SerializeObject(new { url = imageUrl }), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpclient.PostAsync(requestUri, content);

                return response.Content.ReadAsStringAsync().Result;
            }
        }


        public async Task<string> PostFaceDetectAsync(IDialogContext context, string imageUrl)
        {
            using (httpclient = new HttpClient())
            {
                // Request headers.
                httpclient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _cognitiveFaceApiServiceKey);

                // Assemble the URI for the REST API Call.
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["returnFaceAttributes"] = "age,gender,emotion"; //too many will take more time
                string requestUri = _cognitiveFaceApiBaseAddress+ "/detect?" + queryString;
                HttpContent content = new StringContent(JsonConvert.SerializeObject(new { url = imageUrl }), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpclient.PostAsync(requestUri, content);

                return response.Content.ReadAsStringAsync().Result;
            }
        }


        public async Task<string> GetDescriptionAsync(IDialogContext context, string imageBlobUrl)
        {
            string description = "default desciption";
            try
            {
                description = await PostComputerVisionDescribeAsync(context, imageBlobUrl);
                Caption[] captions = JsonConvert.DeserializeObject<AnalysisResult>(description).Description.Captions;
                //Caption[] captions = await _cognitiveServicesHelper.SdkComputerVisionDescribeAsync(context, imageBlobUrl);

                if (captions != null && captions.Length > 0)
                {
                    //if (_DEBUG) await context.PostAsync(String.Format("length: {0}", captions.Length));
                    description = "";
                    foreach (var item in captions)
                    {
                        description += $"{item.Text} ({Math.Round(item.Confidence, 2)})  ";
                    }
                    //description = $"{captions.GetValue([0].Text}({captions[0].Confidence})";
                }
            }
            catch (NullReferenceException e)
            {
                await context.PostAsync($"Check if the bot has access to Cognitive Services: {e.ToString()}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }

            return description;
        }

        public async Task<Microsoft.ProjectOxford.Face.Contract.Face[]> GetFacesAsync(IDialogContext context, string imageBlobUrl)
        {

            string faces = "";
            Microsoft.ProjectOxford.Face.Contract.Face[] detectedFaces = new Microsoft.ProjectOxford.Face.Contract.Face[] { };
            //potential leak

            try
            {
                faces = await PostFaceDetectAsync(context, imageBlobUrl);
                detectedFaces = JsonConvert.DeserializeObject<Microsoft.ProjectOxford.Face.Contract.Face[]>(faces);

            }
            catch (NullReferenceException e)
            {
                await context.PostAsync($"Check if the bot has access to Cognitive Services: {e.ToString()}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }

            return detectedFaces;
        }



        public async Task<string> GetAsync(IDialogContext context, string requestUri)
        {
            var response = await httpclient.GetAsync(requestUri);
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }



        //Due to potential risk of introducing more exceptions from visionclient, 
        //I am using REST API version(PostComputerVisionDescribeAsync) instead of Client Library. 
        //But the AnalysisResult class is still useful. 
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
            catch (Microsoft.ProjectOxford.Vision.ClientException e)
            {
                await context.PostAsync($"Failed to initiate Vision client. (Do you have access to Cognitive Services and a valid key?): {e.ToString()}");
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

        public IEnumerable<EmotionData> ScoresToEmotionData(EmotionScores scores)
        {
            List<EmotionData> result = new List<EmotionData>();
            result.Add(new EmotionData { EmotionName = "Anger", EmotionScore = scores.Anger });
            result.Add(new EmotionData { EmotionName = "Contempt", EmotionScore = scores.Contempt });
            result.Add(new EmotionData { EmotionName = "Disgust", EmotionScore = scores.Disgust });
            result.Add(new EmotionData { EmotionName = "Fear", EmotionScore = scores.Fear });
            result.Add(new EmotionData { EmotionName = "Happiness", EmotionScore = scores.Happiness });
            result.Add(new EmotionData { EmotionName = "Neutral", EmotionScore = scores.Neutral });
            result.Add(new EmotionData { EmotionName = "Sadness", EmotionScore = scores.Sadness });
            result.Add(new EmotionData { EmotionName = "Surprise", EmotionScore = scores.Surprise });
            return result;
        }


    }
}