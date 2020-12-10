using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;

namespace dfe.Client.Engine.Network
{
    
    public class ChatMessageEventArgs : EventArgs
    {
        public ChatMessageEventArgs(List<string> messages)
        {
            Messages = messages;
        }

        public List<string> Messages { get; set; }
    }

    public class ChatClient
    {
        public HubConnection chat_hub_conn;

        public event EventHandler<ChatMessageEventArgs> onChatUpdated;
        

        public List<string> messages = new List<string>();

        public ChatClient(HubConnection new_hub_connection)
        {
            
            chat_hub_conn = new_hub_connection;

            chat_hub_conn.On<string, string>("receiveMessage", (user, message) =>
            {
                updateChat(user, message);
            });

            chat_hub_conn.StartAsync();
        }

        public async Task sendMsg(string msg_data, string user_data)
        {
            await chat_hub_conn.SendAsync("sendMessage", user_data, msg_data);
        }

        protected void updateChat(string user, string message)
        {
            EventHandler<ChatMessageEventArgs> handler = onChatUpdated;

            var encoded_msg = $"{user}: {message}";
            messages.Add(encoded_msg);

            if(handler != null)
            {
                handler(this, new ChatMessageEventArgs(messages));
            }
        }

        public async ValueTask DisposeAsync()
        {
            await chat_hub_conn.DisposeAsync();
        }

    }
}
