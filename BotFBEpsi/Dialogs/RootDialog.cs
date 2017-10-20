using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BotFBEpsi.Dialogs
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
           

            string reponseBot = GetKaamelott("http://kaamelott.underfloor.io/quote/rand");

            var activity = await result as Activity;
            string userName = "Désolé je n'ai pas trouvé ton nom :(";
           // activity = context.Activity;
            if (activity.From.Name != null)
            {
                userName = activity.From.Name;
            }
            var dateTimeStamp =  activity.Timestamp;
           
            string chanel = activity.ChannelId;
            // calcule le nombre de caractère dans le msg
            int length = (activity.Text ?? string.Empty).Length;

            //string reponseBot = $"Tu m'as écrit {activity.Text}, cela contient {length} caractères, ton nom est {userName} ! Tu m'écris de {chanel} le {dateTimeStamp}, ton id est {activity.From.Id}";
            await context.PostAsync(reponseBot);
            BotDBModel db = new BotDBModel();
            db.enregistrementUtilisateur(activity);
            bool messageEnregistre = db.enregistrementMessage(activity);
            db.enregistrementBot(activity, reponseBot);
            context.Wait(MessageReceivedAsync);
        }

        private string GetKaamelott(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
               
                    using (var webClient = new System.Net.WebClient())
                    {
                        var json = webClient.DownloadString(url);
                        Replique replique = JsonConvert.DeserializeObject<Replique>(json);
                    string reponse = replique.character + ": " + replique.quote;
                        byte[] bytes = Encoding.Default.GetBytes(reponse);
                        reponse = Encoding.UTF8.GetString(bytes);

                    return reponse;
                    }
                    
                
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    return errorText;
                    // log errorText
                }
                throw;
            }
            
        }


    }
}