using System;
using Newtonsoft.Json;

namespace HTML5Lab.StockChart.Server.Model
{
    public class ChatMessage: IExchangeMessage
    {
        #region ITransferedMessage Members
        public string MessageName
        {
            get { return "CHAT_MESSAGE"; }
        }

        #endregion

        public string From { get; set; }

        public string To { get; set; }

        public string Content { get; set; }

        public DateTime Time { get; set; }
    }

    public class SysMessage: IExchangeMessage
    {
        public string MessageName
        {
            get { return "SYS_MESSAGE"; }
        }

        public string Cookie { get; set; }
        public string Status { get; set; }
    }
}