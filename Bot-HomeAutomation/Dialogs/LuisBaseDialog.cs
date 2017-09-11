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
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Common.Contract;
using AdaptiveCards;

namespace Bot_HomeAutomation.Dialogs
{

    [LuisModel("8df3eb72-5ca3-4ca4-86ee-7e715efed5a7", "d44d9ba581e5412abd2352835c2c6385", domain: "southeastasia.api.cognitive.microsoft.com", Staging =false)]
    [Serializable]
    public class LuisBaseDialog : LuisDialog<object>
    {
        private DeviceControlHelper _deviceControlHelper;
        private CognitiveServicesHelper _cognitiveServicesHelper;
        private string _iotDeviceId;
        private static bool _DEBUG = false;
        private static string _version = "20170911 1800 KST";


        public LuisBaseDialog()
        {
            _deviceControlHelper = new DeviceControlHelper();
            _cognitiveServicesHelper = new CognitiveServicesHelper();
            _iotDeviceId = "homehub-01";
        }

        public EntityRecommendation GetTopEntity(IList<EntityRecommendation> entities)
        {
            EntityRecommendation entity = null;
            float tempScore = 0;
            foreach(EntityRecommendation item in entities)
            {
                if (item.Score > tempScore) //not inclusive of '='
                    entity = item;
            }

            return entity;
        }


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task LuisNone(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I am, '{result.Query}', I did not understand. 'Help' you may type, if you need assistance.";
            
            await context.PostAsync(message);
            //            context.Wait(this.MessageReceived);
            context.Done(context.MakeMessage());
        }


        [LuisIntent("Hello")]
        public async Task LuisHello(IDialogContext context, LuisResult result)
        {
            string message = $"Hello, there, Mr. Han So Slow. Yoda Man I am, and 'Help' you can say, at any time if you need assistance.";

            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
//            context.Done(context.MakeMessage());
        }



        [LuisIntent("Start.Over")]
        public async Task LuisStartOver(IDialogContext context, LuisResult result)
        {
            context.PostAsync("Starting over, I am...");
            context.Done(context.MakeMessage());
        }


