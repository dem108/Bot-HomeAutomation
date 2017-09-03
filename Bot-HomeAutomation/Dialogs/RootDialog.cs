using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

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

            await this.SendWelcomeMessageAsync(context);

            /*
            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");
            context.Wait(MessageReceivedAsync);
            */
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)

        {

            await context.PostAsync("Hi, I'm the GBB Home Automation bot. Let's get started.");

            await context.PostAsync("I can do: Lights on, Lights off.");
            
            context.Call(new EchoDialog(), this.MessageReceivedAsync);

        }
    }
}