using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SignalR.Hubs;
using System.Linq;

namespace Eking.Lab.SignalR
{
    public class Chat : Hub, IDisconnect
    {
        public void BroadcastChatMsg(Message msg)
        {
            Clients.receiveMsg(msg);
        }

        public void LoginChat(string userName)
        {

            if (ChatData.I.ConnectionId2User.Values.Contains(userName))
                throw new Exception("This name already exist. Please choose other name");

            ChatData.I.ConnectionId2User[this.Context.ConnectionId] = userName;
            Clients.receiveText(userName + " connected");
            Clients.notifyUsers(ChatData.I.ConnectionId2User.Values.ToArray());
        }

        public class ChatData
        {
            private static ChatData _i;
            internal static ChatData I
            {
                get
                {
                    return _i ?? (_i = new ChatData());
                }
            }

            public Dictionary<string, string> ConnectionId2User = new Dictionary<string, string>();
        }

        public Task Disconnect()
        {
            string txt = null;
            if (ChatData.I.ConnectionId2User.ContainsKey(this.Context.ConnectionId))
            {
                txt = ChatData.I.ConnectionId2User[this.Context.ConnectionId];
                ChatData.I.ConnectionId2User.Remove(this.Context.ConnectionId);
            }

            Clients.notifyUsers(ChatData.I.ConnectionId2User.Values.ToArray());

            return txt == null ? null : Clients.receiveText(txt + " disconnected");
        }
    }



    public class ConnectMsg
    {
        public string ClientName { get; set; }
        public bool IsConnect { get; set; }
    }

    public class Message
    {
        public string Text { get; set; }
        public string ToClient { get; set; }
        public string FromUser { get; set; }
    }
}