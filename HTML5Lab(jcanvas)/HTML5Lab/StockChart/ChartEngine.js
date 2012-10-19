/// <reference path="StockDataExchanger.js" />
/// <reference path="StockDataConverter.js" />
/// <reference path="ViewModel/TickDataViewModel.js" />
/// <reference path="../Lib/datetime.1-3.js" />
/// <reference path="Controls/ChartBaseControl.js" />
/// <reference path="../Html5Framework/UiBase.js" />

// ChartEngine
ChartEngine = function (canvas) {
    ChartBaseControl.prototype.constructor.call(this, canvas);
    this.TimeAxis.Time = DateTime.now();
    this.TimeAxis.MillisecondsPerPixel = 1000 * 60 / 50;
    this.TimeAxis.ValuesPerPixel = 1;

    var timeAxisControl = new TimeAxisControl(this.Canvas);
    timeAxisControl.TimeAxis = this.TimeAxis;
    timeAxisControl.Bound.Pos.X = 0;
    timeAxisControl.Bound.Pos.Y = 0;
    timeAxisControl.Bound.Size.X = this.Canvas.width();
    timeAxisControl.Bound.Size.Y = this.Canvas.height();
    this.Controls.push(timeAxisControl);

    this.addCandlestick();

    var ref = this;

    $(canvas).mousedown(function (a) {
        var args = new EventArgs();
        ref.onmousedown(a, args);
        ref.draw(a);
    });
    $(canvas).mousemove(this, function (a) {
        var args = new EventArgs();
        ref.onmousemove(a, args);
        ref.draw(a);
    });

    $(canvas).mouseup(function (a) {
        var args = new EventArgs();
        ref.onmouseup(a, args);
    });

    $(canvas).mouseleave(function (a) {
        var args = new EventArgs();
        ref.onmouseleave(a, args);
        ref.draw(a);
    });

    $(canvas).mouseenter(function (a) {
        var args = new EventArgs();
        ref.onmouseenter(a, args);
        ref.draw(a);
    });

    var stockDataExchanger = new StockDataExchanger();
    stockDataExchanger.setOnConnectionStatusChanged(function (a) {
        if (a == "Opened") {
            $("#noSupportBrower").css("visibility", "hidden");
            $("#supportedBrower").css("visibility", "visible");
        }
        else if (a == "NotSupportBrowser") {
            $("#noSupportBrower").css("visibility", "visible");
            $("#supportedBrower").css("visibility", "hidden");
        }
        else if (a == "Closed") {
            $("#noSupportBrower").css("visibility", "hidden");
            $("#supportedBrower").css("visibility", "visible");
            $("#supportedBrower").css("background-color", "red");
            $("#supportedBrower").html("Connection closed");
        }
    });


    stockDataExchanger.setOnReceiveTickData(function (tick) {
        var st = new Candlestick(ref.Canvas, ref.Context);
        st.setTickData(tick);
        st.TimeAxis = ref.TimeAxis;

        ref.TimeAxis.Time = tick.TTime.addMilliseconds(-(ref.Canvas.width() * ref.TimeAxis.MillisecondsPerPixel));

        var removal = []
        for (var i = 0; i < this.Controls; i++) {
            if (this.Controls[i].TickData != undefined) {
                var c = ref.TimeAxis.Time.TTime.compareTo(ref.Controls[i].TickData.TTime);
                if (c >= 0)
                    continue;
                removal.push(ref.Controls[i]);
            }
        }

        for (var i = 0; i < removal.length; i++) {
            ref.Controls.remove(removal[i]);
        }

        $("#log").html(tick);

        ref.Controls.push(st);

        ref.draw();
    });

    stockDataExchanger.connect();
}

ChartEngine.prototype = new ChartBaseControl();

ChartEngine.prototype.draw = function (args) {

    this.Canvas.clearCanvas();
    this.Canvas.drawRect({
        fillStyle: "black",
        strokeStyle: "violet",
        strokeWidth: 2,
        x: 1,
        y: 1,
        width: this.Canvas.width() - 2,
        height: this.Canvas.height() - 2

    });

    ChartBaseControl.prototype.draw.call(this, args);
    this.Canvas.drawText({
        text: "HERO HTML5 LAB: REALTIME STOCKTRADING CHART",
        x: 7,
        y: 17,
        align: "left",
        fillStyle:"red",
        font: "bold 10pt Tahoma"
        
    });
}

ChartEngine.prototype.addCandlestick = function () {

    var ticks = TickDataViewModelStatic.randomTicks2(50, 400, 60, 30, this.TimeAxis.Time.addSeconds(60), 20, 60);
    for (var i = 0; i < ticks.length; i++) {
        var st = new Candlestick(this.Canvas, this.Context);
        st.setTickData(ticks[i]);
        st.TimeAxis = this.TimeAxis;
        this.Controls.push(st);
    }
}