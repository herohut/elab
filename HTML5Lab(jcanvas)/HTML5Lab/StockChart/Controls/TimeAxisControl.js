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


    var p = this.Bound.Width() / (this.TimeAxis.PixelsPerMillisecond() * 1000 * 60);
    var d = 5;
    var y = this.Bound.Bottom() - 10;
    var delta = this.TimeAxis.PixelsPerMillisecond() * 1000 * 60;
    var time = this.TimeAxis.Time;
    var format = "hh:mm";

    var lColor = 'rgba(0, 128, 0, 0.5)';
    for (var i = 0; i < p; i++) {
        var txt = time.format(format);

        this.Canvas.drawLine({
            x1: d,
            y1: 2,
            x2: d,
            y2: this.Bound.Bottom() - 18,
            strokeStyle: lColor,            
            lineWidth: 0.5
        });

        this.Canvas.drawText({
            fillStyle: RES.White50,
            x: d,
            y: y,
            text: txt,
            align: "center",
            baseline: "middle"
        });

        d += delta;
        time = time.addMinutes(1);
    }

    var val = 0;
    var maxVal = this.TimeAxis.ValuesPerPixel * this.Bound.Height();

    while (val < maxVal) {
        y = this.Canvas.height() - val * this.TimeAxis.PixelsPerValue();
        this.Canvas.drawLine({
            x1: 20,
            y1: y - 20,
            x2: this.Bound.Right(),
            y2: y - 20,
            strokeWidth: 1,
            strokeStyle: lColor,
        });

        this.Canvas.drawText({
            text: val,
            x: 0,
            y: y - 20,
            fillStyle: RES.White50
        });
        val += this.ValStep;
    }


    if (!this.DrawIndicator)
        return;

    // V-H indicator    
    this.Canvas.drawLine({
        x1: this.MouseOrdinate.X,
        y1: 0,
        x2: this.MouseOrdinate.X,
        y2: this.Bound.Bottom(),
        strokeStyle: RES.White50,
        lineWidth: 0.5

    });

    this.Canvas.drawLine({
        x1: 0,
        y1: this.MouseOrdinate.Y,
        x2: this.Bound.Right(),
        y2: this.MouseOrdinate.Y,
        strokeStyle: RES.White50,
        lineWidth: 0.5
    });

    // V-H value indicator
    var h = 12;
    var w = 30;
    var x = this.MouseOrdinate.X - 2 - w;
    var y = this.Bound.Bottom() - h - 5;

    this.Canvas.drawRect({
        x: this.MouseOrdinate.X - 2 - w,
        y: this.Bound.Bottom() - h - 5,
        width: w,
        height: h,
        roundCorner: 2,
        fillStyle: RES.Green50
    });

    y = y + h / 2;
    x = x + w / 2 - 5;

    this.Canvas.drawText({
        x: x,
        y: y,
        fillStyle: RES.White50,
        text: this.MouseOrdinateTime.format(format)
    });

    x = 5;
    y = this.MouseOrdinate.Y + 2;

    this.Canvas.drawRect({
        x: x,
        y: y,
        height: h,
        width: w,
        roundCorner: 2,
        fillStyle: RES.Green50
    });

    y = y + h / 2;
    x = x + w / 2 - 5;

    var val = (this.Bound.Height() - this.MouseOrdinate.Y) * this.TimeAxis.ValuesPerPixel;

    this.Canvas.drawText({
        text: val,
        x: x,
        y: y,
        fillStyle: RES.White50
    });
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