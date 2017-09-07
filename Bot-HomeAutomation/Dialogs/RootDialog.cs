using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using AdaptiveCards;
using System.Collections.Generic;

namespace Bot_HomeAutomation.Dialogs
{

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            await this.SendWelcomeMessageAsync(context, activity);
            
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context, Activity activity)

        {

            //TODO (MEDIUM)
            //move welcome message to before authentication, when you implemented authentication
            await context.PostAsync("Hi, I'm the GBB Home Automation bot. Let's get started.");

            //TODO (LOW)
            //show weather at user location using adaptive card
            //http://adaptivecards.io/visualizer/?card=/samples/cards/Weather%20Compact.json
            //but will need to get weather info from somewhere. 


            await context.PostAsync(InitializeWelcomeMessageAsync(activity));

            context.Call(new LuisBaseDialog(), this.MessageReceivedAsync);

        }

        static Activity InitializeWelcomeMessageAsync(Activity activity)
        {

            List<CardAction> cardButtons = new List<CardAction>
            {
                new CardAction()
                {
                    Title = "Light On",
                    Type = "imBack",
                    Value = "Turn on the light"
                },
                new CardAction()
                {
                    Title = "Light Off",
                    Type = "imBack",
                    Value = "Turn off the light"
                },
                new CardAction()
                {
                    Title = "Heater On",
                    Type = "imBack",
                    Value = "Turn on the heater"
                },
                new CardAction()
                {
                    Title = "Heater Off",
                    Type = "imBack",
                    Value = "Turn off the heater"
                },
                new CardAction()
                {
                    Title = "Cooler On",
                    Type = "imBack",
                    Value = "Turn on the cooler"
                },
                new CardAction()
                {
                    Title = "Cooler Off",
                    Type = "imBack",
                    Value = "Turn off the cooler"
                },
                new CardAction()
                {
                    Title = "Get Humidity",
                    Type = "imBack",
                    Value = "How is the humidity?"
                },
                new CardAction()
                {
                    Title = "Get Temperature",
                    Type = "imBack",
                    Value = "How is the temperature?"
                },
                new CardAction()
                {
                    Title = "Check Device Status",
                    Type = "imBack",
                    Value = "Status report"
                }
            };

            List<CardImage> cardImages = new List<CardImage>
            {
                new CardImage()
                {
                    Url = "https://bothomeauto.blob.core.windows.net/resources/AsiaGBB.png"
                }
            };

            var card = new HeroCard()
            {
                Title = "Home Automation Bot",
                Subtitle = "I can do things like turning on and off the light or fan, take a picture in the room etc.",
                Images = cardImages,
                Buttons = cardButtons
            };


            Activity replyToConversation = activity.CreateReply("");
            replyToConversation.Attachments = new List<Attachment>
            {
                card.ToAttachment()
            };

            return replyToConversation;
        }
    }
}