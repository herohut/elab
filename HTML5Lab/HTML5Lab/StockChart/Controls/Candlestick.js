/// <reference path="../Html5Framework/UiBase.js" />
/// <reference path="../Html5Framework/GeometryHelper.js" />
/// <reference path="ChartBaseControl.js" />
/// <reference path="TickDataViewModel.js" />
/// <reference path="../Lib/datetime.1-3.js" />
/// <reference path="../Lib/jquery-1.6.3.min.js" />
/// <reference path="../Html5Framework/Utils.js" />
/// <reference path="RES.js" />



// -----------------------------------------
// Candlestick

Candlestick = function (canvas, context) {
    ChartBaseControl.prototype.constructor.call(this, canvas, context);
    this.EnableTooltip = true;
    this.TickData = new TickDataViewModel();
}

Candlestick.prototype = new ChartBaseControl();

Candlestick.prototype.setTickData = function (tick) {
    this.TickData = tick;
}

Candlestick.prototype.TickData = function () {
    this.CHigh = 0;
    this.CLow = 0;
    this.COpen = 0;
    this.CClose = 0;
    this.TTime = DateTime.now();
}

Candlestick.prototype.TickData = new TickDataViewModel();

Candlestick.prototype.draw = function () {
    ChartBaseControl.prototype.draw.call(this);
    this.Context.fillStyle = this.TickData.IsBuy() ? "red" : "white";
    this.Context.strokeStyle = "white";
    this.Context.lineWidth = 1.5;

    var yHigh = this.Canvas.height - this.TickData.High * this.TimeAxis.PixelsPerValue();
    var yLow = this.Canvas.height - this.TickData.Low * this.TimeAxis.PixelsPerValue();
    var yOpen = this.Canvas.height - this.TickData.Open * this.TimeAxis.PixelsPerValue();
    var yClose = this.Canvas.height - this.TickData.Close * this.TimeAxis.PixelsPerValue();
    
    var s = this.TickData.TTime.subtractDate(this.TimeAxis.Time).totalMilliseconds();
    this.Bound.Size.X = 9;
    this.Bound.Size.Y = yLow - yHigh;
    this.Bound.Pos.X = s * this.TimeAxis.PixelsPerMillisecond() - this.Bound.Size.X / 2;
    this.Bound.Pos.Y = yHigh;

    var cX = this.Bound.X() + this.Bound.Size.X / 2;
    this.Context.strokeLine(cX, yHigh, cX, yLow);
    this.Context.fillRect(this.Bound.X(), Math.min(yOpen, yClose), this.Bound.Size.X, Math.abs(yClose - yOpen));
    this.Context.strokeRect(this.Bound.X(), Math.min(yOpen, yClose), this.Bound.Size.X, Math.abs(yClose - yOpen));
}

Candlestick.prototype.drawTooltip = function (args) {
    this.Context.fillStyle = RES.Green50;
    var tW = 80;
    var tX = args.offsetX + 3;
    var tY = args.offsetY + 3;
    var dH = 15;
    this.Context.fillRect(tX, tY, tW, 110);

    var dX = tX + tW / 2;
    dX = dX - 2;
    tY = tY + 10;
    this.Context.fillStyle = RES.White50;
    this.Context.textAlign = "right";
    this.Context.fillText("Symbol:", dX, tY);
    this.Context.fillText("High:", dX, tY + dH);
    this.Context.fillText("Low:", dX, tY + dH * 2);
    this.Context.fillText("Open:", dX, tY + dH * 3);
    this.Context.fillText("Close:", dX, tY + dH * 4);
    this.Context.fillText("Market:", dX, tY + dH * 5);
    this.Context.fillText("Time:", dX, tY + dH * 6);

    this.Context.fillStyle = "red";
    this.Context.textAlign = "left";
    dX = dX + 4;
    this.Context.fillText(this.TickData.Symbol, dX, tY);
    this.Context.fillText(this.TickData.High.toFixed(2), dX, tY + dH);
    this.Context.fillText(this.TickData.Low.toFixed(2), dX, tY + dH * 2);
    this.Context.fillText(this.TickData.Open.toFixed(2), dX, tY + dH * 3);
    this.Context.fillText(this.TickData.Close.toFixed(2), dX, tY + dH * 4);
    this.Context.fillText(this.TickData.TMarket().toFixed(2), dX, tY + dH * 5);
    this.Context.fillText(this.TickData.TTime.format("hh:mm"), dX, tY + dH * 6);
}
