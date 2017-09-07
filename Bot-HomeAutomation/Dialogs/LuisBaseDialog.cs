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
using HomeAutomationWebAPI.Models;
using Bot_HomeAutomation.Helpers;

namespace Bot_HomeAutomation.Dialogs
{

    [LuisModel("8df3eb72-5ca3-4ca4-86ee-7e715efed5a7", "d44d9ba581e5412abd2352835c2c6385", domain: "southeastasia.api.cognitive.microsoft.com", Staging =false)]
    [Serializable]
    public class LuisBaseDialog : LuisDialog<object>
    {
        private DeviceControlHelper _deviceControlHelper;
        private string _iotDeviceId;


        public LuisBaseDialog()
        {
            _deviceControlHelper = new DeviceControlHelper();
            _iotDeviceId = "homehub-01";
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

            _deviceControlHelper.ControlDeviceSwitchAsync(_iotDeviceId, ElementType.Cooler, SwitchStatus.Off);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Cooler.On")]
        public async Task LuisSwitchCoolerOn(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchCoolerOn: Turning on the Cooler...";
            await context.PostAsync(message);

            _deviceControlHelper.ControlDeviceSwitchAsync(_iotDeviceId, ElementType.Cooler, SwitchStatus.On);

            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Switch.Heater.Off")]
        public async Task LuisSwitchHeaterOff(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchHeaterOff: Turning off the Heater...";
            await context.PostAsync(message);

            _deviceControlHelper.ControlDeviceSwitchAsync(_iotDeviceId, ElementType.Heater, SwitchStatus.Off);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Heater.On")]
        public async Task LuisSwitchHeaterOn(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchHeaterOn: Turning on the Heater...";
            await context.PostAsync(message);

            _deviceControlHelper.ControlDeviceSwitchAsync(_iotDeviceId, ElementType.Heater, SwitchStatus.On);

            context.Wait(this.MessageReceived);
        }



        [LuisIntent("Switch.Light.Off")]
        public async Task LuisSwitchLightOff(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchLightOff: Turning off the Light...";
            await context.PostAsync(message);

            _deviceControlHelper.ControlDeviceSwitchAsync(_iotDeviceId, ElementType.Light, SwitchStatus.Off);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Light.On")]
        public async Task LuisSwitchLightOn(IDialogContext context, LuisResult result)
        {
            string message = $"SwitchLightOn: Turning on the Light...";
            await context.PostAsync(message);

            _deviceControlHelper.ControlDeviceSwitchAsync(_iotDeviceId, ElementType.Light, SwitchStatus.On);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Get.Temperature")]
        public async Task LuisGetTemperature(IDialogContext context, LuisResult result)
        {
            string message = $"GetTemperature.";
            await context.PostAsync(message);

            await context.PostAsync(await _deviceControlHelper.GetTemperatureAsync(_iotDeviceId));

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Get.Humidity")]
        public async Task LuisGetHumidity(IDialogContext context, LuisResult result)
        {
            string message = $"GetHumidity.";
            await context.PostAsync(message);

            await context.PostAsync(await _deviceControlHelper.GetHumidityAsync(_iotDeviceId));

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Forecast.Rain")]
        public async Task LuisRainForecast(IDialogContext context, LuisResult result)
        {
            string message = $"RainForecast.";
            await context.PostAsync(message);

            double probabilityRain = 0;

            try
            {
                probabilityRain = await _deviceControlHelper.GetRainForecastAsync(_iotDeviceId, context);
            }
            catch (NullReferenceException e)
            {
                await context.PostAsync($"Check if the device that bot is trying to talk with is operational (Is the bot talking to right device?): {e.ToString()}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }

            if (probabilityRain == 0)
            {
                await context.PostAsync("Probability of rain is not predicted yet. Check device connectivity.");
            }
            else
            {
                await context.PostAsync(String.Format("Probability of rain is {0:0.00} %.", probabilityRain * 100));
            }

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Get.DeviceStatus")]
        public async Task LuisGetDeviceStatus(IDialogContext context, LuisResult result)
        {
            string message = $"GetDeviceStatus.";
            await context.PostAsync(message);

            //DeviceModel deviceStatus = 
            //await 
            DeviceModel deviceModel = await _deviceControlHelper.ReadDeviceStatusAsync(_iotDeviceId, context);

            try
            {
                await context.PostAsync($"deviceId={deviceModel.DeviceId}");
                await context.PostAsync($"time={deviceModel.Time}");
                await context.PostAsync($"location=long:{deviceModel.Location.Longitude},lat:{deviceModel.Location.Latitude}");
                await context.PostAsync($"temperature={deviceModel.Temperature}");
                await context.PostAsync($"humidity={deviceModel.Humidity}");
                await context.PostAsync($"forecast={deviceModel.Forecast}");

                foreach (VirtualDeviceElement element in deviceModel.Elements)
                {
                    await context.PostAsync($"elementType={element.ElementType}");
                    await context.PostAsync($"SwitchStatus={element.SwitchStatus}");
                    await context.PostAsync($"SetValue={element.SetValue}");
                    await context.PostAsync($"PowerConsumptionValue={element.PowerConsumptionValue}");
                    
                }


                
            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }
 

            context.Wait(this.MessageReceived);
        }


    }

}