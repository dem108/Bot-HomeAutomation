using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using HomeAutomationPlatformAPI.Models;

namespace Bot_HomeAutomation.Dialogs
{

    [LuisModel("8df3eb72-5ca3-4ca4-86ee-7e715efed5a7", "d44d9ba581e5412abd2352835c2c6385", domain: "southeastasia.api.cognitive.microsoft.com", Staging =false)]
    [Serializable]
    public class LuisBaseDialog : LuisDialog<object>
    {
        static HttpClient httpclient;
        static public string iotDeviceId = "homehub-02";
        static public string iotPlatformApiBaseAddress = "http://homeautomationwebapi.azurewebsites.net/";
        //static public string iotPlatformApiBaseAddress = "http://localhost:58675/";


        //static void PostAsync(string requestUri, DeviceElementStatus control)
        static async Task<string> PostDeviceElementSwitchAsync(string requestUri, DeviceElementSwitchModel control)
        {
            var content = JsonConvert.SerializeObject(control);
            var response = await httpclient.PostAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"));
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        static async Task<string> GetAsync(string requestUri)
        {
            var response = await httpclient.GetAsync(requestUri);
            string result = response.Content.ReadAsStringAsync().Result;

            return result;            
        }

        static async void ControlDeviceSwitch(string deviceID, ElementType elementType, SwitchStatus switchStatus)
        {

            httpclient = new HttpClient
            {
                BaseAddress = new Uri(iotPlatformApiBaseAddress)
            };

            var deviceElementSwitch = new DeviceElementSwitchModel
            {
                DeviceId = deviceID,
                ElementType = elementType,
                SwitchStatus = switchStatus
            };
            string result = await PostDeviceElementSwitchAsync("api/deviceElement/setdeviceelementswitch", deviceElementSwitch);    
        }

        static async Task<string> GetTemperature(string deviceId)
        {
            httpclient = new HttpClient
            {
                BaseAddress = new Uri(iotPlatformApiBaseAddress)
            };

            string temperature = await GetAsync($"api/deviceElement/Temperature?deviceId={deviceId}");
            return temperature;
        }
        static async Task<string> GetHumidity(string deviceId)
        {
            httpclient = new HttpClient
            {
                BaseAddress = new Uri(iotPlatformApiBaseAddress)
            };

            string temperature = await GetAsync($"api/deviceElement/Humidity?deviceId={deviceId}");
            return temperature;
        }



        public class StatusMessage
        {
            public string time;
            public int statusCode;
            public string resultJson;
            
        }
        

        static async void ReadDeviceStatus(string deviceId, IDialogContext context)
        {
            httpclient = new HttpClient
            {
                BaseAddress = new Uri(iotPlatformApiBaseAddress)
            };

            StatusMessage statusMessage;

            DeviceModel deviceModel;

            var response = await httpclient.GetAsync($"api/deviceElement/ReadDeviceStatus?deviceId={deviceId}");
            var parsed = JObject.Parse(await response.Content.ReadAsStringAsync());

            List<object> myList = new List<object>();

            await context.PostAsync("ReadDeviceStatus starts.");

            try
            {
                foreach (var item in parsed)
                {
                    var key = item.Key;
                    var value = item.Value;

                    if (item.Key == "result")
                    {


                        var innerJObject = item.Value;
                        var resultArray = JObject.Parse((string)innerJObject);
                        foreach (var resultItem in resultArray)
                        {
                            await context.PostAsync($"key:{resultItem.Key}, value:{resultItem.Value}");
                        }

                        await context.PostAsync("Trying deserialization.");

                        var deviceModelParsed = JsonConvert.DeserializeObject<DeviceModel>((string)item.Value);
                        await context.PostAsync($"deviceId={deviceModelParsed.DeviceId}, time={deviceModelParsed.Time}");

                    }
                }

            }
            catch (Exception e)
            {
                await context.PostAsync(e.ToString());
            }

            return;
        }

        


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task LuisNone(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";
            
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }
        
        [LuisIntent("Help")]
        public async Task LuisHelp(IDialogContext context, LuisResult result)
        {
            string message = $"Help: You can do these things...";

            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Cooler.Off")]
        public async Task LuisSwitchCoolerOff(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchCoolerOff: Turning off the Cooler...";
            await context.PostAsync(message);

            ControlDeviceSwitch(iotDeviceId, ElementType.Cooler, SwitchStatus.Off);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Cooler.On")]
        public async Task LuisSwitchCoolerOn(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchCoolerOn: Turning on the Cooler...";
            await context.PostAsync(message);

            ControlDeviceSwitch(iotDeviceId, ElementType.Cooler, SwitchStatus.On);

            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Switch.Heater.Off")]
        public async Task LuisSwitchHeaterOff(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchHeaterOff: Turning off the Heater...";
            await context.PostAsync(message);

            ControlDeviceSwitch(iotDeviceId, ElementType.Heater, SwitchStatus.Off);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Heater.On")]
        public async Task LuisSwitchHeaterOn(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchHeaterOn: Turning on the Heater...";
            await context.PostAsync(message);

            ControlDeviceSwitch(iotDeviceId, ElementType.Heater, SwitchStatus.On);

            context.Wait(this.MessageReceived);
        }



        [LuisIntent("Switch.Light.Off")]
        public async Task LuisSwitchLightOff(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchLightOff: Turning off the Light...";
            await context.PostAsync(message);

            ControlDeviceSwitch(iotDeviceId, ElementType.Light, SwitchStatus.Off);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Light.On")]
        public async Task LuisSwitchLightOn(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchLightOn: Turning on the Light...";
            await context.PostAsync(message);

            ControlDeviceSwitch(iotDeviceId, ElementType.Light, SwitchStatus.On);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Get.Temperature")]
        public async Task LuisGetTemperature(IDialogContext context, LuisResult result)
        {
            string message = $"GetTemperature.";
            await context.PostAsync(message);

            await context.PostAsync(await GetTemperature(iotDeviceId));

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Get.Humidity")]
        public async Task LuisGetHumidity(IDialogContext context, LuisResult result)
        {
            string message = $"GetHumidity.";
            await context.PostAsync(message);

            await context.PostAsync(await GetHumidity(iotDeviceId));

            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Get.DeviceStatus")]
        public async Task LuisGetDeviceStatus(IDialogContext context, LuisResult result)
        {
            string message = $"GetDeviceStatus.";
            await context.PostAsync(message);
            double value = -100;

            //DeviceModel deviceStatus = 
            //await 
            ReadDeviceStatus(iotDeviceId, context);

            /*
            foreach (VirtualDeviceElement deviceElement in deviceStatus.Elements)
            {
                await context.PostAsync($"ElementType: {deviceElement.ElementType}");
                await context.PostAsync($"SetValue: {deviceElement.SetValue}");

                if (deviceElement.ElementType == ElementType.Temperature)
                {
                    value = deviceElement.SetValue;
                }
            }
            await context.PostAsync($"Value is now {value}.");
            

            if (deviceStatus != null)
                await context.PostAsync($"deviceStatus: {deviceStatus.ToString()}.");
            else
                await context.PostAsync("couldn't get deviceStatus.");
            */


            context.Wait(this.MessageReceived);
        }
        

    }

}