StockDataExchanger = function () {

    var _recieveTickFunc = undefined;
    var _connectionStatusChanged = undefined;
    this.setOnReceiveTickData = function (func) {
        _recieveTickFunc = func;
    }

    this.setOnConnectionStatusChanged = function (func) {
        _connectionStatusChanged = func;
    }


    function raiseEvent(event, data) {
        if (event != undefined)
            event(data);
    }

    this.sendMessage = function (a) {
        ws.send(a);
    }

    this.connect = function () {
        var support = "MozWebSocket" in window ? 'MozWebSocket' : ("WebSocket" in window ? 'WebSocket' : null);

        if (!support) {
            raiseEvent(_connectionStatusChanged, "NotSupportBrowser");
            return;
        }

        ws = new window[support]('ws://localhost:2011/sample');

        ws.onopen = function () {
            raiseEvent(_connectionStatusChanged, "Opened");
        }

        ws.onclose = function () {
            raiseEvent(_connectionStatusChanged, "Closed");
        }

        ws.onmessage = function (evt) {
            var data = StockDataConverter.exchangeMessage(evt.data);

            if (data.Name != undefined && data.Name == "TickDataViewModel")
                raiseEvent(_recieveTickFunc, data);
        };
    }
}