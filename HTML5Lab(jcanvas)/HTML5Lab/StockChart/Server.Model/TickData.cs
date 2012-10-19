using System;
using System.Collections.Generic;

namespace HTML5Lab.StockChart.Server.Model
{
    public class TickData: IExchangeMessage
    {
        public string MessageName
        {
            get { return "TICK_DATA"; }
        }

        public TickData() { }
        public TickData(string symbol, double high, double low, double open, double close, DateTime time)
        {
            this.Symbol = symbol;
            this.High = high;
            this.Low = low;
            this.Open = open;
            this.Close = close;
            this.Time = time;
        }

        public string Symbol { get; set; }

        public string Id { get; set; }

        public double Open { get; set; }

        public double Close { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Market { get; set; }

        public DateTime Time { get; set; }

        public static readonly Random Random = new Random();
        public static TickData RandomATick(double high, double low, DateTime time)
        {
            var open = low + Random.NextDouble() * (high - low);
            var close = low + Random.NextDouble() * (high - low);
            var t = new TickData("M$", high, low, open, close, time);
            return t;
        }
        public static List<TickData> RandomTicks(double maxDelta, double maxRange, double maxChange, double minChange, DateTime time, int seconds, int tickNum)
        {
            var output = new List<TickData>();

            for (var i = 0; i < tickNum; i++)
            {
                var low = Random.NextDouble() * maxRange;
                var high = low + Random.NextDouble() * (maxChange - minChange) + minChange;
                var t = RandomATick(high, low, time);
                output.Add(t);

                time = time.AddSeconds(seconds);
            }


            return output;
        }

        public static List<TickData> RandomTicks2(double deltaChange, double maxRange, double maxChange, double minChange, DateTime time, int seconds, int tickNum)
        {
            var output = new List<TickData>();

            var current = Random.NextDouble() * maxRange;
            for (var i = 0; i < tickNum; i++)
            {
                var low = current;
                var high = current + minChange + Math.Abs(minChange - maxChange) * Random.NextDouble();
                var t = RandomATick(high, low, time);
                output.Add(t);

                time = time.AddSeconds(seconds);

                current = Math.Max(Math.Min(Random.NextDouble() > 0.5 ? -Random.NextDouble() * deltaChange + current : Random.NextDouble() * deltaChange + current, maxRange), 0);
            }

            return output;
        }

        public static TickData RandomATicks2(double current, double deltaChange, double maxRange, double maxChange, double minChange, DateTime time)
        {
            var low = current;
            var high = current + minChange + Math.Abs(minChange - maxChange) * Random.NextDouble();
            var t = RandomATick(high, low, time);
            t.Id = t.Time.Ticks.ToString();
            return t;
        }


        
    }
}