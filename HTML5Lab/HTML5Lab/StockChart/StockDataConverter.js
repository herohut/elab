/// <reference path="TickDataViewModel.js" />
/// <reference path="../Lib/datetime.1-3.js" />

StockDataConverter = function () {
    var tickDataMsg = "TICK_DATA";
    var chatMsg = "CHAT_MESSAGE";
    var sysMsg = "SYS_MESSAGE";

    this.exchangeMessage = function (input) {
        var input = eval("(" + input + ")");
        var msgType = input.MessageName;

        if (msgType == sysMsg) {
            return "sysmsg";
        }
        if (msgType == tickDataMsg) {
            var output = new TickDataViewModel(input.Symbol, input.High, input.Low, input.Open, input.Close, eval(input.Time));
            output.Name = "TickDataViewModel";
            return output;

        }
        if (msgType == chatMsg) {
            return "chatmsg";
        }

        throw "Not support ExhangeMessage";
    }
}

var StockDataConverter = new StockDataConverter();