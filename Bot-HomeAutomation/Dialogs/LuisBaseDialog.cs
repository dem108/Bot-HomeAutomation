using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Bot_HomeAutomation.Dialogs
{
   /*
 * Data Models used in API / Device Communication 
 */
    public enum ElementType
    {
        LGT = 0,
        FAN = 1,
        CAM = 2,
        TMP = 3,
        HMD = 4,
        ALL = 5
    }
    public enum ElementState
    {
        OFF = 0,
        ON = 1,
        VARIANT = 2,
    }

    public class DeviceElement
    {
        public ElementType ElementType { get; set; }
        public ElementState ElementState { get; set; }
        public string Value { get; set; }
    }
    public class DeviceElementStatus
    {
        public string DeviceId { get; set; }
        public DeviceElement[] DeviceElements { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }




    [LuisModel("2b768735-9bd2-4c31-9f89-74945da5ce01", "d44d9ba581e5412abd2352835c2c6385", domain: "southeastasia.api.cognitive.microsoft.com", Staging =false)]
    [Serializable]
    public class LuisBaseDialog : LuisDialog<object>
    {
        static HttpClient httpclient;

        static void PostAsync(string requestUri, DeviceElementStatus control)
        {
            var content = JsonConvert.SerializeObject(control);
            var response = httpclient
                .PostAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"))
                .ContinueWith(responseTask =>
                {
                    var result = responseTask.Result;
                    var json = result.Content.ReadAsStringAsync();
                    Console.WriteLine(json.Result);
                });
            response.Wait();
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";
            
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }
        
        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            string message = $"Help: You can do these things...";

            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Light.Off")]
        public async Task SwitchLightOff(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchLightOff: Turning off the light...";
            await context.PostAsync(message);

            httpclient = new HttpClient
            {
                BaseAddress = new Uri("http://homeautomationplatform.azurewebsites.net/")
            };
            var deviceElementStatus = new DeviceElementStatus
            {
                DeviceId = "gbb-kickoff-demo",
                DeviceElements = new DeviceElement[]{
                    new DeviceElement()
                    {
                        ElementType = ElementType.LGT,
                        ElementState = ElementState.OFF,
                        Value = "0.00"
                    }
                }
            };

            PostAsync("api/element/setdevices", deviceElementStatus);
            
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Light.On")]
        public async Task SwitchLightOn(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchLightOn: Turning on the light...";
            await context.PostAsync(message);


            httpclient = new HttpClient
            {
                BaseAddress = new Uri("http://homeautomationplatform.azurewebsites.net/")
            };
            var deviceElementStatus = new DeviceElementStatus
            {
                DeviceId = "gbb-kickoff-demo",
                DeviceElements = new DeviceElement[]{
                    new DeviceElement()
                    {
                        ElementType = ElementType.LGT,
                        ElementState = ElementState.ON,
                        Value = "0.00"
                    }
                }
            };

            PostAsync("api/element/setdevices", deviceElementStatus);

            context.Wait(this.MessageReceived);
        }



    }

}