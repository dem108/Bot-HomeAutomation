﻿using System;
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
        [NonSerialized] private HttpClient httpclient;
        private string _iotPlatformApiBaseAddress = "http://homeautomationwebapi.azurewebsites.net/";
        //static public string _iotPlatformApiBaseAddress = "http://localhost:58675/";

        /*
        public DeviceControlHelper()
        { 
            _iotPlatformApiBaseAddress = "http://homeautomationwebapi.azurewebsites.net/";
            //_iotPlatformApiBaseAddress = "http://localhost:58675/";

        }
        */


        public async Task<string> PostDeviceElementSwitchAsync(string requestUri, DeviceElementSwitchModel control)
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

        public async void ControlDeviceSwitchAsync(string deviceID, ElementType elementType, SwitchStatus switchStatus)
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
            string result = await PostDeviceElementSwitchAsync("api/deviceElement/setdeviceelementswitch", deviceElementSwitch);
        }

        public async Task<string> GetTemperatureAsync(string deviceId)
        {
            httpclient = new HttpClient
            {
                BaseAddress = new Uri(_iotPlatformApiBaseAddress)
            };

            string temperature = await GetAsync($"api/deviceElement/Temperature?deviceId={deviceId}");
            return temperature;
        }
        public async Task<string> GetHumidityAsync(string deviceId)
        {
            httpclient = new HttpClient
            {
                BaseAddress = new Uri(_iotPlatformApiBaseAddress)
            };

            string temperature = await GetAsync($"api/deviceElement/Humidity?deviceId={deviceId}");
            return temperature;
        }


        public async Task<DeviceModel> ReadDeviceStatusAsync(string deviceId, IDialogContext context)
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

                    if (item.Key == "result")
                    {
                        deviceModel = JsonConvert.DeserializeObject<DeviceModel>((string)item.Value);
                        //await context.PostAsync($"deviceId={deviceModelParsed.DeviceId}, time={deviceModelParsed.Time}");
                    }
                }

            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }

            return deviceModel;
        }


        
    }
}