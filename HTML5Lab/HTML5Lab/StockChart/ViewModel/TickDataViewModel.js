/// <reference path="../Lib/datetime.1-3.js" />
/// <reference path="../../Lib/datetime.1-3.js" />


TickDataViewModel = function (symbol,high, low, open, close, time) {
    this.Symbol = symbol;
    this.High = high;
    this.Low = low;
    this.Open = open;
    this.Close = close;
    this.TTime = time;

    // WRONG!!! INCRORRECT!!!
    this.TMarket = function () {
        return (this.High + this.Low + this.Close + this.Open) / 4;
    }
    

    this.IsBuy = function () {
        return this.Close > this.Open;
    }
    this.IsSell = function () {
        return !IsBuy();
    }

    this.randomATick = function (high, low, time) {
        var open = low + Math.random() * (high - low);
        var close = low + Math.random() * (high - low);
        var t = new TickDataViewModel("M$",high, low, open, close, time);
        return t;
    }
    this.randomTicks = function (maxDelta, maxRange, maxChange, minChange, time, seconds, tickNum) {
        var output = [];

        for (var i = 0; i < tickNum; i++) {
            var low = Math.random() * maxRange;
            var high = low + Math.random() * (maxChange - minChange) + minChange;
            var t = this.randomATick(high, low, time);
            output.push(t);

            time = time.addSeconds(seconds);
        }


        return output;
    }

    this.randomTicks2 = function (deltaChange, maxRange, maxChange, minChange, time, seconds, tickNum) {
        var output = [];

        var current = Math.random() * maxRange;
        for (var i = 0; i < tickNum; i++) {
            var low = current;
            var high = current + minChange + Math.abs(minChange - maxChange) * Math.random();
            var t = this.randomATick(high, low, time);
            output.push(t);

            time = time.addSeconds(seconds);

            current = Math.max(Math.min(Math.random() > 0.5 ? -Math.random() * deltaChange + current : Math.random() * deltaChange + current, maxRange), 0);
        }

        return output;
    }
}

var TickDataViewModelStatic = new TickDataViewModel();