        [LuisIntent("SET.DEBUG")]
        public async Task LuisSetDebug(IDialogContext context, LuisResult result)
        {
            EntityRecommendation entity = GetTopEntity(result.Entities);

            if (_DEBUG) context.PostAsync($"topEntity: {entity}");

            string message;

            if (entity != null && entity.Entity == "on")
            {
                _DEBUG = true;
                message = $"_DEBUG set to true.";
            }
            else if (entity.Entity == "off")
            {
                _DEBUG = false;
                message = $"_DEBUG set to false.";
            }
            else
            {
                message = $"Did nothing.";
            }

            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Check.Version")]
        public async Task LuisCheckVersion(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"version: {_version}");
            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Help")]
        public async Task LuisHelp(IDialogContext context, LuisResult result)
        {
            string message = $"Help: Click from the top menu you can, or just type what you want you can. \nForce is strong with the Home Automation device. \nThings you may try: turn the light off, switch the heater on, show device status, and even show who's in the room.";
            await context.PostAsync(message);
            //context.Wait(this.MessageReceived);
            context.Done(context.MakeMessage());
        }

        [LuisIntent("Switch.Cooler.Off")]
        public async Task LuisSwitchCoolerOff(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchCoolerOff: Turning off the Cooler...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Cooler, SwitchStatus.Off);
            if (_DEBUG) await context.PostAsync(callResult);

            callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceValueAsync(context, _iotDeviceId, ElementType.Cooler, 0);
            if (_DEBUG) await context.PostAsync(callResult);
            
            await context.PostAsync("Used the Force to turn off the cooler.");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Cooler.On")]
        public async Task LuisSwitchCoolerOn(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchCoolerOn: Turning on the Cooler...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Cooler, SwitchStatus.On);
            if (_DEBUG) await context.PostAsync(callResult);

            callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceValueAsync(context, _iotDeviceId, ElementType.Cooler, 20);
            if (_DEBUG) await context.PostAsync(callResult);
            
            await context.PostAsync("Used the Force to turn on the cooler.");
            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Switch.Heater.Off")]
        public async Task LuisSwitchHeaterOff(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchHeaterOff: Turning off the Heater...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Heater, SwitchStatus.Off);        
            if (_DEBUG) await context.PostAsync(callResult);

            callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceValueAsync(context, _iotDeviceId, ElementType.Heater, 0);
            if (_DEBUG) await context.PostAsync(callResult);

            await context.PostAsync("Used the Force to turn off the heater.");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Heater.On")]
        public async Task LuisSwitchHeaterOn(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchHeaterOn: Turning on the Heater...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Heater, SwitchStatus.On);
            if (_DEBUG) await context.PostAsync(callResult);

            callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceValueAsync(context, _iotDeviceId, ElementType.Heater, 20);
            if (_DEBUG) await context.PostAsync(callResult);

            await context.PostAsync("Used the Force to turn on the heater.");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Make.It.Cooler")]
        public async Task LuisMakeItCooler(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"MakeItCooler...");

            //first read current status
            DeviceModel deviceModel = await _deviceControlHelper.ReadDeviceStatusAsync(context, _iotDeviceId);

            SwitchStatus heaterSwitch = SwitchStatus.Off;
            SwitchStatus coolerSwitch = SwitchStatus.Off;
            double heaterValue = 0.0;
            double coolerValue = 0.0;
            foreach (var item in deviceModel.Elements)
            {
                if (item.ElementType == ElementType.Heater)
                {
                    heaterSwitch = item.SwitchStatus;
                    heaterValue = item.SetValue;
                }
                else if (item.ElementType == ElementType.Cooler)
                {
                    coolerSwitch = item.SwitchStatus;
                    coolerValue = item.SetValue;
                }
            }
            //TODO: Refactor: I could encapsulate this.


            string callResult = "";
            //Now, if heater is on, and value is already >0, decrease the heater power.
            if (heaterSwitch == SwitchStatus.On && heaterValue > 0)
            {
                callResult = "";
                callResult = await _deviceControlHelper.ControlDeviceValueChangeAsync(context, _iotDeviceId, ElementType.Heater, -20);
                if (_DEBUG) await context.PostAsync(callResult);
            }
            //in other cases, make sure heater is off and cooler is on, and increase the cooler power.
            else
            {
                callResult = "";
                callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Heater, SwitchStatus.Off);
                if (_DEBUG) await context.PostAsync(callResult);

                callResult = "";
                callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Cooler, SwitchStatus.On);
                if (_DEBUG) await context.PostAsync(callResult);

                callResult = "";
                callResult = await _deviceControlHelper.ControlDeviceValueChangeAsync(context, _iotDeviceId, ElementType.Cooler, 20); //20 cooler
                if (_DEBUG) await context.PostAsync(callResult);
            }

            await context.PostAsync("Used the Force to make the room cooler.");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Make.It.Warmer")]
        public async Task LuisMakeItWarmer(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"MakeItWarmer...");

            //first read current status
            DeviceModel deviceModel = await _deviceControlHelper.ReadDeviceStatusAsync(context, _iotDeviceId);

            SwitchStatus heaterSwitch = SwitchStatus.Off;
            SwitchStatus coolerSwitch = SwitchStatus.Off;
            double heaterValue = 0.0;
            double coolerValue = 0.0;
            foreach (var item in deviceModel.Elements)
            {
                if (item.ElementType == ElementType.Heater)
                {
                    heaterSwitch = item.SwitchStatus;
                    heaterValue = item.SetValue;
                }
                else if (item.ElementType == ElementType.Cooler)
                {
                    coolerSwitch = item.SwitchStatus;
                    coolerValue = item.SetValue;
                }
            }
            //TODO: Refactor: I could encapsulate this.


            //TODO: Also, i could encapsulate the below.

