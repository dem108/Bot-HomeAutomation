using System;
using System.Net.Http;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using HomeAutomationWebAPI.Models;

namespace Bot_HomeAutomation.Helpers
{
    [Serializable]
    public class DeviceControlHelper
    {
        private static readonly bool _DEBUG = false;

        [NonSerialized] private HttpClient httpclient;
        private string _iotPlatformApiBaseAddress = "http://homeautomationwebapi.azurewebsites.net/";

        public async Task<string> PostDeviceElementSwitchAsync(IDialogContext context, string requestUri, DeviceElementSwitchModel control)
        {
            var content = JsonConvert.SerializeObject(control);
            var response = await httpclient.PostAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"));
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        public async Task<string> GetAsync(string requestUri)
        {
            var response = await httpclient.GetAsync(requestUri);
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        public async Task<string> ControlDeviceSwitchAsync(IDialogContext context, string deviceID, ElementType elementType, SwitchStatus switchStatus)
        {

            httpclient = new HttpClient
            {
                BaseAddress = new Uri(_iotPlatformApiBaseAddress)
            };

            var deviceElementSwitch = new DeviceElementSwitchModel
            {
                DeviceId = deviceID,
                ElementType = elementType,
                SwitchStatus = switchStatus
            };
            string result = await PostDeviceElementSwitchAsync(context, "api/deviceElement/setdeviceelementswitch", deviceElementSwitch);
            return result;
        }

        public async Task<double> GetTemperatureAsync(IDialogContext context, string deviceId)
        {
            /*
            httpclient = new HttpClient
            {
                BaseAddress = new Uri(_iotPlatformApiBaseAddress)
            };

            string temperature = await GetAsync($"api/deviceElement/Temperature?deviceId={deviceId}");
            return temperature;
            */

            DeviceModel deviceModel = await ReadDeviceStatusAsync(context, deviceId);

            return deviceModel.Temperature;



        }
        public async Task<double> GetHumidityAsync(IDialogContext context, string deviceId)
        {
            /*
            httpclient = new HttpClient
            {
                BaseAddress = new Uri(_iotPlatformApiBaseAddress)
            };

            string temperature = await GetAsync($"api/deviceElement/Humidity?deviceId={deviceId}");
            return temperature;
            */

            DeviceModel deviceModel = await ReadDeviceStatusAsync(context, deviceId);

            return deviceModel.Temperature;


        }

        public async Task<double> GetRainForecastAsync(IDialogContext context, string deviceId)
        { 
            DeviceModel deviceModel = await ReadDeviceStatusAsync(context, deviceId);

            return deviceModel.Forecast;
            
        }

        public async Task<DeviceModel> ReadDeviceStatusAsync(IDialogContext context, string deviceId)
        {
            httpclient = new HttpClient
            {
                BaseAddress = new Uri(_iotPlatformApiBaseAddress)
            };

            var response = await httpclient.GetAsync($"api/deviceElement/ReadDeviceStatus?deviceId={deviceId}");
            var parsed = JObject.Parse(await response.Content.ReadAsStringAsync());

            DeviceModel deviceModel = new DeviceModel();
            //CHECK: leak

            try
            {
                foreach (var item in parsed)
                {
                    var key = item.Key;
                    var value = item.Value;

                    //
                    if (_DEBUG) await context.PostAsync($"key={key}, value={value}");


                    if (item.Key == "result")
                    {
                        deviceModel = JsonConvert.DeserializeObject<DeviceModel>((string)item.Value);
                        //await context.PostAsync($"deviceId={deviceModelParsed.DeviceId}, time={deviceModelParsed.Time}");
                    }
                }

            }
            catch (NullReferenceException e)
            {
                await context.PostAsync($"Check if the device that bot is trying to talk with is operational (Is the bot talking to right device?): {e.ToString()}");

            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }

            return deviceModel;
        }



        public async Task<string> CaptureImageAsync(IDialogContext context, string deviceID)
        {

            httpclient = new HttpClient
            {
                BaseAddress = new Uri(_iotPlatformApiBaseAddress)
            };

            var deviceElementSwitch = new DeviceElementSwitchModel
            {
                DeviceId = deviceID,
                ElementType = ElementType.Camera,
                SwitchStatus = SwitchStatus.On

            };

            if (_DEBUG) Console.WriteLine($"DeviceControlHelper: CaptureImageAsync: {deviceElementSwitch.ToJsonString(formatting: Formatting.None)}");

            string result = await PostDeviceElementSwitchAsync(context, "api/deviceElement/CaptureImage", deviceElementSwitch);
            return result;
        }



    }
}