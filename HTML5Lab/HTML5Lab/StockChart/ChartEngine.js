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

    var timeAxisControl = new TimeAxisControl(this.Canvas, this.Context);
    timeAxisControl.TimeAxis = this.TimeAxis;
    timeAxisControl.Bound.Pos.X = 0;
    timeAxisControl.Bound.Pos.Y = 0;
    timeAxisControl.Bound.Size.X = this.Canvas.width;
    timeAxisControl.Bound.Size.Y = this.Canvas.height;
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

        ref.TimeAxis.Time = tick.TTime.addMilliseconds(-(ref.Canvas.width * ref.TimeAxis.MillisecondsPerPixel));

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

    this.Context.fillStyle = "black";
    this.Context.clearRect(0, 0, canvas.width, canvas.height);
    this.Context.lineWidth = 2;
    this.Context.fillRect(1, 1, canvas.width - 2, canvas.height - 2);

    ChartBaseControl.prototype.draw.call(this, args);

    this.Context.font = "bold 10pt Tahoma";
    this.Context.fillStyle = "red";
    this.Context.textBaseline = "middle";
    this.Context.fillText("HERO HTML5 LAB: REALTIME STOCKTRADING CHART", 7, 14);
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