using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace HTML5Lab.StockChart.Server.Model
{
    public interface IExchangeMessage
    {
        string MessageName { get; }
    }
}