using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web;
using HTML5Lab.StockChart;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using SuperSocket.SocketEngine.Configuration;
using SuperWebSocket;

namespace StockTradingApp
{
    public class Global : System.Web.HttpApplication
    {
        private Broker _broker;
        void Application_Start(object sender, EventArgs e)
        {
            _broker = new Broker(this.Application);
            _broker.Run();
        }

       
        void Application_End(object sender, EventArgs e)
        {
            SocketServerManager.Stop();
        }

        void Application_Error(object sender, EventArgs e)
        {
            //// Code that runs when an unhandled error occurs
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
