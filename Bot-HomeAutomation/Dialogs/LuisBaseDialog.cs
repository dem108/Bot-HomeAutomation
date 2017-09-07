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
        private CognitiveServicesHelper _cognitiveServicesHelper;
        private string _iotDeviceId;
        private static readonly bool _DEBUG = true;


        public LuisBaseDialog()
        {
            _deviceControlHelper = new DeviceControlHelper();
            _cognitiveServicesHelper = new CognitiveServicesHelper();
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
            if (_DEBUG) await context.PostAsync($"SwitchCoolerOff: Turning off the Cooler...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Cooler, SwitchStatus.Off);

            if (_DEBUG) await context.PostAsync(callResult);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Cooler.On")]
        public async Task LuisSwitchCoolerOn(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchCoolerOn: Turning on the Cooler...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Cooler, SwitchStatus.On);

            if (_DEBUG) await context.PostAsync(callResult);

            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Switch.Heater.Off")]
        public async Task LuisSwitchHeaterOff(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchHeaterOff: Turning off the Heater...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Heater, SwitchStatus.Off);

            if (_DEBUG) await context.PostAsync(callResult);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Heater.On")]
        public async Task LuisSwitchHeaterOn(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchHeaterOn: Turning on the Heater...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Heater, SwitchStatus.On);

            if (_DEBUG) await context.PostAsync(callResult);

            context.Wait(this.MessageReceived);
        }



        [LuisIntent("Switch.Light.Off")]
        public async Task LuisSwitchLightOff(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchLightOff: Turning off the Light...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Light, SwitchStatus.Off);

            if (_DEBUG) await context.PostAsync(callResult);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Light.On")]
        public async Task LuisSwitchLightOn(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchLightOn: Turning on the Light...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Light, SwitchStatus.On);

            if (_DEBUG) await context.PostAsync(callResult);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Get.Temperature")]
        public async Task LuisGetTemperature(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"GetTemperature.");

            double temperature = 0;

            try
            {
                temperature = await _deviceControlHelper.GetTemperatureAsync(context, _iotDeviceId);
            }
            catch (NullReferenceException e)
            {
                await context.PostAsync($"Check if the device that bot is trying to talk with is operational (Is the bot talking to right device?): {e.ToString()}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }

            if (temperature == 0)
            {
                await context.PostAsync("Temperature is not measured yet. Check device connectivity.");
            }
            else
            {
                await context.PostAsync(String.Format("Temperature is {0:0.00} degrees Centigrade.", temperature));
            }
            
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Get.Humidity")]
        public async Task LuisGetHumidity(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"GetHumidity.");

            double humidity = 0;
            
            try
            {
                humidity = await _deviceControlHelper.GetHumidityAsync(context, _iotDeviceId);
            }
            catch (NullReferenceException e)
            {
                await context.PostAsync($"Check if the device that bot is trying to talk with is operational (Is the bot talking to right device?): {e.ToString()}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }

            if (humidity == 0)
            {
                await context.PostAsync("Humidity is not measured yet. Check device connectivity.");
            }
            else
            {
                await context.PostAsync(String.Format("Humidity is {0:0.00} % r H.", humidity));
            }
            
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Forecast.Rain")]
        public async Task LuisRainForecast(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"RainForecast.");

            double probabilityRain = 0;

            try
            {
                probabilityRain = await _deviceControlHelper.GetRainForecastAsync(context, _iotDeviceId);
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
            if (_DEBUG) await context.PostAsync($"GetDeviceStatus.");

            //DeviceModel deviceStatus = 
            //await 
            DeviceModel deviceModel = await _deviceControlHelper.ReadDeviceStatusAsync(context, _iotDeviceId);

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

        [LuisIntent("Capture.Image")]
        public async Task LuisCaptureImage(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"CaptureImage.");
            await context.PostAsync($"Let me see what's happening in the room...");

            try
            {
                string resultCapture = await _deviceControlHelper.CaptureImageAsync(context, _iotDeviceId);
                if (_DEBUG) await context.PostAsync(resultCapture);
                string imageBlobUrl = "";

                var parsed = JObject.Parse(resultCapture);
                foreach (var item in parsed)
                {
                    if (_DEBUG) await context.PostAsync($"key:{item.Key}, value:{item.Value}");
                    if (item.Key == "result")
                    {
                        var parsedInternalJObject = JObject.Parse((string) item.Value);
                        foreach (var itemInternal in parsedInternalJObject)
                        {
                            if (itemInternal.Key == "imageBlobUrl")
                            {
                                imageBlobUrl = (string)itemInternal.Value;
                            }
                        }
                    }
                }

                if (imageBlobUrl != "")
                {
                    await DescribeImage(context, imageBlobUrl);
                }
                else
                {
                    await context.PostAsync("No image found. Check device functionality");
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
            
            context.Wait(this.MessageReceived);
        }

        private async Task DescribeImage(IDialogContext context, string imageBlobUrl)
        {
            string description = "default desciption";
            try
            {
                description = await _cognitiveServicesHelper.PostComputerVisionDescribeAsync(context, imageBlobUrl);
            }
            catch (NullReferenceException e)
            {
                await context.PostAsync($"Check if the bot has access to Cognitive Services: {e.ToString()}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }

            var message = context.MakeMessage();
            message.Attachments = new List<Attachment>();

            var card = new HeroCard()
            {
                Title = "Room Now",
                Subtitle = description,
                Images = new List<CardImage>()
                {
                    new CardImage()
                    {
                        Url = imageBlobUrl
                    }
                }
            };

            message.Attachments.Add(card.ToAttachment());

            await context.PostAsync(message);

        }

        [LuisIntent("Find.Person")]
        public async Task LuisFindPerson(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"FindPerson.");

            await context.PostAsync("I'm not tracking wearables etc today. Let's see what I can find from the room camera.");

            await LuisCaptureImage(context, result);
            //TODO: check how it handles result

            try
            {
                throw new NotImplementedException();


            }
            catch (Exception e)
            {
                await context.PostAsync($"Bot needs some care: {e.ToString()}");
            }


            context.Wait(this.MessageReceived);
        }


    }

}