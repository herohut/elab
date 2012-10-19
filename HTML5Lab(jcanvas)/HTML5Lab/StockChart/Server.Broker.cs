using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Timers;
using System.Web;
using HTML5Lab.StockChart.Server.Model;
using Newtonsoft.Json.Converters;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using SuperSocket.SocketEngine.Configuration;
using SuperWebSocket;

namespace HTML5Lab.StockChart
{
    public class Broker
    {
        private TickData _lastTick = new TickData { Symbol = "M$", Open = 10.2, High = 123.3, Close = 22.3, Low = 234.1, Market = 23.2, Id = Guid.NewGuid().ToString("D") };
        private readonly HttpApplicationState _app;
        public Broker(HttpApplicationState app)
        {
            this._app = app;
            LogUtil.Setup();
            StartSuperWebSocketByConfig();

        }

        public void Run()
        {
            var timer = new Timer(1000);
            var current = 10.2;
            const int deltaChange = 50;
            const int maxRange = 400;

            var time = DateTime.Now;
            timer.Elapsed += (s, a) =>
                                 {
                                     _lastTick = TickData.RandomATicks2(current, deltaChange, maxRange, 60, 30, time);
                                     var msg = BuildTransferMessage(_lastTick);

                                     SendToAll(msg);
                                     var msg2 =
                                         BuildTransferMessage(new ChatMessage
                                                                  {
                                                                      From = "System",
                                                                      To = "AllClients",
                                                                      Content = "Welcome",
                                                                      Time = DateTime.Now
                                                                  });
                                     SendToAll(msg2);

                                     current = Math.Max(Math.Min(TickData.Random.NextDouble() > 0.5 ? -TickData.Random.NextDouble() * deltaChange + current :
                                         TickData.Random.NextDouble() * deltaChange + current, maxRange), 0);
                                     time = time.AddSeconds(20);
                                 };

            timer.Start();
        }

        private static string BuildTransferMessage(IExchangeMessage msg)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(msg, new JsDateTimeConverter());
        }

        private static IExchangeMessage ExtractDataFromMessage(string str)
        {
            return null;
        }

        public class JsDateTimeConverter : JavaScriptDateTimeConverter
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (value is DateTime)
                {
                    var dt = (DateTime)value;
                    writer.WriteStartConstructor("DateTime");
                    writer.WriteValue(dt.Year);
                    writer.WriteValue(dt.Month);
                    writer.WriteValue(dt.Day);
                    writer.WriteValue(dt.Hour);
                    writer.WriteValue(dt.Minute);
                    writer.WriteValue(dt.Second);
                    writer.WriteValue(dt.Millisecond);
                    writer.WriteEndConstructor();
                }
            }
        }

        void StartSuperWebSocketByConfig()
        {
            var serverConfig = ConfigurationManager.GetSection("socketServer") as SocketServiceConfig;
            if (!SocketServerManager.Initialize(serverConfig))
                return;

            var socketServer = SocketServerManager.GetServerByName("SuperWebSocket") as WebSocketServer;

            _app["WebSocketPort"] = socketServer.Config.Port;

            socketServer.NewMessageReceived += socketServer_NewMessageReceived;
            socketServer.NewSessionConnected += socketServer_NewSessionConnected;
            socketServer.SessionClosed += socketServer_SessionClosed;

            if (!SocketServerManager.Start())
                SocketServerManager.Stop();
        }

        void socketServer_NewMessageReceived(WebSocketSession session, string e)
        {
            SendToAll(session.Cookies["name"] + ": " + e);
        }

        void socketServer_NewSessionConnected(WebSocketSession session)
        {
            lock (_sessionSyncRoot)
                _sessions.Add(session);

            SendToAll(BuildTransferMessage(new SysMessage { Cookie = session.Cookies["name"], Status = "connected" }));
        }

        void socketServer_SessionClosed(WebSocketSession session, CloseReason reason)
        {
            lock (_sessionSyncRoot)
                _sessions.Remove(session);

            if (reason == CloseReason.ServerShutdown)
                return;

            SendToAll(BuildTransferMessage(new SysMessage { Cookie = session.Cookies["name"], Status = "disconnected" }));
        }

        void SendToAll(string message)
        {
            lock (_sessionSyncRoot)
            {
                foreach (var s in _sessions)
                {
                    s.SendResponseAsync(message);
                }
            }
        }


        private readonly List<WebSocketSession> _sessions = new List<WebSocketSession>();
        private readonly object _sessionSyncRoot = new object();
    }
}