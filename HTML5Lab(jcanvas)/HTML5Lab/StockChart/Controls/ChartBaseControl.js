/// <reference path="../Html5Framework/UiBase.js" />
/// <reference path="../Html5Framework/GeometryHelper.js" />
/// <reference path="../Lib/datetime.1-3.js" />


// -----------------------------------------
// Candlestick

ChartBaseControl = function (canvas) {
    UserControl.prototype.constructor.call(this, canvas);    
}

ChartBaseControl.prototype = new UserControl();

ChartBaseControl.prototype.TimeAxis = new function () {
    this.Time = DateTime.now();
    this.MillisecondsPerPixel = undefined;
    this.ValuesPerPixel = undefined;

    this.PixelsPerMillisecond = function () {
        return 1 / this.MillisecondsPerPixel;
    }
    this.PixelsPerValue = function () {
        return 1 / this.ValuesPerPixel;
    }
}