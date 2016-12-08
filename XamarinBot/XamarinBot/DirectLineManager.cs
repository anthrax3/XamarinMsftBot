﻿using Microsoft.Bot.Connector.DirectLine;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamarinBot
{
    class DirectLineManager
    {
        private static string BOTSECRET = "TRCuflYOylQ.cwA.JQg.Pm3pHC3azJ0gFCHJ3A_omy1dZ86-O6vbE7Htm4R6Gno";
        private DirectLineClient client;
        private string currConvoId;
        private Conversation currConvo;
        private string watermark;

        public DirectLineManager ()
        {
            client = new DirectLineClient(BOTSECRET);
        }

        public async Task<List<string>> GetBotResponse(string userMsg)
        {
            currConvoId = CrossSettings.Current.GetValueOrDefault("convo_key", "");
            watermark = CrossSettings.Current.GetValueOrDefault("watermark_key", "");
            if (currConvoId == null || currConvoId == "")
            {
                currConvo = await client.Conversations.StartConversationAsync();
                currConvoId = currConvo.ConversationId;
                CrossSettings.Current.AddOrUpdateValue("convo_key", currConvoId);
            }

            IMessageActivity message = new Activity()
            {
                Text = userMsg,
                From = new ChannelAccount { Id = "aly" },
                Type = "message"
            };
            await client.Conversations.PostActivityAsync(currConvoId, message as Activity);

            var response = await client.Conversations.GetActivitiesAsync(currConvoId, watermark);
            // Update watermark to the most recent
            CrossSettings.Current.AddOrUpdateValue("watermark_key", response.Watermark);

            var responseArr = new List<string>();

            // TODO: Account for images and attachments
            for (var i = 1; i < response.Activities.Count; i++)
            {
                responseArr.Add(response.Activities[i].Text);
            }
            return responseArr;
        }
    }
}
