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

            /*
            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");
            context.Wait(MessageReceivedAsync);
            */
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context, Activity activity)

        {

            //TODO
            //move welcome message to before authentication, when you implemented authentication
            await context.PostAsync("Hi, I'm the GBB Home Automation bot. Let's get started.");

            //TODO
            //show weather at user location using adaptive card
            //http://adaptivecards.io/visualizer/?card=/samples/cards/Weather%20Compact.json
            //but will need to get weather info from somewhere. 

            Activity replyToConversation = activity.CreateReply("");
            replyToConversation.Attachments = new List<Attachment>();


            
            /*



            var card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = "Home Automation Bot",
                Size = TextSize.Large,
                Weight = TextWeight.Bolder,
                Wrap = true
            });
            card.Body.Add(new TextBlock()
            {
                Text = "I can do things like turning on and off the light or fan, take a picture in the room etc.",
                IsSubtle = true,
                Wrap = true
            });
            
            card.Actions.Add(new OpenUrlAction()
            {
                Title = "Turn On Light",
//                Type = "imBack",
                Value = "Turn on the light"
            });

            
            Attachment attachement = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            */

            List<CardAction> cardButtons = new List<CardAction>();


            cardButtons.Add(new CardAction()
            {
                Value = "Turn on the light",
                Type = "imBack",
                Title = "Turn On Light"
            });
            cardButtons.Add(new CardAction()
            {
                Value = "Turn off the light",
                Type = "imBack",
                Title = "Turn Off Light"
            });


            var card = new HeroCard()
            {
                Title = "Home Automation Bot",
                Subtitle = "I can do things like turning on and off the light or fan, take a picture in the room etc.",
                Buttons = cardButtons
            };

            Attachment attachment = card.ToAttachment();

            replyToConversation.Attachments.Add(attachment);

            await context.PostAsync(replyToConversation);

            context.Call(new LuisBaseDialog(), this.MessageReceivedAsync);

        }
    }
}