            string callResult = "";
            //Now, if cooler is on, and value is already >0, decrease the cooler power.
            if (coolerSwitch == SwitchStatus.On && coolerValue > 0)
            {
                callResult = "";
                callResult = await _deviceControlHelper.ControlDeviceValueChangeAsync(context, _iotDeviceId, ElementType.Cooler, -20);
                if (_DEBUG) await context.PostAsync(callResult);
            }
            //in other cases, make sure cooler is off and heater is on, and increase the heater power.
            else
            {
                callResult = "";
                callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Cooler, SwitchStatus.Off);
                if (_DEBUG) await context.PostAsync(callResult);

                callResult = "";
                callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Heater, SwitchStatus.On);
                if (_DEBUG) await context.PostAsync(callResult);

                callResult = "";
                callResult = await _deviceControlHelper.ControlDeviceValueChangeAsync(context, _iotDeviceId, ElementType.Heater, 20); //20 cooler
                if (_DEBUG) await context.PostAsync(callResult);
            }

            await context.PostAsync("Used the Force to make the room warmer.");
            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Switch.Light.Off")]
        public async Task LuisSwitchLightOff(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchLightOff: Turning off the Light...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Light, SwitchStatus.Off);
            if (_DEBUG) await context.PostAsync(callResult);

            callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceValueAsync(context, _iotDeviceId, ElementType.Light, 0);
            if (_DEBUG) await context.PostAsync(callResult);

            await context.PostAsync("Used the Force to turn off the light.");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Switch.Light.On")]
        public async Task LuisSwitchLightOn(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"SwitchLightOn: Turning on the Light...");

            string callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceSwitchAsync(context, _iotDeviceId, ElementType.Light, SwitchStatus.On);
            if (_DEBUG) await context.PostAsync(callResult);

            callResult = "";
            callResult = await _deviceControlHelper.ControlDeviceValueAsync(context, _iotDeviceId, ElementType.Light, 80);
            if (_DEBUG) await context.PostAsync(callResult);

            await context.PostAsync("Used the Force to turn on the light.");
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
                await context.PostAsync($"Great disturbance of the Force with the device. (Am I talking to the right device?): {e.ToString()}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Meditation I need: {e.ToString()}");
            }

            if (temperature == 0)
            {
                await context.PostAsync("Difficult it is, to read the temperature. (Check device connectivity.)");
            }
            else
            {
                await context.PostAsync(String.Format("{0:0.00} degrees Centigrade, the temperature is.", temperature));
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
                await context.PostAsync($"Great disturbance of the Force with the device. (Am I talking to the right device?): {e.ToString()}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Meditation I need: {e.ToString()}");
            }

            if (humidity == 0)
            {
                await context.PostAsync("Difficult it is, to read the humidity. (Check device connectivity.)");
            }
            else
            {
                await context.PostAsync(String.Format("{0:0.00} % r H, the humidity is.", humidity));
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
                await context.PostAsync($"Great disturbance of the Force with the device. (Am I talking to the right device?): {e.ToString()}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Meditation I need: {e.ToString()}");
            }

            if (probabilityRain == 0)
            {
                await context.PostAsync("Difficult it is, to predict the probability of rain. (Check device connectivity.)");
            }
            else
            {
                if (probabilityRain >= 0.5)
                    await context.PostAsync(String.Format("I sense the rain is coming. ({0:0.00} %.) Meditation I need, to close the window for you.", probabilityRain * 100));
                else
                    await context.PostAsync(String.Format("{0:0.00} %, the probability of rain I sense is.", probabilityRain * 100));

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

            string message = "";

            try
            {
                message += $"deviceId={deviceModel.DeviceId}";
                message += $"time={deviceModel.Time}";
                message += $"location=long:{deviceModel.Location.Longitude},lat:{deviceModel.Location.Latitude}";
                message += $"temperature={deviceModel.Temperature}";
                message += $"humidity={deviceModel.Humidity}";
                message += $"forecast={deviceModel.Forecast}";

                await context.PostAsync(message);
                message = "";

                foreach (VirtualDeviceElement element in deviceModel.Elements)
                {
                    message += $"elementType={element.ElementType}";
                    message += $"SwitchStatus={element.SwitchStatus}";
                    message += $"SetValue={element.SetValue}";
                    //message += $"PowerConsumptionValue={element.PowerConsumptionValue}";

                    await context.PostAsync(message);


                }
            }
            catch (Exception e)
            {
                await context.PostAsync($"Meditation I need: {e.ToString()}");
            }
            context.Wait(this.MessageReceived);
        }


        public async Task ImageCaptureAndDescribe(IDialogContext context, string deviceId)
        {
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
                        var parsedInternalJObject = JObject.Parse((string)item.Value);
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
                await context.PostAsync($"Great disturbance of the Force with the device. (Am I talking to the right device?): {e.ToString()}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Meditation I need: {e.ToString()}");
            }

        }



        [LuisIntent("Capture.Image")]
        public async Task LuisCaptureImage(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"CaptureImage.");
            await context.PostAsync($"What's happening in the room, I will meditate into...");

            await ImageCaptureAndDescribe(context, _iotDeviceId);            
            
            context.Wait(this.MessageReceived);
        }


        static int iToPickOne = 0;


        private async Task<List<Attachment>> GetAttachmentsDescribingImageWithAdaptiveCard(IDialogContext context, string imageBlobUrl, string description)
        {
            Attachment attachment = new Attachment();
            string descriptionAdded = description;

            AdaptiveCard imageDescription = new AdaptiveCard();

            Microsoft.ProjectOxford.Face.Contract.Face[] detecedFaces = await _cognitiveServicesHelper.GetFacesAsync(context, imageBlobUrl);

            if (detecedFaces != null) // there could be 0 person in the image
            {
                
                description += $"\nAlso I sense {detecedFaces.Length} face(s).";

                imageDescription.Title = "In Room Now";
                imageDescription.Body.Add(new TextBlock()
                {
                    Text = "Who are there?",
                    Weight = TextWeight.Bolder,
                    IsSubtle = false
                });
                imageDescription.Body.Add(new ColumnSet()
                {
                    Columns = new List<Column>()
                {
                    new Column()
                    {
                        Size = "2",
                        Items = new List<CardElement>()
                        {
                            new AdaptiveCards.Image()
                            {
                                Url = imageBlobUrl,
                                Size =  ImageSize.Stretch

                            }
                        }
                    },
                    new Column()
                    {
                        Size = "1",
                        Items = new List<CardElement>()
                        {
                            new AdaptiveCards.TextBlock()
                            {
                                Text = descriptionAdded
                            },
                            new AdaptiveCards.TextBlock()
                            {
                                Text = "The other things I see...(tags):"
                            }
                        }

                    }
                }
                });

                //summary
                string tempMessage = "";
                if (detecedFaces != null && detecedFaces.Length == 0)
                {
                    tempMessage = "No people identified in the room";
                }
                else if (detecedFaces.Length > 0)
                {
                    tempMessage = "People identified in the room";
                }

                imageDescription.Body.Add(new TextBlock()
                {
                    Text = tempMessage,
                    Weight = TextWeight.Bolder,
                    IsSubtle = false,
                    Separation = SeparationStyle.Strong
                });
                
                //for each person identified:
                foreach (Microsoft.ProjectOxford.Face.Contract.Face face in detecedFaces)
                {
                    string representativeEmotion = "";
                    iToPickOne = 1;
                    foreach (var item in face.FaceAttributes.Emotion.ToRankedList())
                    {
                        if (iToPickOne == 1)
                            representativeEmotion = $"{item.Key} ({item.Value}), ";
                        iToPickOne++;
                    }

                    //snapshot and description for each person
                    imageDescription.Body.Add(new ColumnSet()
                    {
                        Columns = new List<Column>()
                    {
                        new Column()
                        {
                            Size = "1",
                            Items = new List<CardElement>()
                            {
                                new AdaptiveCards.Image()
                                {
                                    Size = ImageSize.Small,
                                    Url = imageBlobUrl
                                }
                            }
                        },
                        new Column()
                        {
                            Size = "5",
                            Items = new List<CardElement>()
                            {
                                new TextBlock()
                                {
                                    Text = $"{face.FaceAttributes.Age} years old {face.FaceAttributes.Gender} - {representativeEmotion}"
                                    //text += $"left:{face.FaceRectangle.Left}, width:{face.FaceRectangle.Width}";

                                }
                            }
                        }
                    }
                    });

                }
                //for each person
            
            }

            attachment.ContentType = AdaptiveCard.ContentType;
            attachment.Content = imageDescription;

            List<Attachment> attachments = new List<Attachment>
            {
                attachment
            };

            return attachments;
        }

        private async Task<List<Attachment>> GetAttachmentsDescribingImageWithHeroCard(IDialogContext context, string imageBlobUrl, string description)
        {
            List<Attachment> attachments = new List<Attachment>();

            string descriptionAdded = description;

            Microsoft.ProjectOxford.Face.Contract.Face[] detecedFaces = await _cognitiveServicesHelper.GetFacesAsync(context, imageBlobUrl);

            //general description
            attachments.Add(new HeroCard()
            {
                Title = "Who are there?",
                Subtitle = $"{description}. I sense {detecedFaces.Length} face(s). The other things I sense is...(tags)",
                Images = new List<CardImage>()
                {
                    new CardImage()
                    {
                        Url = imageBlobUrl
                    }
                },
            }.ToAttachment());

            //people summary
            string tempMessage = "";
            if (detecedFaces != null && detecedFaces.Length == 0)
                tempMessage = "No people identified in the room";
            else if (detecedFaces.Length > 0)
                tempMessage = "People identified in the room";
            attachments.Add(new ThumbnailCard() { Title = tempMessage }.ToAttachment());

            //for each person identified:


            
            
            //for each person identified:
            foreach (Microsoft.ProjectOxford.Face.Contract.Face face in detecedFaces)
            {
                string representativeEmotion = "";
                iToPickOne = 1;
                foreach (var item in face.FaceAttributes.Emotion.ToRankedList())
                {
                    if (iToPickOne == 1)
                        representativeEmotion = $"{item.Key} ({item.Value}), ";
                    iToPickOne++;
                }

                //snapshot and description for each person
                attachments.Add(new ThumbnailCard()
                {
                    Images = new List<CardImage>
                {
                    new CardImage
                    {
                        Url = imageBlobUrl
                    }
                },
                    Title = "person1 (name if identified)",
                    Subtitle = $"{face.FaceAttributes.Age} years old {face.FaceAttributes.Gender} - {representativeEmotion}",
                }.ToAttachment());

            }

            return attachments;

        }
        
        private async Task DescribeImage(IDialogContext context, string imageBlobUrl)
        {
            string description = await _cognitiveServicesHelper.GetDescriptionAsync(context, imageBlobUrl);
            List<CardAction> cardButtons = new List<CardAction>();

            //IChannelCapability a = context.Activity.ChannelData;

            var message = context.MakeMessage();

            //Skype does not support Adaptive Card today. 
            if (0 != 0) //TODO: Replace this condition with Channel Detector.
            {
                foreach (Attachment attachment in await GetAttachmentsDescribingImageWithAdaptiveCard(context, imageBlobUrl, description))
                {
                    message.Attachments.Add(attachment);
                }
            }
            else //use Skype-compatible attachments
            {
                foreach (Attachment attachment in await GetAttachmentsDescribingImageWithHeroCard(context, imageBlobUrl, description))
                {
                    message.Attachments.Add(attachment);
                }

            }
            
            await context.PostAsync(message);

        }


        [LuisIntent("Find.Person")]
        public async Task LuisFindPerson(IDialogContext context, LuisResult result)
        {
            if (_DEBUG) await context.PostAsync($"FindPerson.");

            await context.PostAsync("No tracking wearables I can focus on to find people today, Able to find someone from the camera, I might be. Going into deep meditation.");

            try
            {
                await ImageCaptureAndDescribe(context, _iotDeviceId);
            }
            catch (Exception e)
            {
                await context.PostAsync($"Meditation I need: {e.ToString()}");
            }

            context.Wait(this.MessageReceived);
        }


    }

}