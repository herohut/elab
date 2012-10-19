/// <reference path="../Html5Framework/UiBase.js" />
/// <reference path="../Html5Framework/GeometryHelper.js" />
/// <reference path="ChartBaseControl.js" />
/// <reference path="../Lib/date.format.js" />
/// <reference path="../Lib/date.js" />
/// <reference path="../Html5Framework/Utils.js" />
/// <reference path="RES.js" />


// -----------------------------------------
// TimeAxisControl

TimeAxisControl = function (canvas, context) {
    ChartBaseControl.prototype.constructor.call(this, canvas, context);
    this.MouseOrdinate = new Vector2D(0, 0);
    this.MouseOrdinateTime = DateTime.now();
    this.DrawIndicator = false;

    this.calMouseOrdinateVals = function (a) {
        this.MouseOrdinate.X = a.offsetX;
        this.MouseOrdinate.Y = a.offsetY;

        var m = (this.MouseOrdinate.X - this.Bound.Pos.X) / (this.TimeAxis.PixelsPerMillisecond());
        this.MouseOrdinateTime = this.TimeAxis.Time.addMilliseconds(m);
    }
}

TimeAxisControl.prototype = new ChartBaseControl();

TimeAxisControl.prototype.ValStep = 40;
TimeAxisControl.prototype.draw = function () {
    ChartBaseControl.prototype.draw.call(this);

    this.Context.fillStyle = 'rgba(0, 128, 0, 0.5)';
    this.Context.strokeStyle = 'rgba(0, 128, 0, 0.5)';
    this.Context.lineWidth = 0.5;
    this.Context.font = "bold 7pt Tahoma";
    this.Context.textBaseline = "middle";
    var p = this.Bound.Width() / (this.TimeAxis.PixelsPerMillisecond() * 1000 * 60);
    var d = 5;
    var y = this.Bound.Bottom() - 10;
    var delta = this.TimeAxis.PixelsPerMillisecond() * 1000 * 60;
    var time = this.TimeAxis.Time;
    var format = "hh:mm";
    var tw = this.Context.measureText(format).width / 2;
    for (var i = 0; i < p; i++) {
        var txt = time.format(format);

        this.Context.strokeLine(d + tw, 2, d + tw, this.Bound.Bottom() - 18);
        this.Context.fillText(txt, d, y);

        d += delta;
        time = time.addMinutes(1);
    }

    var val = 0;
    var maxVal = this.TimeAxis.ValuesPerPixel * this.Bound.Height();

    while (val < maxVal) {
        y = this.Canvas.height - val * this.TimeAxis.PixelsPerValue();
        this.Context.strokeLine(20, y - 20, this.Bound.Right(), y - 20);
        this.Context.fillText(val, 0, y - 20);
        val += this.ValStep;
    }


    if (!this.DrawIndicator)
        return;

    // V-H indicator
    this.Context.strokeStyle = RES.White50;
    this.Context.strokeLine(this.MouseOrdinate.X, 0, this.MouseOrdinate.X, this.Bound.Bottom());
    this.Context.strokeLine(0, this.MouseOrdinate.Y, this.Bound.Right(), this.MouseOrdinate.Y);
    this.Context.textBaseline = "middle";

    // V-H value indicator
    var h = 12;
    var w = 30;
    var x = this.MouseOrdinate.X - 2 - w;
    var y = this.Bound.Bottom() - h - 5;
    this.Context.roundedRect(x, y, w, h, 2, true, false);

    this.Context.fillStyle = RES.White50;
    y = y + h / 2;
    x = x + w / 2 - tw / 2 - 5;
    this.Context.fillText(this.MouseOrdinateTime.format(format), x, y);

    x = 5;
    y = this.MouseOrdinate.Y + 2;
    this.Context.fillStyle = RES.Green50;
    this.Context.roundedRect(x, y, w, h, 2, true, false);
    this.Context.fillStyle = RES.White50;
    y = y + h / 2;
    x = x + w / 2 - tw / 2 - 5;

    var val = (this.Bound.Height() - this.MouseOrdinate.Y) * this.TimeAxis.ValuesPerPixel;
    this.Context.fillText(val, x, y);
}
TimeAxisControl.prototype.onmousedown = function (a, e) {
}


TimeAxisControl.prototype.onmousemove = function (a, e) {
    this.calMouseOrdinateVals(a);

}

TimeAxisControl.prototype.onmouseenter = function (a, e) {
    this.DrawIndicator = true;
}

TimeAxisControl.prototype.onmouseleave = function (a, e) {
    this.DrawIndicator = false;
}