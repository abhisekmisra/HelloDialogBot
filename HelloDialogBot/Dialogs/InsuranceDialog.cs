using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HelloDialogBot.Dialogs
{
    [Serializable]
    public class InsuranceDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //Set this flag as true when the user initiates communication with the Bot for the first time. We will maintain the data in the context
            //Bot will not action anything further if this flag is true except sending the welcome message.
            context.UserData.SetValue<bool>("isFirstTextFromUser", true);
            //Send welcome message to user. Ask for user name
            await context.PostAsync(@"Welcome to ""Quick Insurance - World's best Insurance Provider"". May I know your name please. ");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            string insurerName = string.Empty;
            string insuranceID = string.Empty;
            string complaint = string.Empty;
            bool isFirstTextFromUser = false;
            //Gets value of flag isFirstTextFromUser from the context
            context.UserData.TryGetValue<bool>("isFirstTextFromUser", out isFirstTextFromUser);
            //Gets value of flag insurerName from the context
            context.UserData.TryGetValue<string>("insurerName", out insurerName);
            //Gets value of flag insuranceID from the context
            context.UserData.TryGetValue<string>("insuranceID", out insuranceID);
            //Gets value of flag complaint from the context
            context.UserData.TryGetValue<string>("complaint", out complaint);
            var activity = await result as Activity;
            //Do not get into this loop if this is the first message from the user. This will avoid setting user's first message 
            //to insurerName
            if (!isFirstTextFromUser)
            {
                //insuranceName is blank corresponds that user is reponding to the Bot's reply - 
                //Welcome to "Quick Insurance - World's best Insurance Provider". May I know your name please.
                //Capture insurerName
                if (string.IsNullOrWhiteSpace(insurerName))
                {
                    //Set insurerName to context
                    context.UserData.SetValue<string>("insurerName", activity.Text);
                    //Prompt user to provide identification number
                    await context.PostAsync("Please provide your insurance identification number");
                }
                //insuranceID is blank and insurerName is not blank corresponds that user is reponding to the Bot's reply - 
                //Please provide your insurance identification number
                //Capture insuranceID
                else if (string.IsNullOrWhiteSpace(insuranceID))
                {
                    //Set insuranceID to the context
                    context.UserData.SetValue<string>("insuranceID", activity.Text);
                    //Prompt user to provide complaint
                    await context.PostAsync("Let me know your complaint");
                }
                //insuranceID is not blank and insurerName is not blank corresponds that user is reponding to the Bot's reply - 
                //Let me know your complaint
                //Capture complaint text
                else if (string.IsNullOrWhiteSpace(complaint))
                {
                    //Set complaint to the context
                    context.UserData.SetValue<string>("complaint", activity.Text);
                    //Wrap up
                    await context.PostAsync("Thank you. We have registered your complaint as Name : " + context.UserData.GetValue<string>("insurerName")
                        + ", Insurance ID : " + context.UserData.GetValue<string>("insuranceID")
                        + ", Complaint : " + activity.Text + ". Our representative will get back to you shortly. Please end the conversion chat");
                }
            }
            else
            {
                context.UserData.SetValue<bool>("isFirstTextFromUser", false);
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}